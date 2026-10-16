---
tipo: decisione
stato: presa
aggiornato: 2026-10-16
verificato: 2026-10-11
consolidato: 2026-10-11
fonti:
  - raw/compiled/build/2026-10-16-comando-build.md
  - raw/compiled/build/2026-10-16-modifiche-non-salvate.md
  - raw/compiled/unity-mcp/2026-10-15-decisione-mcp.md
  - raw/compiled/card-generator/2026-10-11-modifiche-editor.md
  - raw/compiled/git-line-endings/2026-10-11-obiettivo.md
  - raw/compiled/llm-wiki/2026-10-07-adattamento-da-marketplace.md
  - raw/compiled/study-new-implementations/2026-10-11-risposte-verifiche.md
  - raw/compiled/study-new-implementations/2026-10-11-proposta-regole.md
---

# Definition of done delle feature di codice

Quando una feature di codice è finita e può andare in PR.

## Decisione

- **Nessuna regola è bloccante.** Alcuni contributor sono persone. Formattazione, convenzioni di `AGENTS.md`, lint del wiki e `.meta` mancanti non fermano mai il `/close`. L'agent sistema quello che può, anche sul lavoro fatto a mano nella feature, e il resto va nelle *Notes* della PR.
- **Formattazione**: `/close` esegue `dotnet csharpier format Assets/Scripts` e committa il risultato (`style: …`). Per le persone è facoltativa.
- **`.meta`**: `/close` committa quelli che mancano, se esistono su disco; gli altri li segnala nella PR (si generano aprendo Unity).
- **Codice scritto dalle persone**: `/close` lo confronta con le regole di `AGENTS.md`, applica i fix piccoli e sicuri che non cambiano il comportamento e segnala il resto. Non rinomina mai gli identificatori e non chiede di rifare il lavoro.
- **Unity**: `/close` legge da solo la Console tramite `unity-mcp` (nessun errore di compilazione, nessun errore o warning nuovo; vedi [Accesso dell'agent all'Unity Editor](./accesso-editor-agent.md)); se l'MCP non è disponibile lo chiede all'utente. All'utente chiede se la feature funziona in Play mode. I problemi segnalati vanno nella PR, e l'utente decide se sistemarli subito o dopo.
- **Niente di non salvato**: prima di leggere la Console `/close` salva tutto quello che è aperto nell'Editor (scene e asset) e guarda `git status`. Unity segna spesso una scena modificata senza cambiarla: salvata, git non mostra niente. Un file che cambia davvero lo mostra all'utente, che decide se appartiene alla feature (si committa) o no (si annulla).
- **Build**: `/close` lancia la build Windows dal menu `Tools/Build Windows` e controlla esito, errori e warning, peso rispetto all'ultimo noto e `git status` pulito dopo la build (vedi [scenes, Build](../tecnico/scenes.md#build)). Come gli altri controlli non blocca: i problemi vanno nella PR. Si salta per gli studi senza codice.
- **Stop veri** (senza, la PR sarebbe sbagliata): ramo diverso da `feature/*`, lavoro non committato, `gh` non autenticato, conflitti di merge fuori dal wiki. Per uno studio che contiene file diversi dalla documentazione, `/close` chiede se spostarli.
- Riferimenti: `.claude/skills/close/SKILL.md`, `.github/pull_request_template.md`, `CONTRIBUTING.md`.

## Perché

- I contributor umani non devono essere rallentati da controlli pensati per l'agent (indicazione dell'utente, 2026-10-11).
- Il check di formattazione falliva su 15 script su 16: il primo `/close` di una feature di codice si sarebbe fermato lì.
- Build a ogni `/close`: regola fissata dall'utente il 2026-10-16 (feature build). La build trova quello che la Console non mostra (es. texture non compresse, file di impostazioni riscritti) e con il menu item è uguale per persone e agent.
- Al posto di una guida per fare la build a mano, un comando: una guida si dimentica, un comando si richiama sempre uguale.

## Conseguenze

- Gli asset che Unity riscrive a ogni Play non risultano più modificati: `.gitattributes` forza LF (vedi [Branching, commit e pull request](./git-flow-e-convenzioni.md)). `/start` e `/close` ignorano comunque i file con `git diff` vuoto.
- Tutto `Assets/Scripts/` è stato formattato con CSharpier al `/close` di card-generator, la prima feature di codice: da lì in poi la formattazione riguarda solo i file nuovi o modificati.

## Alternative scartate

- **`dotnet csharpier check` bloccante al `/close`** — scartata il 2026-10-11: nessuna regola deve fermare il lavoro delle persone; si formatta invece di controllare (fonte: raw/compiled/study-new-implementations/2026-10-11-risposte-verifiche.md)
- **Errori in Console e `.meta` mancanti come stop del `/close`** — scartata il 2026-10-11: diventano note nella PR, per lo stesso motivo (fonte: raw/compiled/study-new-implementations/2026-10-11-risposte-verifiche.md)
- **Skill di build che a ogni build rimette a posto i file di impostazioni riscritti e sistema gli import** — scartata il 2026-10-16: curava il sintomo; le cause si sistemano una volta (import delle texture nei `.meta`, impostazioni committate nello stato che la build scrive) (fonte: raw/compiled/build/2026-10-16-sistemazione.md)
- **Guida alla build tra gli output dei report (`/report`)** — scartata il 2026-10-16: la cartella non è versionata ed è temporanea; al suo posto il menu item `Tools/Build Windows` e la sezione Build del wiki (fonte: raw/compiled/build/2026-10-16-comando-build.md)

## Domande aperte

- Test: servono (il Unity Test Framework è già tra i pacchetti)? Per quali modifiche? Le regole di `AGENTS.md` separano già la logica di gioco dalla vista per poterla testare in Edit Mode.
