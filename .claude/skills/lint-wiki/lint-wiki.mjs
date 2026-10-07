#!/usr/bin/env node
// Mechanical checks on docs/wiki. Rules: docs/CLAUDE.md, section "Wiki pages".
// Usage (from the repo root): node .claude/skills/lint-wiki/lint-wiki.mjs [--today=YYYY-MM-DD]
// Exit code 1 if there are errors; warnings alone exit 0.
// --today overrides the current date for the maintenance checks (to test the thresholds).

import { execFileSync } from "node:child_process";
import { existsSync, readdirSync, readFileSync, statSync } from "node:fs";
import { dirname, join, relative, resolve, sep } from "node:path";

const DOCS = resolve("docs");
const WIKI = join(DOCS, "wiki");
const TYPES = ["info", "decisione"];
const STATES = ["aperta", "in analisi", "presa", "superata"];
const RADAR_STATES = ["aperta", "in analisi"];
const MAX_PAGES_PER_FOLDER = 12;

const errors = [];
const warnings = [];
const rel = (p) => relative(DOCS, p).split(sep).join("/");
const error = (file, msg) => errors.push(`${rel(file)}: ${msg}`);
const warn = (file, msg) => warnings.push(`${rel(file)}: ${msg}`);

if (!existsSync(WIKI)) {
  console.error("docs/wiki not found: run from the repo root.");
  process.exit(2);
}

// --- collect folders and pages ---------------------------------------------

const folders = [];
const pages = [];
(function walk(dir) {
  folders.push(dir);
  for (const name of readdirSync(dir)) {
    const p = join(dir, name);
    if (statSync(p).isDirectory()) walk(p);
    else if (name.endsWith(".md") && name !== "index.md") pages.push(p);
  }
})(WIKI);

// --- helpers ----------------------------------------------------------------

function parseFrontmatter(text) {
  const m = text.match(/^---\r?\n([\s\S]*?)\r?\n---\r?\n/);
  if (!m) return null;
  const data = {};
  let listKey = null;
  for (const line of m[1].split(/\r?\n/)) {
    if (!line.trim() || line.trim().startsWith("#")) continue;
    const item = line.match(/^\s+-\s+(.*)$/);
    if (item && listKey) {
      data[listKey].push(unquote(item[1]));
      continue;
    }
    const kv = line.match(/^([\w-]+):\s*(.*)$/);
    if (!kv) continue;
    const [, key, raw] = kv;
    const value = raw.replace(/\s+#.*$/, "").trim();
    if (value === "") {
      data[key] = [];
      listKey = key;
    } else if (value.startsWith("[") && value.endsWith("]")) {
      data[key] = value.slice(1, -1).split(",").map((s) => unquote(s.trim())).filter(Boolean);
      listKey = null;
    } else {
      data[key] = unquote(value);
      listKey = null;
    }
  }
  return data;
}

function unquote(s) {
  return s.replace(/^["'](.*)["']$/, "$1");
}

// Relative markdown links: [text](target), skipping code blocks and inline code.
function links(text) {
  const body = text.replace(/```[\s\S]*?```/g, "").replace(/`[^`\n]*`/g, "");
  const out = [];
  for (const m of body.matchAll(/\]\(([^)\s]+)(?:\s+"[^"]*")?\)/g)) {
    const target = m[1];
    if (/^[a-z][a-z0-9+.-]*:/i.test(target) || target.startsWith("#")) continue;
    out.push(decodeURI(target.split("#")[0]));
  }
  return out;
}

function linkedFiles(file) {
  const text = readFileSync(file, "utf8");
  return new Set(links(text).map((t) => resolve(dirname(file), t)));
}

// --- checks -----------------------------------------------------------------

// 1. Every folder has an index.md, linked from the parent folder's index.md.
for (const dir of folders) {
  const index = join(dir, "index.md");
  if (!existsSync(index)) {
    error(dir, "folder without index.md");
    continue;
  }
  if (dir !== WIKI) {
    const parentIndex = join(dirname(dir), "index.md");
    if (existsSync(parentIndex) && !linkedFiles(parentIndex).has(index)) {
      error(parentIndex, `does not link the sub-index ${rel(index)}`);
    }
  }
  const count = readdirSync(dir).filter((n) => n.endsWith(".md") && n !== "index.md").length;
  if (count > MAX_PAGES_PER_FOLDER) {
    warn(dir, `${count} pages (> ${MAX_PAGES_PER_FOLDER}): consider sub-folders`);
  }
}

// 2. Every page is listed in its folder's index.md.
for (const page of pages) {
  const index = join(dirname(page), "index.md");
  if (existsSync(index) && !linkedFiles(index).has(page)) {
    error(index, `does not list ${rel(page)}`);
  }
}

// 3. Every relative link in the wiki resolves.
for (const file of [...pages, ...folders.map((d) => join(d, "index.md")).filter(existsSync)]) {
  for (const target of links(readFileSync(file, "utf8"))) {
    if (!existsSync(resolve(dirname(file), target))) error(file, `broken link: ${target}`);
  }
}

// 4. Frontmatter of every page.
const radar = [];
for (const page of pages) {
  const fm = parseFrontmatter(readFileSync(page, "utf8"));
  if (!fm) {
    error(page, "missing frontmatter");
    continue;
  }
  if (!TYPES.includes(fm.tipo)) error(page, `tipo must be one of: ${TYPES.join(", ")}`);
  if (!/^\d{4}-\d{2}-\d{2}$/.test(fm.aggiornato ?? "")) error(page, "aggiornato must be YYYY-MM-DD");
  if (!/^\d{4}-\d{2}-\d{2}$/.test(fm.verificato ?? "")) error(page, "verificato must be YYYY-MM-DD");
  if (fm.consolidato !== undefined && !/^\d{4}-\d{2}-\d{2}$/.test(fm.consolidato)) {
    error(page, "consolidato must be YYYY-MM-DD");
  }

  const fonti = Array.isArray(fm.fonti) ? fm.fonti : null;
  const isTechnical = rel(page).startsWith("wiki/tecnico/");
  if (!fonti) error(page, "fonti must be a list");
  else if (fonti.length === 0 && !isTechnical) error(page, "fonti is empty");
  for (const f of fonti ?? []) {
    if (!f.startsWith("raw/compiled/")) error(page, `fonte outside raw/compiled/: ${f}`);
    else if (!existsSync(join(DOCS, f))) error(page, `fonte not found: ${f}`);
  }

  if (fm.tipo === "decisione") {
    if (!STATES.includes(fm.stato)) error(page, `stato must be one of: ${STATES.join(", ")}`);
    if (fm.stato === "superata") {
      if (!fm.sostituita_da) error(page, "superata without sostituita_da");
      else if (!existsSync(resolve(dirname(page), fm.sostituita_da))) {
        error(page, `sostituita_da not found: ${fm.sostituita_da}`);
      }
    }
    if (RADAR_STATES.includes(fm.stato)) radar.push(page);
  } else if (fm.stato !== undefined) {
    error(page, "stato is only for tipo: decisione");
  }
}

// 5. The root index lists every decision still to take (radar).
const rootIndex = join(WIKI, "index.md");
if (existsSync(rootIndex)) {
  const linked = linkedFiles(rootIndex);
  for (const page of radar) {
    if (!linked.has(page)) error(rootIndex, `radar does not list the open decision ${rel(page)}`);
  }
}

// 5b. The reverse: every decision listed in an index shows its current stato, and the
// radar lists no decision already taken or superseded.
const stateOf = new Map();
for (const page of pages) {
  const fm = parseFrontmatter(readFileSync(page, "utf8"));
  if (fm?.tipo === "decisione") stateOf.set(page, fm.stato);
}
for (const index of folders.map((d) => join(d, "index.md")).filter(existsSync)) {
  const isRoot = index === rootIndex;
  for (const line of readFileSync(index, "utf8").split(/\r?\n/)) {
    for (const target of links(line)) {
      const page = resolve(dirname(index), target);
      const stato = stateOf.get(page);
      if (stato === undefined) continue;
      if (isRoot) {
        if (!RADAR_STATES.includes(stato)) {
          error(index, `radar lists ${rel(page)}, which is ${stato}: remove it`);
        } else {
          const cells = line.split("|").map((c) => c.trim()).filter(Boolean);
          if (cells[cells.length - 1] !== stato) {
            error(index, `radar shows "${cells[cells.length - 1]}" for ${rel(page)}, frontmatter says "${stato}"`);
          }
        }
      } else {
        const shown = line.match(/·\s*([^·]+?)\s*$/)?.[1];
        if (shown !== stato) {
          error(index, `entry for ${rel(page)} shows "${shown ?? "no stato"}", frontmatter says "${stato}" (end the line with "· ${stato}")`);
        }
      }
    }
  }
}

// --- agent indexing (docs/CLAUDE.md, section "Purpose") ----------------------

const REPO = resolve(".");
const stripCode = (text) => text.replace(/```[\s\S]*?```/g, "");

// GitHub-style heading slugs, with -1, -2 suffixes for duplicates.
const slugCache = new Map();
function slugs(file) {
  if (slugCache.has(file)) return slugCache.get(file);
  const seen = new Map();
  const out = new Set();
  for (const [, h] of stripCode(readFileSync(file, "utf8")).matchAll(/^#{1,6}\s+(.+?)\s*#*\s*$/gm)) {
    const base = h
      .toLowerCase()
      .replace(/[^\p{L}\p{N}\s_-]/gu, "")
      .replace(/\s/g, "-");
    const n = seen.get(base) ?? 0;
    seen.set(base, n + 1);
    out.add(n ? `${base}-${n}` : base);
  }
  slugCache.set(file, out);
  return out;
}

const allFiles = [...pages, ...folders.map((d) => join(d, "index.md")).filter(existsSync)];

// 6. Every page and sub-index listed in an index.md has a one-line description:
//    "- [Title](./page.md): description".
for (const dir of folders) {
  const index = join(dir, "index.md");
  if (!existsSync(index)) continue;
  for (const line of stripCode(readFileSync(index, "utf8")).split(/\r?\n/)) {
    const m = line.match(/^\s*-\s+\[[^\]]+\]\(([^)#\s]+\.md)\)(.*)$/);
    if (m && !/^:\s*\S/.test(m[2])) error(index, `entry without description: ${line.trim()}`);
  }
}

// 7. Every anchor (#section) in a relative link exists in its target.
for (const file of allFiles) {
  const body = stripCode(readFileSync(file, "utf8")).replace(/`[^`\n]*`/g, "");
  for (const m of body.matchAll(/\]\(([^)\s#]*)#([^)\s]+)\)/g)) {
    const [, target, anchor] = m;
    if (/^[a-z][a-z0-9+.-]*:/i.test(target)) continue;
    const targetFile = target ? resolve(dirname(file), decodeURI(target)) : file;
    if (!existsSync(targetFile) || !targetFile.endsWith(".md")) continue;
    if (!slugs(targetFile).has(decodeURIComponent(anchor))) error(file, `broken anchor: ${target}#${anchor}`);
  }
}

// 8. Code paths cited in inline code exist (repo-relative, no placeholders).
const CODE_PATH = /^(Assets|Packages|ProjectSettings|\.claude|docs)\/[^\s<>*{}]+$/;
for (const file of allFiles) {
  for (const [, code] of stripCode(readFileSync(file, "utf8")).matchAll(/`([^`\n]+)`/g)) {
    const path = code.replace(/[.,:;)]+$/, "");
    if (!CODE_PATH.test(path) || path.includes("...")) continue;
    if (!existsSync(join(REPO, path))) error(file, `code path not found: ${path}`);
  }
}

// 10. Granularity and size (docs/CLAUDE.md, "Granularity and size").
const MAX_LINES_WITHOUT_INDEX = 200;
const MAX_LINES = 800;
const MAX_BYTES = 40 * 1024;
const MAX_SECTION_LINES = 80;
const MIN_BODY_LINES = 10;
for (const page of pages) {
  const text = readFileSync(page, "utf8");
  const lines = text.split(/\r?\n/);
  const bytes = Buffer.byteLength(text, "utf8");

  if (lines.length > MAX_LINES || bytes > MAX_BYTES) {
    warn(page, `${lines.length} lines, ${Math.round(bytes / 1024)} KB: split the page along the code layout`);
  } else if (lines.length > MAX_LINES_WITHOUT_INDEX && !/^\|\s*\[[^\]]+\]\(#/m.test(text)) {
    warn(page, `${lines.length} lines without an index table of its sections (| [Name](#anchor) | …)`);
  }

  // Sections (## …) longer than MAX_SECTION_LINES: the entity deserves its own page.
  let current = null;
  let start = 0;
  let inCode = false;
  const check = (end) => {
    if (current && end - start > MAX_SECTION_LINES) {
      warn(page, `section "${current}" has ${end - start} lines (> ${MAX_SECTION_LINES}): move it to its own page`);
    }
  };
  lines.forEach((line, i) => {
    if (line.startsWith("```")) inCode = !inCode;
    if (!inCode && /^##\s/.test(line)) {
      check(i);
      current = line.replace(/^##\s+/, "");
      start = i;
    }
  });
  check(lines.length);

  // Tiny pages: body (without frontmatter and blank lines) under MIN_BODY_LINES.
  // Decisions are exempt: each one is its own page, listed by the radar.
  if (/^tipo:\s*decisione\s*$/m.test(text)) continue;
  const body = text.replace(/^---\r?\n[\s\S]*?\r?\n---\r?\n/, "").split(/\r?\n/).filter((l) => l.trim());
  if (body.length < MIN_BODY_LINES) {
    warn(page, `only ${body.length} lines of content: merge it into its parent or a sibling page`);
  }
}

// 11. Maintenance (docs/CLAUDE.md, "Freshness and consolidation"): pages to verify or
//     consolidate. Warnings only: the ingest handles them within its scope.
const TODAY = process.argv.find((a) => a.startsWith("--today="))?.slice(8) ?? new Date().toISOString().slice(0, 10);
const STALE_DAYS = { aperta: 30, "in analisi": 30, presa: 180, info: 90, tecnico: 180 };
const CONSOLIDATE_FEATURES = 4;
const days = (from, to) => Math.round((Date.parse(to) - Date.parse(from)) / 86_400_000);
const isDate = (s) => /^\d{4}-\d{2}-\d{2}$/.test(s ?? "");

// Date (YYYY-MM-DD) of the last non-merge commit touching any of the files, or null.
function lastCommit(files) {
  if (!files.length) return null;
  try {
    const out = execFileSync("git", ["log", "-1", "--no-merges", "--format=%cs", "--", ...files], {
      cwd: REPO,
      encoding: "utf8",
      stdio: ["ignore", "pipe", "ignore"],
    }).trim();
    return out || null;
  } catch {
    return null;
  }
}

for (const page of pages) {
  const text = readFileSync(page, "utf8");
  const fm = parseFrontmatter(text);
  if (!fm || !isDate(fm.verificato)) continue;
  const isTechnical = rel(page).startsWith("wiki/tecnico/");
  const kind = fm.tipo === "decisione" ? fm.stato : isTechnical ? "tecnico" : "info";
  const limit = STALE_DAYS[kind];

  // Stale: verificato older than the threshold of its kind.
  const age = days(fm.verificato, TODAY);
  if (limit !== undefined && age > limit) {
    warn(page, `stale: verificato ${fm.verificato}, ${age} days > ${limit} (${kind}): verify it`);
  }

  // Code changed: a file cited by a technical page (repo-relative path, e.g.
  // `Assets/Scripts/Shared/GameManager.cs`) has a commit after verificato.
  if (isTechnical) {
    const cited = new Set();
    for (const [, code] of stripCode(text).matchAll(/`([^`\n]+)`/g)) {
      const path = code.replace(/[.,:;)]+$/, "").replace(/^\.?\//, "");
      if (!/^[\w.@-]+(\/[\w .@[\]()-]+)+$/.test(path) || path.startsWith("docs/")) continue;
      if (existsSync(join(REPO, path))) cited.add(path);
    }
    const last = lastCommit([...cited]);
    if (last && last > fm.verificato) {
      warn(page, `code changed: a cited file has a commit on ${last}, after verificato ${fm.verificato}: verify it`);
    }
  }

  // Consolidate: features in fonti with raw files dated after consolidato.
  const since = isDate(fm.consolidato) ? fm.consolidato : "0000-00-00";
  const features = new Set();
  for (const f of Array.isArray(fm.fonti) ? fm.fonti : []) {
    const m = f.match(/^raw\/compiled\/([^/]+)\/(\d{4}-\d{2}-\d{2})-/);
    if (m && m[2] > since) features.add(m[1]);
  }
  if (features.size >= CONSOLIDATE_FEATURES) {
    warn(page, `consolidate: ${features.size} features in fonti since ${isDate(fm.consolidato) ? fm.consolidato : "the start"} (>= ${CONSOLIDATE_FEATURES})`);
  }

  // Discarded alternatives: required on decisions taken or superseded.
  if (fm.tipo === "decisione" && ["presa", "superata"].includes(fm.stato) && !/^## Alternative scartate\s*$/m.test(stripCode(text))) {
    warn(page, `missing "## Alternative scartate" (decisione ${fm.stato})`);
  }
}

// --- report -----------------------------------------------------------------

for (const e of errors) console.log(`ERROR  ${e}`);
for (const w of warnings) console.log(`WARN   ${w}`);
console.log(`\n${pages.length} pages, ${folders.length} folders: ${errors.length} errors, ${warnings.length} warnings`);
process.exit(errors.length ? 1 : 0);
