# Adattamento dell'LLM wiki dal marketplace

Base: il setup del marketplace InfinityCar (`docs/CLAUDE.md`, skill `/start`, `/close`, `/ingest`, `/report`, `/lint-wiki`, `CONTRIBUTING.md`, template PR). Il razionale di fondo (wiki come indice per l'agent, raw legati alle feature, ingest a fine feature, decisioni con stato e radar, freschezza e consolidamento, niente backlog) è quello del marketplace e resta invariato.

## Cosa resta uguale

- git-flow: `main` per le release, `develop` di integrazione, `feature/<nome>` per ogni task; PR verso `develop` con squash; allineamento con merge di `origin/develop`, mai rebase.
- Conventional Commits in inglese; commit del wiki `docs(wiki): update from <feature>`.
- Feature `study-<tema>` senza codice; riprese numerate (`study-x-2`).
- `docs/raw/<feature>/` → `docs/raw/compiled/<feature>/` all'ingest; raw mai modificati dopo l'ingest.
- `docs/output/` fuori da git e ignorato dall'agent.
- Frontmatter, stati delle decisioni, radar, `## Alternative scartate`, soglie di freschezza e consolidamento.
- Lingua: wiki in italiano, `tecnico/` e codice in inglese.

## Cosa cambia per un progetto Unity

- **Aree del wiki**: di base sempre `business/` (il gioco come prodotto: concept, regole del gioco, pubblico, mercato e concorrenti, monetizzazione, glossario), `tecnico/` e `progetto/`. Le altre (es. `arte/`) si aggiungono man mano, alla prima pagina che serve.
- **Revisione delle PR**: per ora le gestisce solo Michele; più avanti si vedrà se coinvolgere altri.
- **`tecnico/`**: una pagina per cartella di `Assets/Scripts/` (o per sistema), una sezione `## <NomeClasse>` per classe; una pagina per scena. Riferimenti per percorso dalla root (`Assets/Scripts/...`).
- **Known issues**: una sola pagina `wiki/tecnico/known-issues.md` per tutto il progetto (non una per app). Tag adattati: `bug`, `ux`, `performance`, `dead-code`, `duplicate`, `debt`, `to-verify`, `decision`.
- **Ingest del codice**: al posto del catalogo dei componenti dello storefront, si aggiornano le sezioni delle classi toccate e il loro "Used by" (riferimenti nel codice con `grep`, nelle scene e nei prefab tramite il GUID nel `.meta`).
- **Documentazione esterna**: niente docs locali tipo Mercur; si citano le docs ufficiali di Unity e dei plugin (Odin Inspector, DOTween, UniTask, TextMeshPro).
- **Lint**: controlla i percorsi `Assets/`, `Packages/`, `ProjectSettings/`, `.claude/`, `docs/`; tolto il controllo del catalogo storefront.
- **Check di `/close`**: `dotnet csharpier check Assets/Scripts` (CSharpier è già in `dotnet-tools.json`); la compilazione si verifica solo nell'Editor, quindi `/close` chiede all'utente di confermare che la Console di Unity non ha errori e che la feature funziona in Play mode; controlla che ogni asset aggiunto abbia il suo `.meta`.
- **Conflitti su scene e prefab**: sono YAML di Unity e si uniscono male; due feature aperte insieme non toccano la stessa scena o lo stesso prefab senza accordarsi.
- Tolti `docs/guide/` (flussi e setup per le persone): non servono ancora; si aggiungono quando servono.

## Punti aperti

- Se e quando altre persone sul repo useranno skill e workflow, e chi rivedrà le loro PR.
- Formattazione: oggi `dotnet csharpier check Assets/Scripts` fallisce su 15 file su 16; il primo `/close` di una feature di codice si fermerebbe. Serve una feature che formatti gli script.
- Il primo ingest del codice esistente (`Assets/Scripts/`) va fatto in una feature dedicata, per popolare `tecnico/` e `known-issues.md` (es. `Assets/Scripts/Panchina/` e `Assets/Scripts/Shared/` hanno classi con lo stesso nome: `CardController`, `DeckManager`, `CardGenerator`, `CardAnimations`).
- Definition of done di una feature di codice (test? build?) e tipi di ramo/release: da decidere, come nel marketplace.
