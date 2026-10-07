---
name: start
description: Start a new task as a git-flow feature - create feature/<name> from an up-to-date develop, create docs/raw/<name>/ with a first note describing the goal, commit and push. Use when the user runs /start.
argument-hint: <feature-name> [goal]
---

# /start — open a feature

Conventions (branch names, commit format) are in `CONTRIBUTING.md`; raw rules are in `docs/CLAUDE.md`.

## Example

```
/start deck-shuffle Mescolare il mazzo a inizio partita e distribuire le carte negli slot
/start study-scoring Capire come si calcola il punteggio di una mano: combinazioni, bonus, pareggi
/start scoring Completare lo studio in wiki/business/punteggio.md e implementare il calcolo del punteggio
```

## 1. Inputs

- **Feature name**: from the arguments. Short, English, kebab-case (`deck-shuffle`). Analysis-only features use the `study-` prefix (`study-scoring`). If missing or not in that form, propose one and ask the user to confirm.
- **Continuing a topic**: a feature that resumes an earlier one keeps its name with the next free number: `study-scoring` → `study-scoring-2` → `study-scoring-3`. If the name given is already taken (check 3 below) and the goal continues that topic, propose the numbered name.
- **Goal**: what the task is about and why. If not in the arguments, ask the user. It is written in **Italian**, in their words; don't invent requirements.
- **Resuming a topic**: if the wiki has a decision on the same topic, e.g. a study left `in analisi` (radar in `docs/wiki/index.md`, or `grep -rl "tipo: decisione" docs/wiki`), mention it to the user and link it in the goal note, so the work starts from there.
- **Open questions on the topic**: find the wiki pages about the topic (root index → area index → pages) and read their `## Domande aperte` / `## Open questions` sections (`grep -rln "## Domande aperte\|## Open questions" docs/wiki` lists the pages that have one). Link those pages in the goal note and list their questions in the report (step 6), so they are answered or kept in mind during the feature.
- **Decisions not taken** (code features only, not `study-`): if the goal touches a decision `aperta` or `in analisi` (radar), warn the user: implementing it takes that decision. During the feature the agent asks for an explicit ok before writing that code, and the ingest will record the decision as `presa` with what was implemented (see **Decisions** in `docs/CLAUDE.md`).

## 2. Checks — stop if any fails

1. **Clean working tree**: `git status --porcelain` must be empty. Otherwise stop and ask the user to commit or stash first.
2. **git-flow initialized**: `git config gitflow.branch.develop` must print `develop`. Otherwise stop and tell the user to run `git flow init -d` (defaults: `main`, `develop`, `feature/`).
3. **Name free**: no local or remote branch `feature/<name>` (`git branch -a --list "*feature/<name>"`), and no `docs/raw/<name>/` or `docs/raw/compiled/<name>/`. If taken, propose the next numbered name (see *Continuing a topic*).

## 2b. Open work — information only, never blocking

Tell the user what is still open, then go on:

```bash
git fetch --prune origin
git branch -vv --list "feature/*"      # local features; "[origin/…: gone]" = merged and deleted on GitHub
git stash list                          # work set aside, maybe meant for this feature
gh pr list --state open                 # open PRs (skip if gh is missing or not logged in)
```

- Local branches whose remote is `gone`: merged; suggest `git branch -D feature/<name>`.
- Other local features and open PRs: work not merged yet; the new feature may conflict with it on the wiki (`/close` handles that).
- Stashes: mention them; one may be meant for this feature (`git stash pop` after step 3).

## 3. Create the feature

```bash
git checkout develop
git pull --ff-only origin develop
git flow feature start <name>
```

## 4. First raw note

Create `docs/raw/<name>/YYYY-MM-DD-obiettivo.md` (today's date):

```md
# Obiettivo — <name>

<the goal, as given by the user>
```

It is the task description and the first source for `/ingest`. It also keeps the folder in git, which does not track empty folders.

## 5. Commit and push

```bash
git add docs/raw/<name>
git commit -m "docs(raw): add goal for <name>" -- docs/raw/<name>
git push -u origin feature/<name>
```

End the commit message with the attribution line required by the session.

## 6. Report

Branch created, path of the goal note, open work found in step 2b, the decisions and open questions on the topic, and a reminder: add notes to `docs/raw/<name>/` whenever needed; `/close` will process them. For a `study-` feature, also remind that it touches no code (only `docs/`, `.claude/`, `*.md`).
