---
tipo: decisione
stato: presa
aggiornato: 2026-11-11
verificato: 2026-10-07
consolidato: 2026-11-11
fonti:
  - raw/compiled/new-ingest/2026-11-11-obiettivo.md
  - raw/compiled/new-ingest/2026-11-11-risposte.md
  - raw/compiled/git-line-endings/2026-10-11-obiettivo.md
  - raw/compiled/llm-wiki/2026-10-07-adattamento-da-marketplace.md
  - raw/compiled/study-new-implementations/2026-10-11-risposte-verifiche.md
---

# Branching, commit e pull request

## Decisione

- **git-flow**: `main` solo release con tag `v*`, `develop` integrazione, un ramo `feature/<nome>` per ogni task. Le feature di sola analisi si chiamano `study-<tema>` e non toccano il codice: possono modificare solo documentazione, cioè qualunque file `*.md` (wiki, raw, `AGENTS.md`, `CONTRIBUTING.md`, skill, template) e tutto ciò che sta in `docs/` e `.claude/`. Così regole e skill decise in uno studio si aggiornano nello studio stesso; una feature che riprende un tema già trattato tiene il nome con un numero (`study-x-2`).
- **Commit**: Conventional Commits in inglese, **atomici**; ogni asset si committa con il suo `.meta`.
- **PR** sempre verso `develop`, aperte da `/close` con GitHub CLI; il merge si fa su GitHub, mai con `git flow feature finish` in locale. **`/close` unisce da solo la PR** (`gh pr merge --squash`) e poi lancia l'ingest in `feature/ingest-<nome>`, che si apre e si unisce subito (vedi [Wiki di progetto](./wiki-di-progetto.md#ciclo)).
- **Conflitti con `develop`**: quelli "facili" (spazi, fine riga, EOF, formattazione diversa) `/close` li risolve da solo e unisce; con conflitti veri (contenuto diverso dai due lati, scene e prefab salvo soli spazi) non unisce, commenta la PR con una proposta di risoluzione e lo sviluppatore fa il merge a mano. `/close` non unisce neanche quando il codice non funziona (vedi [Definition of done](./definition-of-done.md)).
- **Merge**: **squash** per feature e bugfix verso `develop`; **merge normale** per release e hotfix verso `main` e per il loro ritorno su `develop`.
- **Branch di default su GitHub**: `develop`, così le PR puntano lì e dopo la chiusura di una PR il checkout va su `develop`. Il ramo remoto si cancella da solo dopo il merge (impostazione del repo).
- **Fine riga**: `.gitattributes` forza LF per i file di testo anche nella copia di lavoro, perché Unity scrive gli asset in LF; senza, con `core.autocrlf=true`, ogni Play lasciava decine di asset falsamente modificati.
- **Allineamento con `develop`**: merge di `origin/develop` nel ramo della feature, mai rebase di un ramo già pushato.
- **Scene e prefab**: due feature aperte insieme non modificano la stessa scena o lo stesso prefab senza accordarsi prima.
- **Regole mai bloccanti**: alcuni contributor sono persone; convenzioni, formattazione e lint non fermano il lavoro, si sistemano al `/close` (vedi [Definition of done](./definition-of-done.md)).
- Le regole operative stanno in `CONTRIBUTING.md`, letto da persone e agenti; `AGENTS.md` ci rimanda.

## Perché

- Stesse convenzioni del marketplace InfinityCar, da cui il workflow è ripreso.
- `git flow feature finish` farebbe il merge in `develop` in locale, senza PR.
- Merge automatico (utente, 2026-11-11): velocizza il processo; la revisione resta sui conflitti veri, gli unici che chiedono una scelta.
- Scene, prefab e asset sono YAML serializzati da Unity: i conflitti di merge su questi file si risolvono male a mano.

## Conseguenze

- Dopo lo squash il ramo locale della feature non risulta unito per git: lo cancella `/close` dopo il merge automatico, altrimenti `/start` al prossimo avvio, se la sua PR risulta unita su GitHub (a mano: `git branch -D`).
- Il prefisso `ingest-` è riservato a `/ingest`.
- Oggi le skill gestiscono solo `feature/*`: gli altri tipi di ramo sono una [decisione aperta](./tipi-di-ramo-e-release.md).

## Alternative scartate

- **Nessun merge senza revisione: PR unite a mano da Michele** — scartata il 2026-11-11: `/close` unisce da solo, salvo conflitti veri o codice che non funziona (fonte: raw/compiled/new-ingest/2026-11-11-risposte.md)
