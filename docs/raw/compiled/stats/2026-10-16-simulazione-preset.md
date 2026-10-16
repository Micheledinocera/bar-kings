# Simulazione dei preset — prime medie

2026-10-16. Prima simulazione dei 4 preset, lanciata dall'Editor con le stesse classi delle scene (`DeckBuilder`, `TableState`) prima che esistesse il tasto di simulazione.

## Come

- 2000 partite per preset e per modalità, seed fisso 12345.
- Autoplay: ogni path parte da uno slot coperto scelto **a caso**; la partita finisce quando non restano carte coperte. È una strategia di prova per le statistiche, non una regola del gioco.
- Lunghezza di un path = carte spostate dal path (la carta della panchina che riempie lo slot di partenza non conta).
- "4 re al path" = numero del path dopo il quale tutti e 4 i re sono scoperti (media sulle partite in cui succede).

## Risultati

| Preset | Modalità | Media path | Primo path | Path a partita | Doppioni sul tavolo | Buchi iniziali | 4 re al path |
|---|---|---|---|---|---|---|---|
| 32 | Tribuna | 2,37 | 6,01 | 15,2 | 5,0 | 4,0 | 10,8 |
| 32 | Panchina | 2,37 | 6,01 | 15,2 | 5,0 | 4,0 | 10,8 |
| 45 | Tribuna | 2,58 | 9,89 | 15,5 | 8,9 | 0 | 10,5 |
| 45 | Panchina | 2,79 | 9,77 | 17,6 | 8,9 | 0 | 10,6 |
| 60 | Tribuna | 2,39 | 9,35 | 16,7 | 10,3 | 0 | 11,6 |
| 60 | Panchina | 2,06 | 9,25 | 31,1 | 10,3 | 0 | 12,9 |
| 80 | Tribuna | 2,20 | 8,62 | 18,2 | 11,8 | 0 | 12,6 |
| 80 | Panchina | 1,65 | 8,63 | 51,0 | 11,8 | 0 | 15,6 |

## Osservazioni

- **Tribuna**: la lunghezza media dei path cambia poco tra 45 e 80 carte (2,58 → 2,20). Sul tavolo vanno sempre 36 carte non-re + 4 re e le altre si buttano: un mazzo più grande aumenta solo la probabilità di doppioni fra le 36 (da ~9 a ~12). Non è un errore del codice, è l'effetto della regola in prova.
- **Panchina**: con 80 carte i path crollano (1,65 di media, 51 path a partita), perché le carte in eccesso rientrano nei buchi una alla volta.
- **Sotto le 40 carte** (preset 32) panchina e tribuna sono identiche: non ci sono carte in eccesso, solo 4 buchi iniziali.
- Se si vuole che il mazzo grande pesi anche nella tribuna serve un'altra regola (es. i doppioni messi apposta sul tavolo invece che pescati a caso): è una scelta della decisione [Modifica del mazzo](../../wiki/business/modifica-del-mazzo.md), ancora `in analisi`.
