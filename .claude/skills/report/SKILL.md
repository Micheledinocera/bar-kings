---
name: report
description: Write a report, analysis or study for people (calls, meetings, playtesters, publishers) as a dated file in docs/output/, from the project wiki and the current feature's raw notes. The folder is not versioned: nothing is committed. Use when the user runs /report.
argument-hint: <topic> [what the report should focus on]
---

# /report — create a report in docs/output

Rules for outputs are in the **Outputs** section of `docs/CLAUDE.md`. Read it first.

## Example

```
/report scoring
/report scoring opzioni per il calcolo del punteggio, da portare al prossimo playtest
```

## 1. Inputs

1. **Feature**: if `git branch --show-current` starts with `feature/`, its notes are a source (step 2). Any branch works.
2. **Topic**: from the arguments, English kebab-case (`scoring`). If missing, ask. Any extra text is the focus of the report: what the user wants to bring to the call.
3. **File name**: `docs/output/<today YYYY-MM-DD>-<topic>.md`. If it already exists, ask whether to add a suffix (`-2`) or pick another topic. Never overwrite an existing output.

## 2. Sources

- `docs/wiki/index.md`, then the pages about the topic (especially the decision on the topic, if it exists, with its `stato`; the root radar lists the open ones).
- On a feature branch: the current feature's raw notes in `docs/raw/<feature-name>/` (not yet processed). Elsewhere: the wiki only.
- For technical topics, the Unity / plugin docs: cite them, don't copy.
- Never use other files in `docs/output/` as a source: outputs are temporary documents, not knowledge.

If the sources say nothing useful about the topic, tell the user and stop instead of writing an empty report.

## 3. Write the report

In **Italian**, for people who did not follow the work. Suggested structure, adapt to the topic:

```md
# <Titolo>

> **Data:** YYYY-MM-DD · **Feature:** <feature-name, or "nessuna"> · **Fonti:** <wiki pages and raw notes used>

## Contesto
Why we are looking at this.

## Cosa sappiamo
Facts and constraints, with their source.

## Opzioni
For each option: what it is, pros, cons, open points.

## Proposta
Only if the sources support one; otherwise say what is missing to decide.

## Domande aperte
What needs an answer (from the call, a playtest, a prototype).
```

Write only what the sources support. Mark assumptions as such.

## 4. Report to the user

Path of the file, sources used, open questions listed in it. Remind them that:

- the file is **not versioned** (`docs/output/` is git-ignored): they share it as they like and delete it by hand when it is no longer needed;
- what comes out of the call, if worth keeping, goes back as a raw note in a feature (`docs/raw/<feature-name>/`).
