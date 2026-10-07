---
tipo: decisione
stato: aperta
aggiornato: 2026-10-07
verificato: 2026-10-07
fonti:
  - raw/compiled/llm-wiki/2026-10-07-adattamento-da-marketplace.md
---

# Definition of done delle feature di codice

Quando una feature di codice è finita e può andare in PR.

## Cosa sappiamo

- Oggi `/close` esegue `dotnet csharpier check Assets/Scripts` (CSharpier è un tool locale in `dotnet-tools.json`), chiede all'utente di confermare che la Console di Unity non ha errori di compilazione e che la feature funziona in Play mode, e controlla che ogni asset aggiunto abbia il suo `.meta` (vedi `.claude/skills/close/SKILL.md` e `.github/pull_request_template.md`).
- La compilazione si verifica solo nell'Editor: l'agent non può lanciarla da solo.
- Oggi il check di formattazione fallisce su 15 file su 16 di `Assets/Scripts/`: il primo `/close` di una feature di codice si fermerebbe lì finché gli script non vengono formattati.

## Domande aperte

- Test: servono (il Unity Test Framework è già tra i pacchetti)? Per quali modifiche?
- Build: serve una build del gioco (es. Unity in batchmode) prima della PR?
- Formattazione: si formatta tutto `Assets/Scripts/` in una feature dedicata prima delle feature di codice?
