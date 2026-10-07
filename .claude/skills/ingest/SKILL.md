---
name: ingest
description: Process the current feature's raw sources and custom code into the project wiki (docs/wiki), move the processed raw files to docs/raw/compiled, run /lint-wiki and create one docs(wiki) commit. Use when the user runs /ingest, or when /close runs it. Only on a feature/* branch.
---

# /ingest — update the wiki from the raw sources

The rules for the wiki (what goes where, language, page format, conflicts) are in `docs/CLAUDE.md`. Read it first and follow its **Ingest** section. This skill adds the operational steps around it.

## Example

```
/ingest
```

No arguments: it works on the current feature branch.

Normally it runs only at the end of a feature, through `/close`. The user may run it earlier (start or middle of a feature) when they need the wiki updated sooner. Running it again later is safe: raw files already processed are in `compiled/` and are not read again, and code already documented leaves the pages unchanged.

## 1. Checks — stop if any fails

1. **Branch**: `git branch --show-current` must start with `feature/`. On `develop`, `main` or any other branch, stop and tell the user: wiki changes only reach `develop` through a feature PR.
2. **Feature name**: the branch name without `feature/` (e.g. `feature/deck-shuffle` → `deck-shuffle`).
3. **No unrelated pending edits in the wiki**: if `git status --porcelain docs/wiki` shows changes, stop and ask the user what to do with them. New or modified files in `docs/raw/` are fine: they are what we process.

## 2. Collect what to process

- Raw sources:
  - every file in `docs/raw/<feature-name>/`;
  - never anything under `docs/raw/compiled/`, except in **re-ingest mode** below, and never `docs/output/` (not versioned, not a source: see **Outputs** in `docs/CLAUDE.md`).
- Custom code of this feature:
  ```bash
  git fetch origin
  git log --no-merges --stat origin/develop..HEAD
  ```
  Ignore earlier `docs(wiki): …` commits of this feature.

- Maintenance warnings: run `node .claude/skills/lint-wiki/lint-wiki.mjs` and keep its `stale`, `code changed`, `consolidate` and `missing "## Alternative scartate"` warnings for the **Maintenance** step below.

If there are no raw sources and no code commits beyond wiki commits: tell the user there is nothing to process, and stop without changes (show the maintenance warnings, if any: they stay on until a feature touches that part of the wiki).

**Re-ingest mode** (only when `/close` asks for it, after taking `develop`'s wiki because of a wiki conflict — see *Two features on the wiki at the same time* in `docs/CLAUDE.md`): the sources are `docs/raw/<feature-name>/` **and** `docs/raw/compiled/<feature-name>/`, and all the feature's code commits, including those already covered by earlier `docs(wiki)` commits. Process them together, as one ingest on the current wiki.

## 3. Update the wiki

Follow the **Ingest** section of `docs/CLAUDE.md` (read `wiki/index.md` and the involved pages, link the Unity / plugin docs for technical pages, update or create pages, handle conflicts, update `index.md`).

Write for the agent, following the **Purpose** section of `docs/CLAUDE.md`: every page you add has a one-line description in its folder index; one `##` per entity named as in the code; code cited by existing repo-relative paths; consumers/callers updated. For code features, also update `wiki/tecnico/known-issues.md`: remove the items the feature fixed, add new ones to the right block with tag, path and link, and keep the block table counts in sync.

Decisions follow the **Decisions** section of `docs/CLAUDE.md`: every decision page has a `stato` in its frontmatter (`aperta`, `in analisi`, `presa`, `superata`). A study feature usually leaves its decision `in analisi`; a later feature on the same topic updates that page, it does not create a new one. A code feature that implemented a decision still `aperta` or `in analisi` (the user gave the ok during the feature) sets it to `presa`, with what was implemented as the choice.

Pages live in the **area** of their topic, with the frontmatter and indexes described in the **Wiki pages** section of `docs/CLAUDE.md`. When you add, move or remove a page, update its folder's `index.md`; when a decision enters or leaves `aperta` / `in analisi`, update the radar in `docs/wiki/index.md`. Fonti in the frontmatter point to where the raw files **will be** after step 4 (`raw/compiled/<feature-name>/…`).

### Coherence check

After writing the pages, run the **Coherence** step of `docs/CLAUDE.md` on what you touched:

1. List the touched pages: `git status --porcelain docs/wiki`.
2. For each, find its neighbourhood: pages linking to it (`grep -rln "<file-name>.md" docs/wiki`), pages it links to, and pages mentioning its entities, rules or decisions (`grep -rn` their names in `docs/wiki`, plus `AGENTS.md`, `CONTRIBUTING.md`, `docs/CLAUDE.md`, `.claude/skills/`).
3. Read only the relevant sections and compare: rules, numbers, names, decision statuses; for `tecnico/` pages also the code and assets they describe and the Unity / plugin docs.
4. Align the wiki pages this feature made stale; record the other mismatches as conflicts (open questions). Do not edit project files here: report them, they are fixed in a feature commit outside the ingest.

### Maintenance

Then handle the maintenance warnings, following step 7 of **Ingest** and **Freshness and consolidation** in `docs/CLAUDE.md`:

1. Keep the warnings on the touched pages and their neighbourhood (the coherence scope), plus at most a few others; list the rest to the user as out of scope.
2. For each one in scope: verify the page for real (code and assets for `tecnico/`, Unity / plugin docs, or the user in the session for business and decisions), consolidate it, or add the missing `## Alternative scartate` with the user.
3. Ask the user before anything structural (merging pages, superseding a decision, dropping content) and wait for the ok.
4. Close each warning handled with the change that turns it off (`verificato`, `## Domande aperte` + `verificato`, `consolidato`), and put the reason of a postponed or skipped consolidation in the commit body. No report files: everything happens in the session.

Discarded alternatives are part of the coherence check: before integrating a source, `grep -rn -A20 "## Alternative scartate" docs/wiki`, and if the source proposes again something ruled out, tell the user why it was discarded and integrate it only on their ok.

## 4. Move the processed raw files

Move the **whole feature folder**, not just its files: afterwards `docs/raw/<feature-name>/` must not exist any more.

```bash
mkdir -p docs/raw/compiled
if [ -d docs/raw/compiled/<feature-name> ]; then
  # a previous ingest of this feature already created it: merge the new files, then drop the folder
  mv docs/raw/<feature-name>/* docs/raw/compiled/<feature-name>/ && rmdir docs/raw/<feature-name>
else
  mv docs/raw/<feature-name> docs/raw/compiled/<feature-name>
fi
```

- Use `mv`, not `git mv` (raw files may still be untracked). Staging with `git add -A` in the next step records them as renames.
- If the folder cannot be removed (on Windows it can be locked by an editor or a terminal whose working directory is inside it), retry once; if it still fails, tell the user to close what holds it and remove the empty folder. Git does not track empty folders, so the commit is correct either way.
- If the user adds notes after a mid-feature ingest, the folder is simply created again; the next ingest merges it as above.

## 5. Lint

Run the `/lint-wiki` skill again and follow it, without its commit: its fixes go into the commit below. It runs after step 4 because it checks that the fonti exist under `raw/compiled/`. Errors it cannot fix go into the report.

## 6. Commit — only wiki and raw

```bash
git add -A docs/wiki docs/raw
git commit -m "docs(wiki): update from <feature-name>" -m "<one line per page created/updated/deleted>" -- docs/wiki docs/raw
```

- The `-- docs/wiki docs/raw` pathspec keeps any other staged change out of this commit.
- End the commit message with the attribution line required by the session.
- Do **not** push: `/close` does it, or the user.

## 7. Report to the user

- Pages created, updated, deleted (with links).
- Lint: errors left, warnings.
- **Maintenance**: warnings handled and how they were closed; warnings left out of scope.
- **Coherence**: pages aligned because this feature made them stale; mismatches recorded as conflicts; mismatches in project files (`AGENTS.md`, `CONTRIBUTING.md`, skills, …) to fix outside the ingest. Say "none found" if so.
- **Conflicts** found and where they were recorded (page and open question).
- Sensitive data found in raw and not copied, if any.
- Raw files moved to `compiled/`.
- The commit hash.
