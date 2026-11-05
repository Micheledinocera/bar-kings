---
tipo: decisione
stato: aperta
aggiornato: 2026-10-29
verificato: 2026-10-29
fonti:
  - raw/compiled/study-roadmap/2026-10-29-delucidazioni.md
  - raw/compiled/llm-wiki/2026-10-07-adattamento-da-marketplace.md
---

# Tipi di ramo e release

Come si usano i rami git-flow diversi da `feature/*` e come si fa una release verso `main`.

## Cosa sappiamo

- git-flow è inizializzato con i prefissi `feature/`, `bugfix/`, `release/`, `hotfix/` (vedi [Branching, commit e pull request](./git-flow-e-convenzioni.md)).
- Le skill (`/start`, `/close`, `/ingest`) gestiscono solo `feature/*`: per ora anche i fix si fanno come feature (`CONTRIBUTING.md`).
- **Per ora solo feature** (team, 2026-10-29): `bugfix/`, `hotfix/` e `release/` non si usano. Le release si decidono più avanti: la [vertical slice](./roadmap-vertical-slice.md) è solo per il team e non ne ha bisogno.

## Opzioni

- Proposta dell'agent (study-roadmap, 2026-10-29), rimandata: `release/<versione>` da `develop` solo per le build da dare fuori dal team, build Windows verificata, merge su `main` con tag `v0.<n>.0`; `bugfix/` e `hotfix/` solo quando ci sarà una build pubblica da correggere; una skill `/release` solo se le release diventano frequenti.

## Domande aperte

- Flusso di release verso `main`: quando si fa (prima build per qualcuno fuori dal team?), come si numerano le versioni, cosa si verifica (build del gioco?).
- `bugfix/` e `hotfix/`: servono quando ci sarà una build pubblica, o si resta sempre a feature?
