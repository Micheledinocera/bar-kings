# Vincoli e proposta — path-length-control

## Perché il primo path è lungo

Con il mazzo non alterato (preset `Standard`, quello con cui partono oggi Tribuna, Bar e TribunaRpg) ogni carta ha il suo slot e non ci sono buchi né doppioni: il path è il ciclo della carta scelta, e la sua lunghezza è uniforme tra 1 e 40. Media del primo path 20,6 carte, il 66% delle partite ha un primo path di 15 carte o più.

## Simulazione (20.000 partite, mazzo non alterato, script temporaneo fuori dal repo)

| Impostazione | Primo path medio | Primo path ≥ 15 | Path a partita | Lunghezza media | Path più lungo | Path del 4° re |
|---|---|---|---|---|---|---|
| Spento (oggi) | 20,6 | 66% | 4,3 | 9,4 | 25,3 | 2,8 |
| N=8, +5% a carta | 11,2 | 20% | 5,8 | 6,9 | 14,1 | 4,2 |
| N=8, +10% a carta | 10,0 | 4% | 6,2 | 6,5 | 12,5 | 4,4 |
| N=6, +15% a carta | 8,1 | 0% | 7,1 | 5,7 | 10,0 | 5,2 |
| N=8, +10%, solo primi 3 path | 10,1 | 4% | 6,0 | 6,7 | 13,0 | 4,4 |

Le carte girate in totale non cambiano: aumentano i path e si accorciano. Nel Bar l'incontro (fine al 4° re) dura più path, i re si distribuiscono su più path (impatto sul bilanciamento RPG). Con impostazioni troppo forti (es. N=5, +25%) i path finiscono quasi sempre intorno a 8 carte: la regolarità si nota.

## Vincoli dati dall'utente (2026-11-04)

- Lo sviluppo prosegue con la **tribuna**.
- Oggi c'è un solo incontro; in futuro più bar di fila, ognuno con più incontri. L'inganno vale per un **numero fisso di incontri (configurabile)**, perché ci si aspetta che il mazzo venga alterato durante la progressione.
- **Nessuna lunghezza minima**: path molto brevi possono essere voluti dal giocatore.
- La modificabilità del mazzo è un argomento di sviluppo successivo.

## Proposta accettata

- Da N carte del path in poi, a ogni carta la probabilità di chiudere cresce di un incremento configurabile; quando scatta, la carta coperta dello slot target viene scambiata con la carta coperta che appartiene allo slot di partenza (lo scambio tra due carte coperte è invisibile).
- Nessun limite "primi M path" dentro l'incontro: in simulazione cambia poco, dopo i primi path i cicli rimasti sono già corti.
- Caso dal seed della partita, così un seed rigiocato dà la stessa partita.
- Se la carta che chiude non è disponibile (tolta dal mazzo o già scoperta) l'inganno non fa nulla.
- Configurazione (N, incremento, numero di incontri) in uno ScriptableObject; l'incontro corrente oggi è sempre il primo.
- Attivo in Tribuna, Bar e TribunaRpg; spento in Panchina e Index40.
- Limite da ricordare: le idee future che rivelano carte coperte (peek, Santa Lucia) dovranno escludere quelle carte dallo scambio.
