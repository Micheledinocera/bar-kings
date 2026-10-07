---
tipo: decisione
stato: aperta
aggiornato: 2026-10-07
verificato: 2026-10-07
fonti:
  - raw/compiled/llm-wiki/2026-10-07-adattamento-da-marketplace.md
---

# Tipi di ramo e release

Come si usano i rami git-flow diversi da `feature/*` e come si fa una release verso `main`.

## Cosa sappiamo

- git-flow è inizializzato con i prefissi `feature/`, `bugfix/`, `release/`, `hotfix/` (vedi [Branching, commit e pull request](./git-flow-e-convenzioni.md)).
- Le skill (`/start`, `/close`, `/ingest`) gestiscono solo `feature/*`: per ora anche i fix si fanno come feature (`CONTRIBUTING.md`).

## Domande aperte

- `bugfix/`, `hotfix/` e `release/`: si gestiscono con le skill o si fa tutto come feature?
- Flusso di release verso `main`: quando si fa, come si numerano le versioni, cosa si verifica (build del gioco?).
