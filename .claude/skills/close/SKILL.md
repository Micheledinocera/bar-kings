---
name: close
description: Close the current feature - check raw notes, merge origin/develop, run /ingest to update the wiki, run /lint-wiki, run the repo checks, fixing instead of blocking (skipped for study- features, which change only documentation), push and open a pull request toward develop with GitHub CLI. Never merges. Use when the user runs /close. Only on a feature/* branch.
---

# /close — close a feature with a PR

Conventions (commit and PR format) are in `CONTRIBUTING.md`; wiki rules in `docs/CLAUDE.md`.

## Example

```
/close
```

No arguments: it closes the current feature branch.

The feature is **not** merged here, and `git flow feature finish` is **never** used: it would merge into `develop` locally and skip the review. The merge happens on GitHub after the user reviews the PR.

**Never blocking on rules.** Some contributors are people. Style, formatting, conventions, wiki lint and `.meta` checks never stop the close: the agent fixes what it can (also in work made by people on the feature) and lists the rest in the PR *Notes*. Only the checks in step 1 marked **stop** stop, because without them the PR itself would be wrong.

## 1. Checks

1. **Branch** (stop): `git branch --show-current` starts with `feature/`. Otherwise stop. Feature name = branch without `feature/`.
2. **No uncommitted work** (stop): `git status --porcelain`, ignoring `docs/raw/`, must be empty. Otherwise stop and ask the user to commit or discard it. New raw notes are fine: `/ingest` processes them. Files listed as modified while `git diff --quiet -- <path>` succeeds are line-ending noise (Unity rewrites assets with LF at every Play): ignore them.
3. **GitHub CLI** (stop): use `gh` if on the PATH, otherwise `"C:/Program Files/GitHub CLI/gh.exe"`. `gh auth status` must succeed; otherwise stop and tell the user to run `gh auth login` in a real terminal.
4. **Study features touch no code** (ask): if the name starts with `study-`, every file changed by the feature should be documentation: any `*.md` file anywhere (wiki, raw, `AGENTS.md`, `CONTRIBUTING.md`, skills, templates), or anything under `docs/` or `.claude/`:
   ```bash
   git fetch origin
   git diff --name-only origin/develop...HEAD
   ```
   If any other file appears, list them and ask the user: move that work to a new feature (`/start <name>`) and revert it here, or keep it in this PR. If kept, the code checks of step 5 run for this feature and the PR *Notes* say that the study contains code.
5. **Raw untouched** (warn): files that already exist on `develop` under `docs/raw/` should never be modified or deleted:
   ```bash
   git diff --name-status --no-renames origin/develop...HEAD -- docs/raw
   ```
   Every line should be `A` (added). For an `M` or `D` line, list the files and offer to restore them (`git checkout origin/develop -- <file>`) and to put the change in a new note instead; if the user declines, mention it in the PR *Notes*. The current feature's own notes are never on `develop`, so correcting them is allowed.

## 2. Raw notes — a reminder, not a check

If `docs/raw/<name>/` holds nothing beyond the goal note (or does not exist) **and** this session has a history of work on the feature (discussion, decisions, changes), tell the user: nothing was recorded as raw, maybe something was forgotten. Offer to save a summary of the session as `docs/raw/<name>/YYYY-MM-DD-sessione.md`: decisions, rules, reasons and open points that came up, not the chat itself. Write it only if the user accepts, then continue.

If there is no session history (e.g. `/close` run in a fresh session), skip this step.

## 3. Sync with develop

Bring `develop` in before the ingest, so the ingest works on the current wiki:

```bash
git fetch origin
git merge --no-edit origin/develop
```

- No conflicts: continue.
- Conflicts **only in `docs/wiki/`**: the wiki is regenerated, not fixed by hand. Take `develop`'s wiki as it is, finish the merge, then run the ingest in **re-ingest mode** (step 4):
  ```bash
  git rm -rq --cached docs/wiki && rm -rf docs/wiki
  git checkout origin/develop -- docs/wiki
  git commit --no-edit
  ```
- Conflicts anywhere else: `git merge --abort`, stop, list the files and ask the user to resolve them.

End the merge commit message with the attribution line required by the session.

## 4. Update the wiki

Run the `/ingest` skill and follow it fully (in re-ingest mode if step 3 took `develop`'s wiki). Afterwards `docs/raw/<name>/` must not exist (everything is in `docs/raw/compiled/<name>/`). If it still has files, stop and report them; if it is only an empty folder, remove it.

If `/ingest` found nothing to process, continue: the PR simply has no wiki commit.

Then run the `/lint-wiki` skill and follow it (it commits its own fixes, if any). It is quick, and it catches what the ingest did not see: manual wiki edits, or an ingest that had nothing to process. Errors it cannot fix go in the PR *Notes*; continue.

## 5. Repo checks — fix, then report

**Skip this step for a `study-` feature** whose check 4 found no code.

None of these checks stops the close. From the repo root:

1. **Formatting**: format instead of checking, and commit the result if anything changed:
   ```bash
   dotnet tool restore
   dotnet csharpier format Assets/Scripts
   git status --porcelain Assets/Scripts
   ```
   If files changed: `git commit -m "style: format scripts with CSharpier" -- Assets/Scripts` (with the session's attribution line). If `dotnet` is missing or the tool fails, skip it and write that in the PR *Notes*.
2. **`.meta` files**: every added asset needs its `.meta` committed: `git diff --name-only --diff-filter=A origin/develop...HEAD -- Assets` should list, for every file or folder that is not a `.meta`, its `<path>.meta` too. If some are missing and exist on disk, commit them; if they don't exist, ask the user to open the project in Unity (which creates them) and list them in the PR *Notes* if they are still missing.
3. **Code made by people**: read the scripts the feature changed (`git diff --name-only origin/develop...HEAD -- Assets/Scripts`) against the rules in `AGENTS.md`. Fix small things in a `refactor:` or `fix:` commit when the fix is safe and doesn't change behaviour; list the rest in the PR *Notes*. Never rename identifiers chosen by people, and never ask them to redo work.
4. **Unity** (through `unity-mcp`, with the Editor open on the project): read the Console (`Unity_GetConsoleLogs`) after Unity has recompiled the merged code: **no compilation errors**, no new errors or warnings. If `unity-mcp` is not connected, ask the user to open the project and retry; if it is still unavailable, ask the user about the Console instead. Then ask the user whether the feature works in Play mode. Problems go in the PR *Notes* and the checklist stays unticked; the user decides whether to fix them now or after.
5. **Build**: with the Editor open, run the menu item `Tools/Build Windows` through `unity-mcp` (`EditorApplication.ExecuteMenuItem("Tools/Build Windows")`, in a `Unity_RunCommand`; it builds the scenes enabled in the Build Settings into `Builds/Windows/`, git-ignored). The server may not respond for a few seconds after a recompile: retry. Then read the result with `BuildReport.GetLatestReport().summary` (result, `totalSize`, `totalErrors`, `totalWarnings`), or the `Windows build …` line in the Console. Check:
   - result `Succeeded`, no errors and no warnings;
   - size close to the last one in [scenes](../../../docs/wiki/tecnico/scenes.md#build): a jump of several MB is usually a texture imported uncompressed (sides not a multiple of 4);
   - `git status --porcelain` unchanged by the build: a settings file rewritten by the build (URP asset, `ProjectSettings/*.asset`) is a new value Unity computes; look at the diff, and commit it in a `build:` commit if it only aligns Unity's own data, otherwise list it in the PR *Notes*.

   If `unity-mcp` is unavailable, ask the user to run `Tools/Build Windows` and report the Console line. Problems go in the PR *Notes*; the build never stops the close.

## 6. Push

```bash
git push -u origin feature/<name>
```

## 7. Pull request

If a PR already exists for the branch (`gh pr view --json url`), just report its URL: the push updated it.

Otherwise create it toward `develop`:

```bash
gh pr create --base develop --head feature/<name> --title "<title>" --body-file <file>
```

- **Title**: Conventional Commits format, English, summarizing the feature (e.g. `feat: shuffle and deal the deck at game start`).
- **Body**: fill `.github/pull_request_template.md` in English:
  - *Summary* from the goal note in `docs/raw/compiled/<name>/` and the commits;
  - *Changes*: one line per commit or group of commits (`git log --no-merges origin/develop..HEAD`);
  - *Wiki*: pages from the `/ingest` report and its conflicts, or "None";
  - *How to verify*: tick the checks that passed (formatting, Unity Console confirmed by the user, Windows build with its size), describe the manual check in Play mode. For a `study-` feature without code, replace the checklist with "Study feature: documentation only, code checks skipped" and what to read to review it;
  - *Notes*: everything steps 1, 4 and 5 left open (unfixed lint errors, missing `.meta`, Console errors, rule issues in the code);
  - end the body with the attribution line required by the session for pull requests.
- Write the body to a temporary file in the scratchpad, not in the repo.

## 8. Report

PR URL, checks passed, fixed by the agent or left in the PR *Notes* (or skipped, for a study), wiki pages touched, conflicts to look at in review, and whether the wiki was regenerated after a conflict with `develop`. Remind the user that the merge is theirs, on GitHub, after review; GitHub deletes the remote branch after the merge (auto-delete is on); the next `/start` deletes the local one, or by hand `git branch -D feature/<name>`.
