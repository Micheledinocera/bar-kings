---
tipo: decisione
stato: presa
aggiornato: 2026-10-15
verificato: 2026-10-11
consolidato: 2026-10-11
fonti:
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
- **Stop veri** (senza, la PR sarebbe sbagliata): ramo diverso da `feature/*`, lavoro non committato, `gh` non autenticato, conflitti di merge fuori dal wiki. Per uno studio che contiene file diversi dalla documentazione, `/close` chiede se spostarli.
- Riferimenti: `.claude/skills/close/SKILL.md`, `.github/pull_request_template.md`, `CONTRIBUTING.md`.

## Perché

- I contributor umani non devono essere rallentati da controlli pensati per l'agent (indicazione dell'utente, 2026-10-11).
- Il check di formattazione falliva su 15 script su 16: il primo `/close` di una feature di codice si sarebbe fermato lì.

## Conseguenze

- Gli asset che Unity riscrive a ogni Play non risultano più modificati: `.gitattributes` forza LF (vedi [Branching, commit e pull request](./git-flow-e-convenzioni.md)). `/start` e `/close` ignorano comunque i file con `git diff` vuoto.
- Tutto `Assets/Scripts/` è stato formattato con CSharpier al `/close` di card-generator, la prima feature di codice: da lì in poi la formattazione riguarda solo i file nuovi o modificati.

## Alternative scartate

- **`dotnet csharpier check` bloccante al `/close`** — scartata il 2026-10-11: nessuna regola deve fermare il lavoro delle persone; si formatta invece di controllare (fonte: raw/compiled/study-new-implementations/2026-10-11-risposte-verifiche.md)
- **Errori in Console e `.meta` mancanti come stop del `/close`** — scartata il 2026-10-11: diventano note nella PR, per lo stesso motivo (fonte: raw/compiled/study-new-implementations/2026-10-11-risposte-verifiche.md)

## Domande aperte

- Test: servono (il Unity Test Framework è già tra i pacchetti)? Per quali modifiche? Le regole di `AGENTS.md` separano già la logica di gioco dalla vista per poterla testare in Edit Mode.
- Build: serve una build del gioco (es. Unity in batchmode) prima della PR? Il codice editor noto ora sta in `Editor/`, ma la build non è mai stata provata. Un'opzione è Unity in batchmode (scartato per il lavoro sull'Editor, non per la build), vedi [Accesso dell'agent all'Unity Editor](./accesso-editor-agent.md).
