# Implementazione — panchina-reprise

Opzione in prova per la decisione [Modifica del mazzo](../../wiki/business/modifica-del-mazzo.md), che resta `in analisi`: le tre scene (Panchina, Tribuna, Index40) sono le tre alternative che si stanno testando.

## Mazzo oltre le 40 carte

- All'inizializzazione si aggiungono **solo doppioni**: le 36 carte non re più `duplicateCount` doppioni (campo serializzato, default 9: asso–nove di denari, come prima), mescolati.
- Le prime 36 vanno sul tavolo con i 4 re (40 carte, rimescolate); le carte in eccesso (prima venivano scartate) diventano il **mazzo panchina**: restano coperte nel mazzo a `deckPosition`, sotto le carte del tavolo.
- Sul tavolo possono quindi mancare carte (quelle finite in panchina) e esserci doppioni: casi di test voluti.

## Buchi riempiti dalla panchina

- Quando un path si ferma (carta sopra un doppione, o carta finita in un buco), lo slot di partenza resta vuoto: la prima carta della panchina ci va **coperta** (`PathEnd.FilledFromBench`), animata dal mazzo allo slot.
- La carta coperta si gioca come le altre: cliccandola parte un nuovo path.
- Se la panchina è vuota si continua senza riempire: lo slot resta un buco.
- Finché la panchina ha carte i buchi si riempiono subito, quindi il caso "carta che arriva su un buco" capita solo a panchina vuota.

## Codice

- `PanchinaTable`: nuovo mazzo panchina (coda), `BenchCount`, riempimento dello slot di partenza alla fine di `PlayPath`.
- `DeckManagerPanchina`: `duplicateCount`, carte in eccesso alla panchina, mazzo unico con il tavolo sopra e la panchina sotto, animazione della carta dalla panchina; le liste statiche si svuotano a ogni avvio.
- Non deciso, non implementato: la fine della partita.
