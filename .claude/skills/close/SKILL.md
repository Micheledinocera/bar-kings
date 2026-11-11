---
name: close
description: Close the current feature - check raw notes, merge origin/develop (fixing whitespace-only conflicts), run the repo checks fixing instead of blocking (skipped for study- features, which change only documentation), push, open a pull request toward develop and squash-merge it, then run /ingest <name> to update the wiki in its own feature. Stops before the merge on real conflicts, leaving the PR to the developer with a proposed resolution. Use when the user runs /close. Only on a feature/* branch.
---

# /close — close a feature, merge it, then ingest it

Conventions (commit and PR format) are in `CONTRIBUTING.md`; wiki rules in `docs/CLAUDE.md`.

## Example

```
/close
```

No arguments: it closes the current feature branch.

The feature is merged on GitHub (`gh pr merge --squash`), never with `git flow feature finish`, which would merge locally. The wiki is **not** updated in the feature: after the merge, `/ingest <name>` updates it in a separate `feature/ingest-<name>`, opened and merged at once. So two features never conflict on the wiki.

**Never blocking on rules.** Some contributors are people. Style, formatting, conventions, wiki lint and `.meta` checks never stop the close: the agent fixes what it can (also in work made by people on the feature) and lists the rest in the PR *Notes*. Only the checks in step 1 marked **stop** stop the close; only the cases in step 8 stop the **merge**.

## 1. Checks

1. **Branch** (stop): `git branch --show-current` starts with `feature/`. Otherwise stop. Feature name = branch without `feature/`. A `feature/ingest-*` branch is closed by `/ingest`, not here: stop and say so.
2. **No uncommitted work** (stop): `git status --porcelain`, ignoring `docs/raw/`, must be empty. Otherwise stop and ask the user to commit or discard it. Uncommitted raw notes of this feature are committed now (`docs(raw): …`): the ingest reads them from `develop` after the merge. Files listed as modified while `git diff --quiet -- <path>` succeeds are line-ending noise (Unity rewrites assets with LF at every Play): ignore them.
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

If `docs/raw/<name>/` holds nothing beyond the goal note (or does not exist) **and** this session has a history of work on the feature (discussion, decisions, changes), tell the user: nothing was recorded as raw, maybe something was forgotten. Offer to save a summary of the session as `docs/raw/<name>/YYYY-MM-DD-sessione.md`: decisions, rules, reasons and open points that came up, not the chat itself. Write and commit it only if the user accepts, then continue.

If there is no session history (e.g. `/close` run in a fresh session), skip this step.

## 3. Sync with develop

```bash
git fetch origin
git merge --no-edit origin/develop
```

- **No conflicts**: continue.
- **Conflicts**: sort them with the procedure in *Conflicts* below. All easy: resolve them, commit the merge and continue. Any hard one: `git merge --abort`, remember the conflicted files and the proposed resolution, and continue **without merging develop**: the checks still run, the PR is opened, and step 8 does not merge.

End the merge commit message with the attribution line required by the session.

### Conflicts — easy or hard

A conflict is **easy** when both sides say the same thing and differ only in form: spaces, tabs, indentation, line endings (LF/CRLF), the final newline at end of file, line wrapping or other formatting.

1. Retry ignoring whitespace: `git merge --abort`, then
   ```bash
   git merge --no-edit -X ignore-all-space -X ignore-cr-at-eol origin/develop
   ```
   If it ends without conflicts, they were all whitespace: continue.
2. For each file still conflicted, compare the two sides of every hunk (`git diff`, `git show :2:<path>` and `:3:<path>`). If they are equal once formatting is normalized (whitespace, line breaks, final newline; for `.cs`, the same after `dotnet csharpier format`), take `develop`'s side (`git checkout --theirs -- <path>` for the whole file only when every hunk of it is easy; otherwise edit the hunks) and `git add` it. Step 5 formats the scripts again.
3. Everything else is **hard**: a change of content on both sides, a file deleted on one side and changed on the other, any conflict in a Unity YAML file (`.unity`, `.prefab`, `.asset`, `.meta`, `.mat`) that is not whitespace-only. For each hard file, write a **proposed resolution**: what each side changed and the merged result you would keep (a short description, plus the resolved hunk when it fits in a comment). Then `git merge --abort`.

Conflicts in `docs/wiki/` follow the same rule: features do not edit the wiki (only `/ingest` does), so they are rare.

## 4. Wiki edited by hand

Features do not change `docs/wiki/`: the ingest does, after the merge. If `git diff --name-only origin/develop...HEAD -- docs/wiki` lists files anyway (a manual edit), run the `/lint-wiki` skill and follow it (it commits its own fixes), and mention the manual edit in the PR *Notes*.

## 5. Repo checks — fix, then report

**Skip this step for a `study-` feature** whose check 4 found no code: no formatting, no Unity checks and **no build**.

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
4. **Unity** (through `unity-mcp`, with the Editor open on the project): first save everything open in the Editor (`EditorSceneManager.SaveOpenScenes()` and `AssetDatabase.SaveAssets()` in a `Unity_RunCommand`) and run `git status --porcelain`. A scene marked modified with no real change is rewritten identical and shows nothing; files that do change are work left unsaved: show them to the user, who decides whether they belong to this feature (commit them) or not (revert them). Then read the Console (`Unity_GetConsoleLogs`) after Unity has recompiled the merged code: **no compilation errors**, no new errors or warnings. If `unity-mcp` is not connected, ask the user to open the project and retry; if it is still unavailable, ask the user about the Console instead. Then ask the user whether the feature works in Play mode. Problems go in the PR *Notes* and the checklist stays unticked.
5. **Build**: with the Editor open, run the menu item `Tools/Build Windows` through `unity-mcp` (`EditorApplication.ExecuteMenuItem("Tools/Build Windows")`, in a `Unity_RunCommand`; it builds the scenes enabled in the Build Settings into `Builds/Windows/`, git-ignored). The server may not respond for a few seconds after a recompile: retry. Then read the result with `BuildReport.GetLatestReport().summary` (result, `totalSize`, `totalErrors`, `totalWarnings`), or the `Windows build …` line in the Console. Check:
   - result `Succeeded`, no errors and no warnings;
   - size close to the last one in [scenes](../../../docs/wiki/tecnico/scenes.md#build): a jump of several MB is usually a texture imported uncompressed (sides not a multiple of 4);
   - `git status --porcelain` unchanged by the build: a settings file rewritten by the build (URP asset, `ProjectSettings/*.asset`) is a new value Unity computes; look at the diff, and commit it in a `build:` commit if it only aligns Unity's own data, otherwise list it in the PR *Notes*.

   If `unity-mcp` is unavailable, ask the user to run `Tools/Build Windows` and report the Console line. Problems go in the PR *Notes*.

## 6. Push

```bash
git push -u origin feature/<name>
```

## 7. Pull request

If a PR already exists for the branch (`gh pr view --json url,number`), keep it: the push updated it.

Otherwise create it toward `develop`:

```bash
gh pr create --base develop --head feature/<name> --title "<title>" --body-file <file>
```

- **Title**: Conventional Commits format, English, summarizing the feature (e.g. `feat: shuffle and deal the deck at game start`).
- **Body**: fill `.github/pull_request_template.md` in English:
  - *Summary* from the goal note in `docs/raw/<name>/` and the commits;
  - *Changes*: one line per commit or group of commits (`git log --no-merges origin/develop..HEAD`);
  - *Wiki*: "Updated after the merge by `feature/ingest-<name>`";
  - *How to verify*: tick the checks that passed (formatting, Unity Console confirmed by the user, Windows build with its size), describe the manual check in Play mode. For a `study-` feature without code, replace the checklist with "Study feature: documentation only, code checks skipped" and what to read to review it;
  - *Notes*: everything steps 1, 3, 4 and 5 left open (conflicts resolved as easy, unfixed lint errors, missing `.meta`, Console errors, rule issues in the code);
  - end the body with the attribution line required by the session for pull requests.
- Write the body to a temporary file in the scratchpad, not in the repo.

## 8. Merge

**Do not merge** when:

- step 3 found a **hard conflict**: post the proposed resolution as a PR comment (`gh pr comment <number> --body-file <file>`, one section per file: what each side changed, the merged result proposed) and stop. The developer resolves and merges by hand; afterwards they run `/ingest <name>`.
- the **code does not work**: compilation errors in the Console, a failed build, or the user said the feature does not work in Play mode. Tell the user and ask: fix now (then run `/close` again) or merge anyway.

Otherwise squash-merge it (GitHub deletes the remote branch: auto-delete is on), then clean up locally:

```bash
gh pr merge <number> --squash
git checkout develop
git pull --ff-only origin develop
git branch -D feature/<name>
```

If GitHub refuses the merge because `develop` moved meanwhile and the PR now conflicts, go back to step 3 once (merge `origin/develop`, sort the conflicts, push), then retry.

## 9. Ingest

Right after the merge, run the `/ingest` skill with `<name>` and follow it fully: it opens `feature/ingest-<name>`, updates the wiki from the feature's raw notes and code, and merges its own PR.

## 10. Report

PR URL and whether it was merged; checks passed, fixed by the agent or left in the PR *Notes* (or skipped, for a study); conflicts resolved as easy, and for hard ones the PR comment with the proposal and the reminder to run `/ingest <name>` after the manual merge; the report of `/ingest` (ingest PR, wiki pages touched, open questions).
