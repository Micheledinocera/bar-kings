---
tipo: decisione
stato: in analisi
aggiornato: 2026-10-22
verificato: 2026-10-20
fonti:
  - raw/compiled/study-tribuna/2026-10-22-discussione-call.md
  - raw/compiled/study-tribuna/2026-10-22-riassunto-call.md
  - raw/compiled/study-rpg/2026-10-20-obiettivo.md
  - raw/compiled/study-rpg/2026-10-20-idee.md
  - raw/compiled/study-rpg/2026-10-20-criticita-e-idee.md
---

# Parte RPG e collegamento con il pathbuilder

Come la parte pathbuilder (tavolo, path, mazzo) e la parte RPG (eventi, combattimento) si parlano, e come è fatta una run. Prime idee del 2026-10-20 (study-rpg): tutte aperte, da vagliare.

## Cosa sappiamo

- **Terminologia** (accettata il 2026-10-20): **run** = l'intera partita alla Balatro (i 3 bar); **incontro** = una partita sul tavolo, finita quando tutte le carte sono girate (per adesso; per la tribuna si propone la fine al 4° re scoperto, vedi [Modifica del mazzo](./modifica-del-mazzo.md#cosa-sappiamo)); **path** = un ciclo.
- **Perché pathbuilder**: alla fine di un ciclo le carte girate formano un path, che triggera **un evento per ogni carta, in sequenza**. Modificare il mazzo orienta la lunghezza dei cicli e quali eventi appaiono.
- **Pathbuilder ↔ RPG**:
  - la costruzione è **tra un incontro e l'altro** (si modifica il mazzo, parte pathbuilder); **durante l'incontro** se ne vedono gli effetti in combattimento (parte RPG);
  - di base "ci si potenzia" con gli eventi e poi si combatte.
- **Eventi** (idea, non decisa):
  - per lo più **parametrici** (scalano con la carta);
  - **non parametrici** (speciali) solo per alcune carte: probabilmente figure, assi e 7 di denari (settebello);
  - **combattimento**: per ora solo i **re**, ma anche altre carte potrebbero triggerare eventi simili.
- **Struttura della run** (idea, non decisa):
  - 3 livelli, i **bar**; ogni bar ha 3 incontri: 2 **scagnozzi** (miniboss) e un **capobar** (boss) che, se battuto, fa passare al bar successivo;
  - ogni bar ha i suoi **malus**: ogni scagnozzo ne applica uno (uno sulla parte RPG, l'altro sulla parte pathbuilder), il capobar li applica entrambi;
  - la parte RPG potrebbe essere un **autobattler** con parametri e skill (vedi l'autobattler "programmabile" in [Meccaniche della run](./meccaniche-della-run.md#opzioni)).

## Opzioni

Idee da valutare, nessuna scelta:

| Idea | Cosa fa | Note |
|---|---|---|
| **Saltare il re** | Se non si è sicuri di vincere il combattimento, si salta il re prendendo un malus nell'incontro | Risposta probabile al rischio di incontrare un re prima di essersi potenziati |
| **Buchi che valgono a fine path** | I buchi contano alla fine del path, così conviene partire dagli slot delle carte tolte dal mazzo | Dà più scelta durante l'incontro (pensata per la panchina) |
| **Condizioni sul path** | Effetti speciali per coppie, tris, scale, ecc. nel path, e i **punteggi della scopa** (settebello, primiera, denari come maggioranza di un seme nei path, scopa come path chiuso in modo pulito), anche come bonus a fine incontro | Rende importante la composizione del path |
| **Ordine degli eventi speciali** | L'ordine nel path conta: uno shop preso dopo dei soldi è diverso dal contrario | Giustifica il "pathbuilder" |
| **Il re interrompe la risoluzione** | Quando un re compare nel path il nemico agisce subito: gli eventi prima del re si applicano, quelli dopo arrivano tardi. Probabilmente vale anche per gli altri eventi speciali | Sì, come direzione |
| **Intento del nemico visibile** | Lo scagnozzo mostra cosa farà al prossimo re (es. "colpisce per 12"), alla Slay the Spire | Dà un motivo per scegliere la carta di partenza |
| **Fiches × moltiplicatore** | Le carte parametriche sommano una base; la lunghezza del path (o i semi consecutivi) moltiplica | Ha senso soprattutto in index>40, dove i cicli si allungano; nelle altre varianti si accorciano |
| **Forme dei buchi come bonus** | Le forme dei buchi della tribuna ([Modifica del mazzo](./modifica-del-mazzo.md#cosa-sappiamo)) danno un bonus | Dalla call del 2026-10-22: bonus a fine incontro, validi per tutta la run (prima l'ipotesi era un bonus solo per quell'incontro) |
| **Malus su gruppi di carte** | Scagnozzi e capobar puniscono gruppi di carte: tutte le carte di un seme, i valori pari o dispari | Esempio di malus sulla parte pathbuilder (call del 2026-10-22); con le forme della tribuna rende rischioso togliere quelle carte. Da annunciare prima dell'incontro |
| **Briscola del bar** | Ogni bar ha un seme di riferimento che ne potenzia le carte e fa da base ai malus | 3 bar e 4 semi: forse un **superboss finale con tutti i semi**, da verificare |
| **Nemici legati ai semi** | Debolezze e resistenze per seme, mostrate in anticipo, così il mazzo si costruisce per il prossimo avversario | Da valutare |

## Alternative scartate

Nessuna alternativa scartata finora.

## Domande aperte

- **Re a caso**: dalla simulazione i 4 re escono tutti intorno al 10°–15° path in panchina e tribuna, già al 3° in index>40 ([Modifica del mazzo](./modifica-del-mazzo.md#cosa-sappiamo)): si può combattere prima di essersi potenziati. Da tenere a mente (vedi *Saltare il re*).
- **Scelta durante l'incontro**: si sceglie solo la carta di partenza, coperta, e in tribuna i path sono in media di circa 2,5 carte: il rischio è guardare il path più che costruirlo. Da tenere a mente (vedi *Buchi che valgono a fine path*).
- **Bilanciamento**: la lunghezza media dei path va da ~2,3 carte (tribuna) a 8–15 (index>40), quindi gli effetti dipendono dalla variante del mazzo, ancora in analisi. Da tenere a mente (proposta: effetti relativi, non numeri fissi, finché la variante non è scelta).
- Da rivedere più avanti: gli HP persistono tra gli incontri o si azzerano? Perdere contro uno scagnozzo chiude la run? Bastano 9 incontri per run (8 momenti di modifica del mazzo)?
- Quali eventi dà ogni carta, parametrici e speciali; come si vince e si perde un combattimento.
- **Momento del fight nel path**: il fight parte quando il re si incontra nella risoluzione in ordine (le carte prima del re si applicano, quelle dopo solo se il re è battuto), oppure a fine path, dopo tutte le carte? Nella scena `TribunaRpg` si prova per ora la prima (utente, 2026-10-28): punto aperto.
