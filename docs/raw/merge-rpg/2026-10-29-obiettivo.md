# Obiettivo — merge-rpg

Bonifichiamo la situazione con il branch `feat/rpg_implementation`. Il merge dovrebbe essere tranquillo, l'unico problema è la nomenclatura che cozza con le regole della llm-wiki.

Punto di partenza: la decisione [Parte RPG e collegamento con il pathbuilder](../../wiki/business/parte-rpg.md) (in analisi).

## Bonifica fatta (2026-10-29)

- `feat/rpg_implementation` (un solo commit, `5f40893`) è stato unito in questa feature senza conflitti: il lavoro RPG passa da qui verso `develop`, con `/close`.
- Le note raw di quel ramo sono state spostate da `docs/raw/rpg_implementation/` a questa cartella (l'obiettivo di allora è `2026-10-28-obiettivo-rpg.md`); i riferimenti al ramo `feat/rpg_implementation` nelle note sono storici.
- La riga aggiunta a mano in `wiki/business/parte-rpg.md` (momento del fight nel path) è stata tolta: il wiki si aggiorna solo con l'ingest, e il punto aperto è già nella nota `2026-10-28-fight.md`.
