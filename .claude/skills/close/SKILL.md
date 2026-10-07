---
name: close
description: Close the current feature - check raw notes, merge origin/develop, run /ingest to update the wiki, run /lint-wiki, run the repo checks (skipped for study- features, which must touch no code), push and open a pull request toward develop with GitHub CLI. Never merges. Use when the user runs /close. Only on a feature/* branch.
---

# /close — close a feature with a PR

Conventions (commit and PR format) are in `CONTRIBUTING.md`; wiki rules in `docs/CLAUDE.md`.

## Example

```
/close
```

No arguments: it closes the current feature branch.

The feature is **not** merged here, and `git flow feature finish` is **never** used: it would merge into `develop` locally and skip the review. The merge happens on GitHub after the user reviews the PR.

## 1. Checks — stop if any fails

1. **Branch**: `git branch --show-current` starts with `feature/`. Otherwise stop. Feature name = branch without `feature/`.
2. **No uncommitted code**: `git status --porcelain`, ignoring `docs/raw/`, must be empty. Otherwise stop and ask the user to commit or discard it. New raw notes are fine: `/ingest` processes them.
3. **GitHub CLI**: use `gh` if on the PATH, otherwise `"C:/Program Files/GitHub CLI/gh.exe"`. `gh auth status` must succeed; otherwise stop and tell the user to run `gh auth login` in a real terminal.
4. **Study features touch no code**: if the name starts with `study-`, every file changed by the feature must be documentation, i.e. under `docs/`, under `.claude/`, or a `*.md` file:
   ```bash
   git fetch origin
   git diff --name-only origin/develop...HEAD
   ```
   If any other file appears, stop, list them and tell the user: a study cannot change code; move that work to a new feature (`/start <name>`) and revert it here.
5. **Raw untouched**: files that already exist on `develop` under `docs/raw/` are never modified or deleted:
   ```bash
   git diff --name-status --no-renames origin/develop...HEAD -- docs/raw
   ```
   Every line must be `A` (added). An `M` or `D` line: stop, list the files and tell the user to restore them (`git checkout origin/develop -- <file>`) and add a new note instead. The current feature's own notes are never on `develop`, so correcting them is allowed.

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

Then run the `/lint-wiki` skill and follow it (it commits its own fixes, if any). It is quick, and it catches what the ingest did not see: manual wiki edits, or an ingest that had nothing to process. If errors remain that it cannot fix, stop and report them.

## 5. Repo checks

**Skip this step for a `study-` feature**: check 4 already proved it changes no code.

Otherwise, from the repo root, in order, stopping at the first failure and reporting its output:

```bash
dotnet tool restore
dotnet csharpier check Assets/Scripts
```

Then the checks the agent cannot run (Unity compiles only inside the Editor): ask the user to confirm that the project opens in Unity with **no compilation errors in the Console** and that the feature works in Play mode. Wait for the answer; if they report errors, stop.

Also check that every added or renamed asset has its `.meta` committed: `git diff --name-only --diff-filter=A origin/develop...HEAD -- Assets` must list, for every file or folder that is not a `.meta`, its `<path>.meta` too. Report the missing ones and stop.

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
  - *How to verify*: tick the checks that passed (formatting, Unity Console confirmed by the user), describe the manual check in Play mode. For a `study-` feature, replace the checklist with "Study feature: documentation only, code checks skipped" and what to read to review it;
  - end the body with the attribution line required by the session for pull requests.
- Write the body to a temporary file in the scratchpad, not in the repo.

## 8. Report

PR URL, checks passed (or skipped, for a study), wiki pages touched, conflicts to look at in review, and whether the wiki was regenerated after a conflict with `develop`. Remind the user that the merge is theirs, on GitHub, after review; GitHub deletes the remote branch after the merge, the local one goes with `git branch -D feature/<name>`.
