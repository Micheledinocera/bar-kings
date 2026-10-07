# Docs & Wiki Guide

This guide covers work in `docs/`: the raw sources, the project wiki, the outputs and the rules to keep them in order. Read it before reading from, writing to or processing anything here. The design rationale is in the raw sources of the `llm-wiki` feature (`docs/raw/compiled/llm-wiki/` once processed).

## Purpose: an index for the agent

The wiki's **first reader is the coding agent** working on this repo; people read it too, but every choice is made so that the agent finds the right information with few reads and trusts it. In practice:

- **Progressive disclosure**: root `index.md` → area `index.md` → page → section. Every index entry has a one-line description precise enough to decide whether to open it without opening it.
- **Few hops**: any fact is reachable from the root in at most four reads; pages that serve a task (a game mechanic, a scene, a group of scripts) point straight to the details.
- **Addressable sections**: one `##` heading per entity, named exactly as in the code (class, MonoBehaviour, ScriptableObject, scene, prefab), so `grep "## <Name>"` and `page.md#anchor` links find it. Long pages start with an index table.
- **Verifiable references**: code and assets are cited by repo-relative path (`Assets/Scripts/...`, `Assets/Scenes/...`, `Assets/Prefab/...`) that exists; the lint checks it. No line numbers, they go stale.
- **Impact first**: for code, say what uses it (consumers, callers) and what is not obvious (traps, bugs), not what the code already says.
- **Lookup over narrative**: tables and short bullets, no history, no prose that repeats the code.
- **Current state only**: what is fixed or removed leaves the wiki in the same feature. The one kind of history kept is what not to propose again: a decision's `## Alternative scartate` (see **Decisions**).

The lint checks the mechanical side of these rules; the ingest applies the rest.

### Granularity and size

What costs the agent is the number of files it opens and the tokens it reads. A section is found with one search (`## <Name>`) whether it lives in a big page or a small one; what makes a page expensive is reading it whole. So:

| Unit | Rule | Lint |
|---|---|---|
| **Page** | One topic; for code, one page per group that matches the code layout (a folder of `Assets/Scripts/`, a scene, a system). Many small entities go in **one page, one `##` section each**, not one file each | — |
| **Page size** | Over 200 lines: starts with an index table of its sections. Target under ~30 KB (~7.5k tokens); over 40 KB or 800 lines: split along the code layout | warn |
| **Section** | One entity per `##` section, up to ~60 lines. A bigger entity (complex manager, system, mechanic) gets **its own page**, and its section in the group page shrinks to a summary and a link | warn over 80 lines |
| **Tiny page** | A page with under ~10 lines of body is merged into its parent or sibling page: every file costs an open, a frontmatter and an index entry. Decisions are exempt (one page each, listed by the radar) | warn |
| **Folder** | Up to ~12 pages; then sub-folders by topic (see **Sub-folders**) | warn |
| **Index** | Up to ~50 entries in a flat list; beyond that, grouped under headings | — |
| **Depth** | Root → area → (sub-folder) → page: any fact within four reads from `wiki/index.md` | — |


## Structure

```
docs/
├── raw/                    # raw sources — added, never edited
│   ├── <feature-name>/     # one folder per git feature (feature/<name>)
│   └── compiled/           # already processed — history, never processed again
├── output/                 # documents for people outside the project (/report) — not versioned, ignored by the agent
└── wiki/                   # compiled knowledge — written by the LLM, reviewed in PRs
    ├── index.md            # the areas + radar of the decisions still to take — always start here
    ├── business/           # the game as a product: concept, game rules, audience, market and competitors, monetisation, glossary
    ├── progetto/           # how the project is built and run: repo, Unity version, packages, git-flow, wiki, studies
    └── tecnico/            # project code only (English): scripts, scenes, prefabs, data assets
```

Other areas (e.g. `arte/` for art direction, UI and audio) are added when the first page needs them, not ahead of time.

The wiki is split by **area** (topic), not by kind of page: an area holds both the pages that describe something and the decisions about it. Every folder has its own `index.md`.

## What goes where

- **Wiki**: what exists in this project and why. Game rules, product choices, decisions, and the documentation of the **project's** code and content: scripts (classes, MonoBehaviours, ScriptableObjects), scenes and what they contain, prefabs, data assets (e.g. the card instances).
- **Not in the wiki**:
  - how to write code → `AGENTS.md`;
  - Unity and third-party behaviour (Unity API, Odin Inspector, DOTween, UniTask, TextMeshPro) → their official docs. Cite them, never copy them;
  - anything under `Assets/Plugins/` or `Packages/` beyond which library is used and why;
  - secrets, credentials, personal data.

## Language

- `wiki/tecnico/`, code and commit messages: **English**.
- Everything else (the other wiki areas, the `index.md` files outside `tecnico/`, raw notes, `output/`): **Italian**. Frontmatter keys and values are always the Italian ones below, in `tecnico/` too.

## Raw sources

- Raw files are **never edited once processed** (`raw/compiled/`). If something changes, add a new file. The current feature's notes in `raw/<feature-name>/` may still be corrected until the ingest moves them. `/close` stops if the feature modifies or deletes raw files that already exist on `develop`.
- File names: `YYYY-MM-DD-topic.md`. Free format inside.
- A feature's sources live in `raw/<feature-name>/`; its first note is the task goal. Tasks are features. Raw notes exist **only inside a feature**: no loose files directly in `raw/`.
- A study resumed later keeps its name with a number: `study-scoring` → `study-scoring-2` → `study-scoring-3`.
- Not everything said in chat becomes a note: the user says what to record. `/close` only warns when a feature has no notes beyond its goal, and offers to save a summary of the session.
- **Study features** (analysis only) are named `study-<topic>` (e.g. `feature/study-scoring`). They **touch no code**: only `docs/`, `.claude/` and `*.md` files; `/close` checks it and skips the code checks. To implement what a study decided, close it and start a new feature. Keep features short: close a study even when the analysis is partial, and resume it later with a new feature.
- There is **no backlog**: a task exists only as a feature (its goal note) and its PR. Knowledge that never reaches the wiki through an ingest was not worth keeping.
- `raw/` is versioned: no secrets or personal data. If you find any, report them and do not copy them into the wiki.
- Never read `raw/compiled/` when processing; `output/` is never read at all (see **Outputs**) (one exception: re-ingesting the current feature after a wiki conflict, see **Ingest**). `raw/compiled/` is history, read it only to trace where a page came from.

## Wiki pages

- One topic per page, in the area of its topic.
- Every page starts with a frontmatter:
  ```yaml
  ---
  tipo: decisione            # info | decisione
  stato: in analisi          # only for decisione — see Decisions
  aggiornato: 2026-10-02     # last ingest that changed the page
  verificato: 2026-10-02     # last check of the WHOLE page against its sources of truth
  consolidato: 2026-10-02    # optional: last consolidation (or decision not to consolidate)
  fonti:                     # raw files it comes from, relative to docs/
    - raw/compiled/study-scoring/2026-10-07-obiettivo.md
  ---
  ```
  `fonti` may be empty only in `tecnico/` (pages that come from code). A `superata` decision also has `sostituita_da: <relative path of the page that replaces it>`. `verificato` and `consolidato` drive the maintenance warnings (see **Freshness and consolidation**): a new page gets `verificato` equal to `aggiornato`; an ingest that changes only part of a page updates `aggiornato` but not `verificato`, so `verificato` may lag behind.
- **Indexes** (progressive disclosure: read the root, then one area, then the pages):
  - `wiki/index.md` lists the areas, one line each, and the **radar**: every decision `aperta` or `in analisi`, with its status;
  - every folder has an `index.md` listing its pages (`- [Title](./page.md): one-line description`, plus ` · <stato>` for decisions) and its sub-folders' indexes. A folder with no pages says so ("Nessuna pagina." / "No pages.").
- **Sub-folders**: split an area when it passes ~12 pages, or when a sub-topic reaches 4 pages (e.g. `business/carte/`). At most one level below an area. In `tecnico/` sub-folders follow `Assets/Scripts/` (e.g. `tecnico/shared/`, `tecnico/panchina/`), plus `tecnico/scene/` for scenes and prefabs when they need more than one page. Don't split ahead of time.
- **Lint**: `/lint-wiki` checks links and anchors, indexes and their descriptions, frontmatter, radar, code and asset paths and page size, and warns on pages to verify or consolidate. `/ingest` runs it at its start (for the maintenance warnings) and before its commit, `/close` after the ingest.
- Unknowns go in a final `## Domande aperte` (or `## Open questions` in `tecnico/`) section. Never guess.
- **Page formats in `tecnico/`**: a page per folder of `Assets/Scripts/` (or per system, when a system spans folders) with one `## <ClassName>` section each: what it does, where it lives (path), the scenes and prefabs it is attached to, the serialized fields that matter, what it calls and who uses it, traps. A page per scene: purpose, main GameObjects and the scripts on them, how it is entered and left. The first pages of each kind set the format for the next ones; new kinds of page follow the **Purpose** rules above.
- **Known issues**: `wiki/tecnico/known-issues.md` (one page for the whole project), grouped in **blocks**, one block per area of code meant to be fixed in one code feature, with a summary table (block, items, tags, decision it waits for). Every item has a tag (`bug`, `ux`, `performance`, `dead-code`, `duplicate`, `debt`, `to-verify`, `decision`), the path of the file and a link to the detailed section. Detailed pages keep their `## Open questions`; `known-issues.md` is the index to plan the fixes.

### Freshness and consolidation

Pages decay with the **age of their last verification** and their kind of content, **never** with how often they are read: a page read rarely can be critical. The lint script warns; the ingest handles the warnings (see **Ingest**, step 7). Thresholds can be revised: if they flag far too many pages, or none for months, propose a change with its reason.

| Page | Stale when | Why |
|---|---|---|
| decisione `aperta` | `verificato` older than 30 days | An open decision left a month is a blocker: take it up before a code feature lands on it |
| decisione `in analisi` | older than 30 days | A stalled analysis loses value: the data it collected ages |
| decisione `presa` | older than 180 days | It decays when something changes, and features change things; time is only a safety net |
| decisione `superata` | never | History: nothing to verify |
| `info` outside `tecnico/` | older than 90 days | Market, competitors and company facts move on a quarterly scale |
| `tecnico/` | a cited file has a commit after `verificato`; safety net 180 days | The code is the truth: a change to what the page cites is a precise signal, days are not |

**Consolidation**: a page gets a warning when **4 or more features** appear in its `fonti` with raw files dated after `consolidato` (all of them when `consolidato` is missing). Four in-place updates from different sources is where a short page starts to read as patched together. Length is not a trigger here: it is handled by **Granularity and size** (in `tecnico/` a long page is split, not summarised). Consolidating means rewriting the page from scratch as one coherent summary of what it holds, not adding a summary on top: compare the rewrite with the previous version (git) so that no valid information is lost; what is dropped must be superseded or redundant; discarded options go to `## Alternative scartate`. Then set `consolidato` and `verificato` to today.

**Leaving the wiki** (outside `tecnico/`, no content is ever simply dropped): a decision replaced by another becomes `superata` with `sostituita_da`; an info page merged into another moves its content there and every link is updated (the lint checks them). No archive folder and no stub pages. In `tecnico/` pages of removed code are deleted as before: the code is the truth, git is the history.

### Decisions

A decision is a page with `tipo: decisione`, in the area of its topic. Its `stato` is one of:

| Status | Meaning |
|---|---|
| `aperta` | To be taken, not analysed yet |
| `in analisi` | Analysis started, not concluded (e.g. a closed study feature) |
| `presa` | Taken, with the reasons |
| `superata` | Replaced by another decision (`sostituita_da`) |

The status lives only in the frontmatter, never in the folder name: finding every open decision is a `grep "stato: aperta"`, and the root radar lists them.

A partial analysis is a decision `in analisi`: what is known, options considered (with pros and cons), open questions. When a later feature completes it, the ingest updates the same page instead of creating a new one.

**Alternative scartate.** A decision `presa` or `superata` ends with a `## Alternative scartate` section (before `## Domande aperte`), so that nobody proposes again what was already ruled out. One line per alternative:

```md
- <alternative, in short> — scartata il YYYY-MM-DD: <why; say it plainly if it did not work or was wrong> (fonte: raw/compiled/<feature>/<file>.md)
```

If no alternative was considered, the section says "Nessuna alternativa considerata.". Decisions `aperta` and `in analisi` keep their candidates under `## Opzioni` and may already list ruled-out ones here. Only discarded facts or choices go here, never rewordings. Finding every discarded alternative is `grep -rn -A20 "## Alternative scartate" docs/wiki`: there is no separate index page.

**Implementing a decision not taken.** Code that implements a game rule whose decision is `aperta` or `in analisi` takes that decision. Before writing such code the agent stops, says which decision is involved, waits for an explicit ok, and warns that at the ingest the wiki will record the decision as `presa`, with what was implemented as the choice. `/start` flags these decisions when it opens a code feature.

## Outputs

`docs/output/` holds documents for people outside the project (calls, meetings, playtesters, publishers): reports, analyses, studies, emails. They are temporary by nature.

- **Not versioned**: the folder is in `docs/.gitignore`. Outputs are never committed, on a feature or elsewhere.
- **Ignored by the agent**: never read as a source (ingest, `/report`, lint), never checked by `/close`, never cited by the wiki. Only `/report` writes there.
- **Cleaned by hand**: whoever created a file deletes it when it is no longer needed; no command does it.
- **Useful content goes to raw**: if something in an output is worth keeping (e.g. what came out of a call), whoever is on it writes it as a raw note in the relevant feature.
- File name: `YYYY-MM-DD-<topic>.md`.

## Ingest

### What to keep — the filter

The wiki describes the **current state** of the project and the reasons behind it, not the history of how we got there. History is already in git (commits, PRs) and in `raw/compiled/`.

Keep only what someone would need later to understand or change the project:

- game rules, product choices, decisions and their reasons, open questions;
- project code and content that **exists now**: scripts, scenes, prefabs, data assets (what they are, what they are for, where they live).

Leave out:

- the list of commits, intermediate attempts, refactors, fixes, renames, tooling or formatting changes: they matter only if they change what exists or a rule;
- meeting chatter, temporary details, things already obvious from the code, anything Unity or the plugins already document.

Update pages **in place**: rewrite the section that changed, never append a chronological log. If a source adds nothing that passes this filter, it changes no page (it still moves to `compiled/`). A large task should not mean a large wiki diff.

### Steps

What the ingest does, in order:

1. Collect what to process: the files in `raw/<feature-name>/` not yet in `compiled/`, plus the feature's commits, to know what custom code was added, changed or removed:
   ```bash
   git fetch origin
   git log --no-merges origin/develop..HEAD
   ```
   Always compare with `origin/develop` after a fetch, never with the local `develop` (it may be stale, and then other features' merged commits look like this feature's). `--no-merges` drops the merges of `develop` into the feature. Commits of other open features are never in this range.

   Also run the lint script (`node .claude/skills/lint-wiki/lint-wiki.mjs`) and keep its **maintenance warnings** (stale, code changed, consolidate, missing `## Alternative scartate`) for step 7.
2. Read `wiki/index.md`, the index of each area involved, and the pages involved.
3. For technical pages, link the Unity / plugin docs for what they already cover; never copy them.
4. Update or create the pages, the `index.md` of their folders and the radar in `wiki/index.md`. Update or delete the technical pages of code the feature changed or removed. Set `aggiornato` to today on every page you change; set `verificato` too only on new pages and on pages you checked whole (see **Wiki pages**). When a source replaces a decision's option or rules one out, add it to `## Alternative scartate` (see **Decisions**).
   - **Write for the agent** (see **Purpose**): update the index descriptions of the pages you touch, keep one `##` per entity named as in the code, cite code by existing repo-relative paths, and update the consumers/callers of what changed.
   - **Known issues**: add new issues to the right block of `wiki/tecnico/known-issues.md` (with tag, path and link) and **remove the items the feature fixed**; keep the summary table counts in sync.
   - **Scripts, scenes, prefabs**: from `git diff --stat origin/develop...HEAD -- Assets`, update the section of every class added, changed (behaviour, serialized fields, callers), moved or removed, and the page of every scene or prefab whose structure changed. Then the **reverse side**: for every new or removed reference to a project class (a call, a `GetComponent<T>`, a serialized field of that type), update the **Used by** of the *referenced* class, even though its own file did not change (`grep -rn "<ClassName>" Assets/Scripts` lists them; for scene and prefab references, `grep -rl "<script GUID>" Assets --include=*.unity --include=*.prefab`, the GUID being in the script's `.meta`).
5. **Conflicts**: when a source contradicts the wiki, keep the existing text, add the point to the page's open questions, and list it in the final summary. Never overwrite silently.
6. **Coherence**: check that the pages you touched agree with the rest of the documentation, not only with the sources. Scope: the touched pages and their neighbourhood, not the whole wiki.
   - **Other pages**: pages that link to or from the touched ones, and pages that mention the same entities, rules or decisions (`grep -rn` their names and key terms in `docs/wiki`). No contradictions on rules, numbers, names, statuses.
   - **Decisions**: no page states as settled what a decision leaves `aperta` or `in analisi`; pages that rely on a decision changed by this feature follow its new content.
   - **Discarded alternatives**: `grep -rn -A20 "## Alternative scartate" docs/wiki`. If a source proposes again something already ruled out, do not integrate it silently: point it out to the user with the reason it was discarded, and integrate it only if they confirm (then the old decision changes, with the new reason).
   - **Engine**: claims about Unity or plugin behaviour match their official docs.
   - **Code** (`tecnico/`): class names, serialized fields, scenes and callers described match the code and assets as they are now, not only the paths (the lint checks those).
   - **Project files**: `AGENTS.md`, `CONTRIBUTING.md`, `docs/CLAUDE.md` and the skills in `.claude/skills/` do not contradict the touched pages (e.g. a workflow rule).

   If the mismatch is in a wiki page and is caused by this feature (its sources are the newer truth), align that page in the same commit. Otherwise (two sources disagree, wiki against code or docs with no clear winner) treat it as a conflict (step 5). Mismatches in project files are only reported: the ingest commit touches the wiki and raw only. List all of them in the summary.
7. **Maintenance**: handle the maintenance warnings from step 1 (see **Freshness and consolidation**).
   - **Scope**: the flagged pages among the touched pages and their neighbourhood (the same scope as step 6), plus at most a few others. The rest is only shown to the user: those warnings stay on until a feature touches that part of the wiki.
   - **Verify for real**: against the code and assets (`tecnico/`), the Unity / plugin docs (engine behaviour) or a confirmation of the user in the session (business, decisions). Never set `verificato` without a check.
   - **Ask first** for anything structural: merging pages, superseding a decision, dropping content. Wait for the user's ok before the commit.
   - **Close every warning handled with a change that turns it off**, because the decision taken must be written: page confirmed or corrected → `verificato` to today; a doubt remains → add it to `## Domande aperte` and set `verificato` (the page was reviewed, the doubt is recorded); consolidation done, postponed or not needed → `consolidato` to today, with the reason in the commit body; missing `## Alternative scartate` → add it with the user. A warning that comes back in a later feature is then always a new problem.
   - **No report files**: the warnings live in the session and are solved there.
8. Move the processed sources to `raw/compiled/`: the **whole feature folder** (`raw/<feature-name>/` must not remain).
9. Create **one** commit with only the wiki and raw changes: `docs(wiki): update from <feature-name>`.
10. Report a summary: pages created, updated, deleted; pages aligned by the coherence check; conflicts; maintenance warnings handled and how, and those left out of scope; anything skipped.

If there is nothing new to process (no new raw, no custom code), make no changes and no commit.

A code feature that implemented a decision `aperta` or `in analisi` (see **Decisions**) sets it to `presa`, with what was implemented as the choice, and removes it from the radar.

### Two features on the wiki at the same time

The wiki is derived from the raw sources, so wiki conflicts are not resolved by hand: they are regenerated. `/close` merges `origin/develop` into the feature **before** the ingest, so the ingest works on the current wiki. If that merge conflicts in `docs/wiki/` (an earlier mid-feature ingest of this feature against pages changed by another feature), the feature takes `develop`'s wiki as it is and runs **one** ingest of all its sources: `raw/<feature-name>/` and `raw/compiled/<feature-name>/` (the only case where `compiled/` is read when processing). Conflicts outside `docs/wiki/` are resolved by the user.

Wiki changes never reach `develop` directly: they go through the feature's PR, reviewed by Michele.

## Using the wiki during development

How to read it (agent): open the indexes down to the page you need, then read **only the section**: search its heading (`## <Name>`) and read a few dozen lines around it, or use the index table at the top of long pages. Read a whole page only when it is short (under ~200 lines) or when the task covers all of it.

Sources, in this order:

1. `AGENTS.md` — **always**: structure, libraries in use, Unity rules.
2. `wiki/tecnico/` for the project's code, scenes and prefabs.
3. The other wiki areas (business, progetto, …) and their decisions before implementing a game rule or a product choice. If the rule is missing or unclear, ask instead of inventing it.
