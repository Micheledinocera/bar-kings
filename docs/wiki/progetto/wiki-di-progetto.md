---
tipo: decisione
stato: presa
aggiornato: 2026-10-07
verificato: 2026-10-07
fonti:
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
3. `/close` fa il merge di `origin/develop`, lancia l'ingest e il lint, esegue i controlli e apre la PR verso `develop`.
4. L'**ingest** aggiorna il wiki in place, controlla la coerenza, gestisce i warning di manutenzione, sposta il raw in `raw/compiled/` e fa un commit `docs(wiki): …` dedicato.
5. Michele rivede anche il wiki nella PR prima del merge.

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
- Unity non ha docs locali versionate come Mercur, e scene e prefab non si leggono come codice: per questo servono il "Used by" via GUID e le docs ufficiali citate.

## Alternative scartate

- **Aree di base `gioco/` (game design) e `arte/` (direzione artistica, UI, audio) al posto di `business/`** — scartata il 2026-10-07: si parte sempre da `business/`, `tecnico/` e `progetto/` e le altre aree si aggiungono man mano (fonte: raw/compiled/llm-wiki/2026-10-07-adattamento-da-marketplace.md)
- **Cartella `guide/` di `docs/` per flussi e setup destinati alle persone**, come nel marketplace — scartata il 2026-10-07: per ora non serve, si aggiunge quando serve (fonte: raw/compiled/llm-wiki/2026-10-07-adattamento-da-marketplace.md)

## Domande aperte

- Se e quando altre persone che lavorano sul repo useranno skill e workflow, e chi rivedrà le loro PR.
- Il codice esistente (`Assets/Scripts/`) non è ancora documentato in `tecnico/`: serve una feature dedicata per il primo ingest del codice e per `known-issues.md`.
