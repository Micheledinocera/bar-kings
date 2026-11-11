---
name: ingest
description: Update the project wiki (docs/wiki) from a merged feature - open feature/ingest-<name> from develop, process the feature's raw notes and code, move the raw files to docs/raw/compiled, run /lint-wiki, commit, then open and squash-merge the PR at once with no further checks. Use when /close runs it after the merge, or when the user runs /ingest <name> after merging a PR by hand.
---

# /ingest — update the wiki after a feature is merged

The rules for the wiki (what goes where, language, page format, conflicts) are in `docs/CLAUDE.md`. Read it first and follow its **Ingest** section. This skill adds the operational steps around it.

## Example

```
/ingest deck-shuffle
```

Argument: the name of a feature **already merged** into `develop`. `/close` runs it right after the merge; the user runs it by hand after merging a PR themselves (e.g. after hard conflicts). Without an argument, list the features waiting for an ingest (folders in `docs/raw/` on `develop` other than `compiled/`) and ask which one.

The ingest never runs inside the feature it documents: it has its own branch, `feature/ingest-<name>`, opened, merged and deleted in one go. It is short-lived and touches only `docs/wiki/` and `docs/raw/`, so it skips the checks of `/close`.

## 1. Checks — stop if any fails

1. **Clean working tree**: `git status --porcelain` empty (ignore line-ending noise: files for which `git diff --quiet -- <path>` succeeds). Otherwise stop and ask the user to commit or stash.
2. **GitHub CLI**: `gh auth status` succeeds (`gh` on the PATH, or `"C:/Program Files/GitHub CLI/gh.exe"`).
3. **Feature merged**: find its PR and the commit it became on `develop`:
   ```bash
   gh pr list --state merged --head feature/<name> --json number,title,mergeCommit
   ```
   No merged PR: stop. If the PR is still open, the user merges it first (or runs `/close`).

## 2. Open the ingest feature

```bash
git checkout develop
git pull --ff-only origin develop
git flow feature start ingest-<name>
```

If `feature/ingest-<name>` already exists (an earlier run stopped halfway), check it out and `git merge --no-edit origin/develop` instead.

## 3. Collect what to process

- Raw sources: every file in `docs/raw/<name>/`; never anything under `docs/raw/compiled/` and never `docs/output/` (not versioned, not a source: see **Outputs** in `docs/CLAUDE.md`).
- Custom code of the feature: the commit it was squashed into on `develop` (`mergeCommit` from step 1):
  ```bash
  git show --stat <mergeCommit>
  ```
  If the PR was merged with a merge commit instead of a squash, the feature's commits are `git log --no-merges --stat <mergeCommit>^1..<mergeCommit>^2`. Ignore the files under `docs/raw/`.
- Maintenance warnings: run `node .claude/skills/lint-wiki/lint-wiki.mjs` and keep its `stale`, `code changed`, `consolidate` and `missing "## Alternative scartate"` warnings for the **Maintenance** step below.

If there are no raw sources and no code beyond raw notes: tell the user there is nothing to process, delete the branch (`git checkout develop && git branch -D feature/ingest-<name>`) and stop (show the maintenance warnings, if any: they stay on until a feature touches that part of the wiki).

## 3. Update the wiki

Follow the **Ingest** section of `docs/CLAUDE.md` (read `wiki/index.md` and the involved pages, link the Unity / plugin docs for technical pages, update or create pages, handle conflicts, update `index.md`).

Write for the agent, following the **Purpose** section of `docs/CLAUDE.md`: every page you add has a one-line description in its folder index; one `##` per entity named as in the code; code cited by existing repo-relative paths; consumers/callers updated. For code features, also update `wiki/tecnico/known-issues.md`: remove the items the feature fixed, add new ones to the right block with tag, path and link, and keep the block table counts in sync.

Decisions follow the **Decisions** section of `docs/CLAUDE.md`: every decision page has a `stato` in its frontmatter (`aperta`, `in analisi`, `presa`, `superata`). A study feature usually leaves its decision `in analisi`; a later feature on the same topic updates that page, it does not create a new one. A code feature that implemented a rule of a decision still `aperta` or `in analisi` leaves its status unchanged and records the implementation as the option being tried; it sets it to `presa` only if the user said explicitly during the feature that the rule is fixed (POC phase, see **Decisions** in `docs/CLAUDE.md`).

Pages live in the **area** of their topic, with the frontmatter and indexes described in the **Wiki pages** section of `docs/CLAUDE.md`. When you add, move or remove a page, update its folder's `index.md`; when a decision enters or leaves `aperta` / `in analisi`, update the radar in `docs/wiki/index.md`. Fonti in the frontmatter point to where the raw files **will be** after step 4 (`raw/compiled/<name>/…`).

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

Move the **whole feature folder**: afterwards `docs/raw/<name>/` must not exist any more.

```bash
mkdir -p docs/raw/compiled
if [ -d docs/raw/compiled/<name> ]; then
  # an older ingest of this feature already created it: merge the new files, then drop the folder
  git mv docs/raw/<name>/* docs/raw/compiled/<name>/ && rmdir docs/raw/<name>
else
  git mv docs/raw/<name> docs/raw/compiled/<name>
fi
```

If the folder cannot be removed (on Windows it can be locked by an editor or a terminal whose working directory is inside it), retry once; if it still fails, tell the user to close what holds it and remove the empty folder. Git does not track empty folders, so the commit is correct either way.

## 5. Lint

Run the `/lint-wiki` skill again and follow it, without its commit: its fixes go into the commit below. It runs after step 4 because it checks that the fonti exist under `raw/compiled/`. Errors it cannot fix go into the report.

## 6. Commit — only wiki and raw

```bash
git add -A docs/wiki docs/raw
git commit -m "docs(wiki): update from <name>" -m "<one line per page created/updated/deleted>" -- docs/wiki docs/raw
```

End the commit message with the attribution line required by the session.

## 7. Push, PR and merge — no checks

```bash
git push -u origin feature/ingest-<name>
gh pr create --base develop --head feature/ingest-<name> --title "docs(wiki): update from <name>" --body-file <file>
gh pr merge <number> --squash
git checkout develop
git pull --ff-only origin develop
git branch -D feature/ingest-<name>
```

- **Body** (in the scratchpad, English, not the full template): the feature and its PR (`#<number>`), the pages created, updated and deleted, the conflicts recorded as open questions, the coherence mismatches in project files to fix in a later feature; end with the attribution line required by the session for pull requests.
- No repo checks: the branch touches only the wiki and raw, and lives a few minutes.
- If GitHub refuses the merge because `develop` moved and the PR conflicts: `git fetch origin && git merge --no-edit origin/develop` and sort the conflicts as in *Conflicts* of `/close` (easy: resolve, push, merge; hard: abort, comment the proposed resolution on the PR, stop and leave the merge to the developer).

## 8. Report to the user

- Ingest PR URL and whether it was merged.
- Pages created, updated, deleted (with links).
- Lint: errors left, warnings.
- **Maintenance**: warnings handled and how they were closed; warnings left out of scope.
- **Coherence**: pages aligned because this feature made them stale; mismatches recorded as conflicts; mismatches in project files (`AGENTS.md`, `CONTRIBUTING.md`, skills, …) to fix in a later feature. Say "none found" if so.
- **Conflicts** found and where they were recorded (page and open question).
- Sensitive data found in raw and not copied, if any.
- Raw files moved to `compiled/`.
