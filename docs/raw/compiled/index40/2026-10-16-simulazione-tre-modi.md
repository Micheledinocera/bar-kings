# Simulazione dei tre modi — panchina, tribuna, index>40

2026-10-16. Prima simulazione con le regole di index>40, i buchi in fondo per panchina e tribuna e `Preset71` (ex `Preset80`: 5 tolte, 40 doppioni). Sostituisce, per il preset 71 e per i buchi del preset 32, i numeri di `raw/compiled/stats/2026-10-16-simulazione-preset.md`.

## Come

- 1000 partite per preset e per modo, seed 12345; autoplay con slot di partenza casuale (strategia di prova).
- Lunghezza di un path = carte spostate; "4 re al path" = path dopo il quale i 4 re sono tutti scoperti.

## Risultati

| Preset | Modo | Path a partita | Media path | Primo path | Path più lungo | % path chiusi | 4 re al path |
|---|---|---|---|---|---|---|---|
| 32 | Panchina | 15,5 | 2,32 | 5,6 | 8,7 | 9,1 | 11,1 |
| 32 | Tribuna | 15,5 | 2,32 | 5,6 | 8,7 | 9,1 | 11,1 |
| 32 | Index | 4,1 | 8,82 | 18,5 | 23,0 | 100 | 2,8 |
| 45 | Panchina | 17,7 | 2,77 | 10,2 | 12,4 | 11,6 | 10,6 |
| 45 | Tribuna | 15,5 | 2,58 | 9,8 | 11,8 | 9,3 | 10,5 |
| 45 | Index | 4,6 | 10,76 | 25,6 | 31,1 | 100 | 2,9 |
| 60 | Panchina | 31,1 | 2,06 | 9,5 | 11,6 | 6,2 | 12,8 |
| 60 | Tribuna | 16,8 | 2,39 | 9,4 | 11,1 | 7,8 | 11,5 |
| 60 | Index | 4,8 | 13,36 | 32,2 | 40,6 | 100 | 2,9 |
| 71 | Panchina | 42,0 | 1,79 | 8,8 | 10,8 | 4,6 | 14,8 |
| 71 | Tribuna | 17,6 | 2,28 | 8,9 | 10,6 | 6,7 | 12,3 |
| 71 | Index | 4,9 | 15,20 | 37,5 | 47,4 | 100 | 2,9 |

## Osservazioni

- **Index>40: ogni path si chiude**. Ogni carta ha un indice suo, quindi non incontra mai un doppione scoperto; gli slot delle carte tolte restano buchi ma nessuna carta del mazzo ha quell'indice, quindi non ci arriva mai nessuno. Il tavolo è una permutazione: pochi cicli lunghi (4–5 a partita), più lunghi col mazzo più grande. Conferma l'"effetto: li allunga" della tabella delle opzioni.
- In index>40 i 4 re sono tutti scoperti già intorno al 3° path, contro il 10°–15° di panchina e tribuna.
- Panchina e tribuna come prima: la panchina accorcia molto i path con il mazzo grande, la tribuna poco.
