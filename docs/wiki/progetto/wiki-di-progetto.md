---
tipo: decisione
stato: presa
aggiornato: 2026-11-11
verificato: 2026-10-07
consolidato: 2026-11-11
fonti:
  - raw/compiled/new-ingest/2026-11-11-obiettivo.md
  - raw/compiled/new-ingest/2026-11-11-risposte.md
  - raw/compiled/merge-rpg/2026-10-29-obiettivo.md
  - raw/compiled/study-call-28-10-2026/2026-10-28-note-call.md
  - raw/compiled/llm-wiki/2026-10-07-obiettivo.md
  - raw/compiled/llm-wiki/2026-10-07-adattamento-da-marketplace.md
---

# Wiki di progetto

Come è fatto il wiki, come si aggiorna e come resta affidabile nel tempo. Le regole operative sono in `docs/CLAUDE.md`; qui ci sono le scelte e il perché.

## Decisione

Si adotta il pattern "LLM wiki" già in uso sul marketplace InfinityCar, adattato a un progetto Unity. **Il primo lettore del wiki è l'agent** che lavora sul codice: indici con descrizioni, una sezione `##` per entità con il nome del codice, percorsi verificabili, chi usa cosa, tabelle invece di prosa.

### Ciclo

Fonti grezze che non si modificano (`docs/raw/`), pagine scritte e aggiornate dall'LLM (`docs/wiki/`), regole per l'agent (`docs/CLAUDE.md`). Il ciclo è legato a git-flow (vedi [Branching, commit e pull request](./git-flow-e-convenzioni.md)):

1. `/start <nome>` apre `feature/<nome>` e la cartella `raw/<nome>/` con la nota sull'obiettivo.
2. Le note si aggiungono quando serve, solo dentro una feature; l'utente dice cosa va registrato.
3. `/close` fa il merge di `origin/develop`, esegue i controlli, apre la PR verso `develop` e la unisce da solo (vedi [Branching, commit e pull request](./git-flow-e-convenzioni.md)). La feature non tocca il wiki.
4. Subito dopo il merge, `/ingest <nome>` apre `feature/ingest-<nome>` dal `develop` aggiornato: aggiorna il wiki in place partendo dai raw e dal commit di squash della feature, controlla la coerenza, gestisce i warning di manutenzione, sposta il raw in `raw/compiled/<nome>/` e fa un commit `docs(wiki): update from <nome>`. Poi push, PR e merge immediato, senza altri controlli.
5. Se la PR della feature è stata unita a mano (conflitti veri), chi l'ha unita lancia `/ingest <nome>`. Le cartelle in `raw/` diverse da `compiled/` su `develop` sono feature unite in attesa di ingest. Non si fa ingest a metà feature.

### Contenuto

- **Sì**: regole del gioco, scelte di prodotto, decisioni prese e da prendere, e il codice e i contenuti del progetto (script, scene, prefab, asset di dati).
- **No**: come si scrive il codice (`AGENTS.md`), il comportamento di Unity e dei plugin (Odin Inspector, DOTween, UniTask, TextMeshPro: si citano le docs ufficiali), il contenuto di `Assets/Plugins/` e `Packages/`, la cronologia (git e `raw/compiled/`), gli output per le persone (cartella `output/` di `docs/`, fuori da git e mai letta dall'agent).
- **Lingua**: inglese per il codice e `wiki/tecnico/`, italiano per il resto; chiavi del frontmatter sempre in italiano.
- **Niente backlog**: un task esiste solo come feature.

### Struttura

- **Aree di base**: `business/` (il gioco come prodotto: concept, regole del gioco, pubblico, mercato e concorrenti, monetizzazione, glossario), `progetto/`, `tecnico/`. Le altre aree (es. `arte/`) si aggiungono man mano, alla prima pagina che ne ha bisogno.
- **`tecnico/`**: una pagina per cartella di `Assets/Scripts/` (o per sistema) con una sezione `## <NomeClasse>` per classe, una pagina per scena; sottocartelle che seguono `Assets/Scripts/`. Un solo `known-issues.md` per tutto il progetto, a blocchi per area di codice.
- **Indici, frontmatter, decisioni con stato e radar, freschezza e consolidamento, `## Alternative scartate`**: come nel marketplace, regole in `docs/CLAUDE.md`.

### Adattamenti per Unity

- **Ingest del codice**: per ogni classe toccata si aggiornano la sua sezione e il "Used by" delle classi che referenzia, cercando nel codice con `grep` e nelle scene e nei prefab tramite il GUID nel `.meta`.
- **Lint**: controlla che esistano i percorsi `Assets/`, `Packages/`, `ProjectSettings/`, `.claude/` e `docs/` citati; segnala le pagine `tecnico/` i cui file citati hanno commit dopo `verificato`.
- **Check di `/close`**: vedi [Definition of done](./definition-of-done.md).

## Perché

- Stesso workflow del marketplace: le regole e le skill sono già provate, e si lavora allo stesso modo sui due progetti.
- Poche aree di base, aggiunte quando servono: un'area vuota costa un indice da leggere e non dice niente all'agent.
- **Ingest dopo il merge, in una feature a parte** (2026-11-11): con l'ingest dentro la PR della feature, al merge sul wiki c'erano spesso molti conflitti, perché due feature aperte insieme rigeneravano le stesse pagine. Una feature di solo ingest parte dal wiki corrente e viene aperta e chiusa subito, quindi è praticamente impossibile che qualcun altro unisca qualcosa nel mezzo; con solo wiki e raw non c'è niente che i controlli di `/close` possano trovare.
- Unity non ha docs locali versionate come Mercur, e scene e prefab non si leggono come codice: per questo servono il "Used by" via GUID e le docs ufficiali citate.

## Alternative scartate

- **Aree di base `gioco/` (game design) e `arte/` (direzione artistica, UI, audio) al posto di `business/`** — scartata il 2026-10-07: si parte sempre da `business/`, `tecnico/` e `progetto/` e le altre aree si aggiungono man mano (fonte: raw/compiled/llm-wiki/2026-10-07-adattamento-da-marketplace.md)
- **Ingest dentro la PR della feature, al `/close`, con revisione del wiki nella PR** (e rigenerazione del wiki di `develop` in caso di conflitto) — scartata il 2026-11-11: troppi conflitti sul wiki al merge; ora l'ingest gira dopo il merge in `feature/ingest-<nome>` (fonte: raw/compiled/new-ingest/2026-11-11-obiettivo.md)
- **Cartella `guide/` di `docs/` per flussi e setup destinati alle persone**, come nel marketplace — scartata il 2026-10-07: per ora non serve, si aggiunge quando serve (fonte: raw/compiled/llm-wiki/2026-10-07-adattamento-da-marketplace.md)

## Domande aperte

- Se e quando altre persone che lavorano sul repo useranno skill e workflow, e chi rivedrà le loro PR. Stato al 2026-10-28 (call): luigi-ds ha iniziato la parte RPG sul branch `feat/rpg_implementation`, fuori da git-flow, con note raw ma senza ingest; il 2026-10-29 è stato portato dentro con la feature `merge-rpg`, che ha spostato le note e rimesso a posto il wiki (vedi [Parte RPG](../business/parte-rpg.md#cosa-sappiamo)); un'altra persona che vuole entrare nel progetto usa Gemini Pro invece di Claude Code.
- Il wiki e il workflow funzionano con agenti diversi da Claude Code (es. Gemini)? L'idea è dire esplicitamente all'agent di leggere i `CLAUDE.md`; la preoccupazione maggiore sono le skill. Nota dell'agent: le regole stanno già in `AGENTS.md` (`CLAUDE.md` lo importa soltanto) e in `docs/CLAUDE.md`, file di testo leggibili da qualunque agent; le skill in `.claude/skills/` sono invece un formato di Claude Code, quindi per un altro agent andrebbero lette come istruzioni a mano o adattate. Da verificare con l'agent che userà.
- Il codice esistente (`Assets/Scripts/`) non è ancora documentato in `tecnico/`: serve una feature dedicata per il primo ingest del codice e per `known-issues.md`.
