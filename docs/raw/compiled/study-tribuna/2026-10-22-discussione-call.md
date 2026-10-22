# Discussione dopo la call — study-tribuna

Riassunto della discussione con l'agent sul [riassunto della call](./2026-10-22-riassunto-call.md). Niente di deciso: sono le regole e le idee **in prova** per la decisione `in analisi` [Modifica del mazzo](../../../wiki/business/modifica-del-mazzo.md).

## Direzione

- Si approfondisce la **tribuna**, senza scartare panchina e index>40. Piace perché non cambia troppo i numeri (la lunghezza dei path resta quasi costante all'aumentare delle carte) e perché ci si incastrano altre meccaniche.
- Con le forme dei buchi la composizione del mazzo diventa la leva principale. Questo risolve il problema del wiki "oltre le 40 carte il mazzo pesa poco".

## Nome delle "forme di buchi"

- Serve un nome per il concetto, al posto della perifrasi "forme di buchi".
- Orientamento: lessico della **tombola** (o del bingo), non degli spalti dello stadio.
- Proposta dell'agent: **vincita** (ambo, terno, quaterna, cinquina e tombola sono le vincite della tombola; "a fine incontro si contano le vincite", il bonus per la run è il premio). Altre idee: **cartella** (il tavolo dei buchi, più che la singola forma), **smorfia** (il catalogo di quale forma vale cosa).
- Da evitare: **figura**. Nelle carte italiane le figure sono fante, cavallo e re.
- Nomi possibili delle singole forme, tutti da tenere: fila (colonna), ola (diagonale), settore (quadrato), e i nomi della tombola (ambo, terno, quaterna, cinquina, tombola).

## Le forme (dalla call)

Il tavolo è una griglia 4×10 per indice: riga = seme (denari, coppe, spade, bastoni), colonna = valore.

- **Colonna**: togliere tutte le carte dello stesso valore (i 4 assi, …). La colonna dei re non si può fare, perché i re ci sono sempre.
- **Diagonale**: togliere 4 carte di valori consecutivi e semi diversi.
- **Quadrato** 2×2 / 3×3 / 4×4: togliere una doppia coppia, 3 tris o 4 poker con gli stessi semi. Il 4×4 sono 16 carte, quasi metà dei 36 non-re.
- Le forme grandi servono anche a bilanciare (vedi *Fine dell'incontro*).

## Le forme non sono rare: sono troppo facili

- Se tolgo una carta dal mazzo (es. l'asso di denari), un path che parte dal suo slot non può chiudersi lì, perché la carta di quello slot non c'è: lo slot diventa un buco. Il buco in quello slot è quindi **certo a priori**.
- Esempio: tolti i 4 assi, con 4 path che partono dagli slot degli assi la colonna viene sempre.

## Fine dell'incontro al 4° re

- In prova per tribuna + forme: l'incontro finisce **appena escono tutti e 4 i re**, non quando sono girate tutte le carte. Così preparare il mazzo per una forma non garantisce di farla.
- Conseguenze notate dall'agent:
  - la forma finale non dipende più solo dal mazzo: conta anche da quali slot e in che ordine si aprono i path, quindi il giocatore ha di nuovo una scelta durante la partita;
  - cade la regola "buchi finali = slot delle carte che mancano dal tavolo", perché a fine incontro restano carte coperte. Va ridefinito cosa conta come buco a fine incontro.
- È la perplessità principale: il 4° re basta come freno? Una forma piccola chiede pochi path (4 per una colonna), e i 4 re potrebbero uscire troppo di rado entro quei path. Nelle simulazioni del 2026-10-16 (partenze casuali) i re escono tutti verso il 10°–15° path. Va misurato (vedi *Simulazioni*).
- Ipotesi dell'agent, da verificare: le forme grandi si bilanciano da sole. Servono più path, e con molte carte tolte ci sono meno non-re sul tavolo, quindi i re escono prima.

## Bonus e malus

- I bonus delle forme si danno **a fine incontro** e valgono **per tutta la run** (legame con [Meccaniche della run](../../../wiki/business/meccaniche-della-run.md)).
- Siccome i buchi diventano centrali, deve succedere **spesso** di poter togliere o trasformare carte del mazzo.
- **Malus dei boss e miniboss**: regole su gruppi di carte, ad esempio tutte le carte di un seme o le carte con valore pari/dispari (legame con [Parte RPG](../../../wiki/business/parte-rpg.md)).
  - Nota dell'agent: anche queste regole sono forme sulla griglia (un seme = una riga, pari/dispari = colonne alterne), quindi bonus e malus possono parlare la stessa lingua: insiemi di slot.
  - Un boss che punisce i buchi su un seme o sui valori pari rende rischioso togliere quelle carte. Per pianificare il mazzo i malus vanno annunciati prima dell'incontro (come i boss di Balatro).

## Modificare il mazzo durante l'incontro (problema aperto dalla call)

- Problema: se un evento dell'incontro cambia la composizione del mazzo (es. una "bomba" che distrugge una carta sul tavolo, girata o coperta), l'effetto vale a fine incontro o subito?
- Proposta dell'agent: separare **tavolo** (stato dell'incontro) e **mazzo** (stato della run). La bomba agisce subito sul tavolo (crea un buco ora, che conta per le forme di questo incontro). Se la carta esce anche dal mazzo della run è una regola separata, applicata a fine incontro o solo da effetti rari "permanenti". Così il deal con il seed resta riproducibile e le simulazioni semplici.

## Domande aperte

1. Nome del concetto: **vincita**, o un altro termine della tombola o del bingo? Quali nomi per le singole forme?
2. Le forme si definiscono come **combinazioni di carte** (valori e semi, in qualsiasi ordine dei semi) o come **disegni sulla griglia** (dipendono dall'ordine delle righe)? Proposta dell'agent: combinazioni di carte, più facili da leggere e indipendenti dall'ordine dei semi.
   - Diagonale: qualsiasi ordine dei 4 semi, o solo quello delle righe (e il suo inverso)?
   - Quadrato: due semi qualsiasi, o solo righe vicine?
3. Le forme si sommano? Il 4×4 contiene 4 colonne e quadrati più piccoli: vale una forma sola (la più grande) o tutte?
4. Con la fine al 4° re, cosa conta come buco a fine incontro: solo gli slot vuoti visibili, o anche uno slot coperto la cui carta si sa che manca?
5. Il 4° re basta a togliere la certezza delle forme piccole? Se no: forme più grandi, malus, altro?
6. Bonus per la run: la stessa forma presa in più incontri si somma? C'è un limite?
7. Malus dei boss: si applicano a fine incontro, come i bonus, o durante? Colpiscono i buchi su quelle carte o le carte scoperte?
8. Un effetto che cambia il mazzo durante l'incontro (la bomba): vale subito sul tavolo, a fine incontro sul mazzo, o tutte e due?
9. La bomba su una carta coperta: il buco è nello slot fisico dove stava la carta, o in quello del suo indice?
10. Togliere o trasformare carte "spesso": quando (dopo ogni incontro, in negozio, come evento) e quanto?

## Prossima feature di codice: preset e simulazioni

Da fare dopo la chiusura di questo studio, in una feature di codice (il nome lo sceglie l'utente). Solo la scena Tribuna. Le regole qui sotto sono in prova e non prendono la decisione.

### Cosa manca oggi nel codice

- I preset oggi dicono solo **quante** carte togliere e duplicare, e come sceglierle (`Random` o le prime N, `First`); i doppioni si spargono, una copia per carta prima di una seconda ([tecnico/deck](../../../wiki/tecnico/deck.md)). **Non si può** scegliere quali carte togliere, né mettere n copie della stessa carta.
- La partita finisce quando non restano carte coperte; non esiste la fine al 4° re.
- L'autoplay parte sempre da uno slot coperto a caso: non c'è un giocatore che punta a una forma.
- Il CSV non conta buchi a fine partita né forme.

### 1. Preset con carte scelte

- Un nuovo modo di scelta delle carte (accanto a `Random` e `First`): **lista esplicita** di carte tolte e di carte duplicate, ognuna con il suo numero di copie. Il tavolo si mescola comunque con il seed.
- Il Preset Builder (`Tools/Preset Builder`) permette di scegliere le carte (es. una griglia 4×10 come il tavolo) e le copie.
- Mazzi di **40 carte o meno**: in tribuna le carte oltre le 40 si buttano, e un originale buttato diventa un buco in più a caso, che sporca la misura. Sotto le 40 le carte mancanti vanno sugli ultimi slot, come oggi.

### 2. Preset da creare

Ogni forma in due versioni: **secca** (solo le carte tolte, meno di 40 carte) e **piena** (tante copie a caso quante le carte tolte, 40 carte), per vedere quanto i doppioni cambiano le cose.

| Preset | Carte tolte | Doppioni | Cosa misura |
|---|---|---|---|
| Copie-2 / Copie-4 / Copie-8 | n−1 carte a caso | 1 carta (es. asso di denari) in n copie in tutto | Cosa succede con n volte la stessa carta (richiesta della call) |
| Colonna | i 4 assi | nessuno / 4 a caso | La forma più piccola: quanto è facile |
| Diagonale | asso di denari, 2 di coppe, 3 di spade, 4 di bastoni | nessuno / 4 a caso | Diagonale sulla griglia |
| Quadrato 2×2 | asso e 2 di denari e di coppe | nessuno / 4 a caso | Doppia coppia |
| Quadrato 3×3 | asso, 2, 3 di denari, coppe e spade | nessuno / 9 a caso | 3 tris |
| Quadrato 4×4 | asso, 2, 3, 4 di tutti i semi | nessuno / 16 a caso | 4 poker: forma grande, bilanciamento con i re |
| Seme | i 9 non-re di denari | nessuno | Riga intera: malus "tutte le carte di un seme" |
| Pari | i 2, 4, 6 di tutti i semi (12 carte) | nessuno | Malus "valori pari" (quali valori contano come pari va scelto: fante 8, cavallo 9?) |

Come confronto: i preset di oggi (32, 45, 60, 71, `Prime18`) con le nuove regole.

### 3. Regole da aggiungere alla simulazione (in prova)

- **Fine dell'incontro**: un'opzione con "nessuna carta coperta" (come oggi) o "4° re scoperto", per confrontarle.
- **Strategia dell'autoplay**: un'opzione con "slot coperto a caso" (come oggi) o "**mirata**": i primi path partono dagli slot delle carte tolte (il giocatore che conosce il suo mazzo), poi a caso.

### 4. Cosa misurare (CSV)

Per ogni preset, fine e strategia:

- path al 4° re e carte ancora coperte a fine incontro;
- buchi a fine incontro (slot vuoti visibili), in totale, per seme (riga) e per valori pari/dispari;
- per ogni forma (colonna, diagonale in tutte e due le definizioni della domanda 2, quadrati 2×2, 3×3, 4×4): % di partite in cui c'è a fine incontro, sia quella "voluta" dal preset sia quelle uscite per caso;
- per la forma voluta: % di partite in cui si completa prima del 4° re, e in quanti path;
- quante volte escono tutti e 4 i re entro k path (k = 1…15): risponde alla domanda 5.
