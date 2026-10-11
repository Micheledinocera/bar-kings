# Obiettivo — card-generator

Riprendiamo da dove ci eravamo fermati con lo studio (study-new-implementations): primo lavoro dell'ordine deciso, il blocco A dei known issues.

## Riferimenti

- [Known issues, blocco A — generazione delle carte e codice editor](../../wiki/tecnico/known-issues.md#block-a--card-generation-and-editor-code):
  - i generatori usano `UnityEditor` fuori da una cartella `Editor/`, quindi la build non compila;
  - vengono chiamati a runtime in `Start` e riscrivono i 40 asset a ogni Play. Devono girare solo dal menu, senza ricreare gli asset che esistono già (indicazione dell'utente nello studio);
  - seme e valore sono dedotti dall'ordine dei file;
  - i due generatori sono identici.
- Classi coinvolte: [CardGenerator](../../wiki/tecnico/shared.md#cardgenerator), [CardGeneratorPanchina](../../wiki/tecnico/panchina.md#cardgeneratorpanchina), [DeckManager3D](../../wiki/tecnico/shared.md#deckmanager3d), [DeckManagerPanchina](../../wiki/tecnico/panchina.md#deckmanagerpanchina).
- Regole da seguire: `AGENTS.md`, sezione *Scripts* (codice editor solo in `Editor/`, generatori da menu che saltano gli asset esistenti).
