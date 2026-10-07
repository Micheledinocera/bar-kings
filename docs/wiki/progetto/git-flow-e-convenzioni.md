---
tipo: decisione
stato: presa
aggiornato: 2026-10-07
verificato: 2026-10-07
fonti:
  - raw/compiled/llm-wiki/2026-10-07-adattamento-da-marketplace.md
---

# Branching, commit e pull request

## Decisione

- **git-flow**: `main` solo release con tag `v*`, `develop` integrazione, un ramo `feature/<nome>` per ogni task. Le feature di sola analisi si chiamano `study-<tema>` e non toccano il codice; una feature che riprende un tema già trattato tiene il nome con un numero (`study-x-2`).
- **Commit**: Conventional Commits in inglese, **atomici**; ogni asset si committa con il suo `.meta`.
- **PR** sempre verso `develop`, aperte da `/close` con GitHub CLI; il merge si fa su GitHub, mai con `git flow feature finish` in locale. Per ora le PR le gestisce e le rivede solo Michele.
- **Merge**: **squash** per feature e bugfix verso `develop`; **merge normale** per release e hotfix verso `main` e per il loro ritorno su `develop`.
- **Allineamento con `develop`**: merge di `origin/develop` nel ramo della feature, mai rebase di un ramo già pushato.
- **Scene e prefab**: due feature aperte insieme non modificano la stessa scena o lo stesso prefab senza accordarsi prima.
- Le regole operative stanno in `CONTRIBUTING.md`, letto da persone e agenti; `AGENTS.md` ci rimanda.

## Perché

- Stesse convenzioni del marketplace InfinityCar, da cui il workflow è ripreso.
- `git flow feature finish` farebbe il merge in `develop` in locale, saltando la revisione della PR e del wiki.
- Scene, prefab e asset sono YAML serializzati da Unity: i conflitti di merge su questi file si risolvono male a mano.

## Conseguenze

- Dopo lo squash il ramo locale della feature non risulta unito: si cancella con `git branch -D`. `/start` segnala i rami locali già uniti.
- Oggi le skill gestiscono solo `feature/*`: gli altri tipi di ramo sono una [decisione aperta](./tipi-di-ramo-e-release.md).

## Alternative scartate

Nessuna alternativa considerata.
