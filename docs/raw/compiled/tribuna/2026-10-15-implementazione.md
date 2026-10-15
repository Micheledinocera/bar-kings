# Implementazione — tribuna

Opzione in prova per la decisione [Modifica del mazzo](../../wiki/business/modifica-del-mazzo.md), che resta `in analisi`: le tre scene sono le tre alternative in prova.

## Regole della tribuna (POC)

- Stesse logiche della panchina: mazzo oltre le 40 carte con solo doppioni (9, asso–nove di denari, mai re), 40 carte sul tavolo sempre con i 4 re, path con doppione impilato e buco, carta che arriva su un buco ci va scoperta e ferma il path.
- Differenza: le carte oltre le 40 vengono **buttate** (non create in scena) e i **buchi non si riempiono**.
- Pulsante Reset come nella panchina.

## Codice condiviso

- Il codice della panchina è diventato comune alle due scene, in `Assets/Scripts/Shared/Table/`: `TableState` (era `PanchinaTable`), `TableDeckManager` (era `DeckManagerPanchina`), `TableCardController`, `TableCardAnimations`, `CardSlot`, `CardSlotContainer`. La cartella `Assets/Scripts/Panchina/` non esiste più. GUID degli script invariati: scene e prefab non cambiano.
- `TableDeckManager.excessCards` sceglie l'idea: `Bench` (panchina, carte in eccesso nel mazzo panchina che riempie i buchi) o `Discard` (tribuna, carte in eccesso buttate).

## Scena `ScenaTribuna`

- Ora ha la stessa struttura della Panchina: `SlotContainer` con i 40 slot copiato dentro `CardsContainer`, `TableDeckManager` su `GameManager` al posto di `DeckManager3D` (`excessCards = Discard`, stesso prefab carta `3DCardTable`, stesso `CardDatabase`), `UI` con `ResetButton` collegato al suo `TableDeckManager`, `EventSystem` con `InputSystemUIInputModule`.
- Il prefab delle carte `3DCardPanchina` è stato rinominato `Assets/Prefab/3DCardTable.prefab` (stesso GUID): è usato da entrambe le scene.
