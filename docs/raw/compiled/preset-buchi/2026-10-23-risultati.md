# Risultati della simulazione — preset-buchi

Tribuna (`Discard`), fine al 4° re, 300 partite per preset e strategia, seed 20261023 (stesse distribuzioni per le due strategie). File in `Simulations/20261023-preset-buchi-2-Tribuna-*.csv` (non versionati). Regole in prova: la decisione [Modifica del mazzo](../../wiki/business/modifica-del-mazzo.md) resta `in analisi`.

## Buchi e forme a fine partita (medie per partita; a caso / mirata)

| Preset | Path al 4° re | Buchi | Forme per partita | Partite con almeno una forma |
|---|---|---|---|---|
| Preset32 | 10,9 / 7,1 | 7,4 / 7,9 | ~0,04 | 4% / 6% |
| Preset71 | 12,6 / 9,9 | 8,0 / 8,1 | ~0,13 / 0,17 | 10% / 14% |
| Copie2 / 4 / 8 | 4,0 / 6,5 / 10,8 (a caso) | 0,8 / 2,4 / 5,6 (a caso) | 0 | 0–1% |
| ColonnaSecca | 8,1 / 4,5 | 4 / 4 | 0,18 / 0,79 colonne | 18% / 79% |
| ColonnaPiena | 6,5 / 3,7 | 2,8 / 3,3 | 0,14 / 0,56 colonne | 14% / 56% |
| DiagonaleSecca / Piena | 7,8 / 4,3 (secca) | 4 / 4 (secca) | 0,13 / 0,73 (secca) | 13–17% / 56–73% |
| Quadrato2Secca / Piena | 7,8 / 4,5 (secca) | 4 / 4 (secca) | 0,12 / 0,76 (secca) | 12–18% / 56–76% |
| Quadrato3Secca | 12,8 / 8,3 | 9 / 9 | 3×3: 0,16 / 0,55; 2×2: 1,2 / 2,8 | 52% / 85% |
| Quadrato4Secca | 13,7 / 10,6 | 16 / 16 | 4×4: 0,20 / 0,39; e tante forme minori | 95% / 99% |
| Quadrato4Piena | 14,8 / 11,9 | 11,0 / 11,8 | 4×4: 0,11 / 0,24 | 71% / 81% |
| SemeSecca | 12,7 / 8,2 | 9 / 9 | ~0 (la riga non è una forma) | 2% / 0% |
| PariSecca | 12,3 / 9,0 | 16 / 16 | 2,0 / 3,0 colonne | 96% / 99% |

## Cosa si vede

- **Buchi a fine partita**: nei mazzi secchi (meno di 40 carte) i buchi sono sempre tanti quante le carte mancanti. Nessuna carta lascia il tavolo, quindi i buchi si spostano soltanto. Con i doppioni (mazzi pieni, preset di oggi) restano 3–8 buchi.
- **Senza carte scelte le forme quasi non escono**: con i preset di oggi c'è una forma nel 4–14% delle partite, quasi sempre un 2×2 o una diagonale.
- **Le forme vengono dalle carte tolte**: giocando a caso esce la forma voluta in circa 1 partita su 6–8, giocando mirato in 3 su 4 (forme di 4 carte, mazzo secco), in poco più di metà con i doppioni.
- **Le forme grandi si chiudono di rado**: il 4×4 richiede almeno 16 path, e il 4° re arriva prima. Escono però molte forme più piccole al suo interno.
- **I preset secchi grandi** (Quadrato4Secca, PariSecca) hanno forme in quasi tutte le partite: tante carte mancanti = tanti buchi, anche dagli slot vuoti iniziali (ultimi slot, bastoni e spade).
