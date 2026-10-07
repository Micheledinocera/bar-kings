---
name: lint-wiki
description: Check the project wiki (docs/wiki) for broken links and anchors, pages missing from their folder index or without a description, folders without index.md, invalid frontmatter, open decisions missing from the root radar, code and asset paths that no longer exist, oversized pages and pages to verify or consolidate, then fix what it can. Use when the user runs /lint-wiki; /ingest runs it at its start and before its commit, /close after the ingest.
---

# /lint-wiki — check the wiki

The rules being checked are in `docs/CLAUDE.md`, sections **Purpose** (the wiki is an index for the agent) and **Wiki pages**. Read them before fixing anything.

## Example

```
/lint-wiki
```

No arguments. It works on any branch, but fixes are committed only on a `feature/*` branch (wiki changes reach `develop` only through a PR).

## 1. Mechanical checks

From the repo root:

```bash
node .claude/skills/lint-wiki/lint-wiki.mjs
```

`--today=YYYY-MM-DD` runs the maintenance checks as if it were that day (to test the thresholds).

It prints one `ERROR` or `WARN` line per problem and exits 1 if there are errors. It checks:

- every folder of `docs/wiki/` has an `index.md`, linked from the parent folder's `index.md`;
- every page is listed in its folder's `index.md`;
- every relative link resolves, and every `#anchor` matches a heading of its target;
- every entry of an `index.md` that lists a page or sub-index has a one-line description (`- [Title](./page.md): description`);
- every page has the frontmatter (`tipo`, `stato` for decisions, `aggiornato`, `verificato`, `consolidato` if present as a date, `fonti` under `raw/compiled/` and existing, `sostituita_da` for a superseded decision);
- the root `index.md` lists every decision `aperta` or `in analisi` (the radar);
- the reverse: the radar lists no decision `presa` or `superata`, and the stato shown in the radar and at the end of each area index entry (`· <stato>`) matches the page's frontmatter;
- every repo path cited in inline code (`Assets/…`, `Packages/…`, `ProjectSettings/…`, `.claude/…`, `docs/…`) exists, so the wiki never points the agent to removed or moved code or assets;
- warning: a folder with more than 12 pages, to split into sub-folders;
- warning: a page over 200 lines without an index table of its sections (`| [Name](#anchor) | …`), or over 800 lines / 40 KB (to split);
- warning: a `##` section over 80 lines (the entity deserves its own page);
- warning: a page with under 10 lines of content, decisions excepted (to merge into a parent or sibling page);
- maintenance warnings (rules and thresholds in `docs/CLAUDE.md`, **Freshness and consolidation**):
  - `stale`: `verificato` older than the threshold of the page's kind;
  - `code changed`: in `tecnico/`, a cited file (repo-relative path) has a commit after `verificato`;
  - `consolidate`: 4 or more features in `fonti` with raw files dated after `consolidato`;
  - `missing "## Alternative scartate"`: a decision `presa` or `superata` without the section.

The size rules are in `docs/CLAUDE.md`, **Granularity and size**.

## 2. Fix

Fix every error in the wiki, following `docs/CLAUDE.md`: add the missing index entries and descriptions, repair or remove broken links and anchors, update or remove code paths that moved or no longer exist (check the code with `git log --follow` / a search before editing), complete the frontmatter from the page content and its sources (a missing `verificato` takes the value of `aggiornato`: the page was checked when it was written). Never invent content: if a fix needs information the wiki and its sources do not have (e.g. which decision replaces a superseded one), leave the error and report it.

Size warnings are for the user: report them, do not split folders on your own unless asked. Maintenance warnings are handled by the ingest (step 7 of **Ingest** in `docs/CLAUDE.md`), within its scope; run alone, the lint only reports them: never set `verificato` or `consolidato` here.

Run the script again until it reports no errors, or only errors you cannot fix.

## 3. Commit

- **Called by `/ingest`**: no commit of its own; the fixes go into the ingest commit.
- **Called by `/close`, or by the user** on a `feature/*` branch, with changes: one commit with only the wiki:
  ```bash
  git add -A docs/wiki
  git commit -m "docs(wiki): fix lint issues" -- docs/wiki
  ```
  End the commit message with the attribution line required by the session.
- On any other branch: do not commit; report the fixes as pending and tell the user to start a feature.

## 4. Report

Errors fixed, errors left (with the reason), warnings, and the commit hash if any.
