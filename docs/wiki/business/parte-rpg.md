---
tipo: decisione
stato: in analisi
aggiornato: 2026-11-06
verificato: 2026-10-20
consolidato: 2026-10-29
fonti:
  - raw/compiled/events-impl/2026-11-06-obiettivo.md
  - raw/compiled/events-impl/2026-11-06-task.md
  - raw/compiled/events-impl/2026-11-06-idee-eventi.md
  - raw/compiled/study-roadmap/2026-10-29-delucidazioni.md
  - raw/compiled/merge-rpg/2026-10-28-obiettivo-rpg.md
  - raw/compiled/merge-rpg/2026-10-28-fight.md
  - raw/compiled/merge-rpg/2026-10-28-popup-e-config.md
  - raw/compiled/merge-rpg/2026-10-28-riassunto-sessione.md
  - raw/compiled/merge-rpg/2026-10-29-obiettivo.md
  - raw/compiled/santini-and-amari-first-implementation/2026-11-05-obiettivo.md
  - raw/compiled/santini-and-amari-first-implementation/2026-11-05-sessione.md
  - raw/compiled/study-call-28-10-2026/2026-10-28-note-call.md
  - raw/compiled/study-tribuna/2026-10-22-discussione-call.md
  - raw/compiled/study-tribuna/2026-10-22-riassunto-call.md
  - raw/compiled/study-rpg/2026-10-20-obiettivo.md
  - raw/compiled/study-rpg/2026-10-20-idee.md
  - raw/compiled/study-rpg/2026-10-20-criticita-e-idee.md
  - raw/compiled/study-05-11-2026/2026-11-05-recap-call.md
---

# Parte RPG e collegamento con il pathbuilder

Come la parte pathbuilder (tavolo, path, mazzo) e la parte RPG (eventi, combattimento) si parlano, e come è fatta una run. Prime idee del 2026-10-20 (study-rpg): tutte aperte, da vagliare.

## Cosa sappiamo

- **Prima implementazione in prova** (2026-10-28, scena `TribunaRpg`, copia di Tribuna; codice in [RPG scripts](../tecnico/rpg.md)). Niente di questo è deciso: sono le regole che si stanno provando.
  - **Statistiche del giocatore**: danno, vita (con un massimo), armatura, denari (Lire). Partenza: vita 100 su 100, danno 1, armatura 1, modificabili dall'Inspector.
  - **Effetti delle carte 1–9** (n = valore), alla fine di ogni path, nell'ordine del path: Spade +n danno, Denari +n×1000 Lire, Coppe +n vita (oltre il massimo non ha effetto), Bastoni +n armatura. La carta che arriva dalla panchina non conta.
  - **Re = combattimento**: tutti i re uguali (vita 10, armatura 0, danno 1), nuovi a ogni combattimento. Turni scanditi da un clock non a tempo, uno a testa per giro, prima il giocatore. Colpo = danno − armatura del bersaglio (minimo 0); l'armatura non si consuma.
  - **Momento**: il combattimento parte quando il re si incontra nel path (vedi l'opzione *Il re interrompe la risoluzione*): le carte prima si applicano, quelle dopo solo se il re è battuto.
  - **Sconfitta** (vita a 0): l'incontro finisce, le carte dopo il re non si applicano. **Pareggio**: se nessuno dei due passa l'armatura dell'altro, il combattimento finisce subito e si va avanti (proposta dell'agent, accettata dall'utente il 2026-10-29).
  - Un popup riassume ogni combattimento e va chiuso per continuare.
  - **Vita tra i combattimenti** (2026-11-05, solo per la prova): le statistiche valgono per tutto l'incontro, quindi la vita resta da un combattimento all'altro; solo il Reset la riporta ai valori iniziali. Tra gli incontri resta una domanda aperta.
  - **Santini e amari** in prova sopra queste regole: vedi [Meccaniche della run](./meccaniche-della-run.md#cosa-sappiamo).
- **Seconda prova: gli eventi delle carte** (2026-11-06, feature events-impl, scena `Bar-Michele`, copia di Bar con la parte RPG; codice in [Bar-Michele scripts](../tecnico/bar-michele.md)). Scena di prova "randomica" di Michele: le idee che funzionano andranno poi riunite con quelle di altri rami in una feature definitiva. Regole in prova, non decise (l'utente le ha fissate "per adesso"):
  - **Risoluzione carta per carta**: ogni carta si applica appena si gira, mentre il path va avanti; quando arriva un re il path si ferma sulla sua scelta e riprende dopo.
  - **Combattimento durante il path**; si **vince** quando il re perde tutta la vita, si **perde** quando la vita del giocatore va a 0. Con la sconfitta **si interrompe subito tutto**, anche il path in corso, e un pannello dice "Sconfitto!" e di premere Reset. Il pareggio resta nel codice ma con i numeri di oggi non capita.
  - **Carte da 1 a 7: parametriche**, con gli stessi effetti per seme della prima prova.
  - **7**: dà la sua statistica e conta per il **bonus dei quattro 7**: al quarto 7 girato nell'incontro (anche in path diversi) +14 danno, +14 armatura, cura 14 vita, +14.000 Lire (il doppio di quanto danno da soli; bonus provvisorio).
  - **8 (fante)**: popup "Entra nello shop" / "Salta". Lo shop non fa ancora niente; saltandolo si prende **un quarto delle Lire guadagnabili in una partita** (denari da 1 a 7 = 28.000, quindi 7.000 Lire, santini esclusi). Più avanti lo shop potrebbe dipendere dal seme.
  - **9 (cavallo): fuga**. Non dà statistiche, solo una fuga (due con San Francesco); le fughe si accumulano per l'incontro e Reset le azzera.
  - **Re: sempre la scelta** "Scappa" / "Combatti". Con una fuga si scappa gratis; **senza fughe si scappa pagando un malus**, per ora il **25% delle Lire** possedute (provvisorio). Il re da cui si scappa è perso ma conta tra i 4 re scoperti. È l'opzione *Saltare il re* in prova.
  - **Tutte le carte girate**: se dopo un path non resta nessuna carta coperta lo si notifica con il pannello di fine incontro. Con la fine al quarto re succede solo se le ultime carte coperte escono nello stesso path del quarto re.
  - **Interfaccia**: statistiche del giocatore (vita, danno, armatura, fughe, Lire, sette) in un pannello, scontrino dei path consultabile path per path e scorrevole.
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
  - 3 livelli, i **bar**; ogni bar ha 3 incontri: 2 **scagnozzi** (miniboss) e un **capobar** (boss) che, se battuto, fa passare al bar successivo; la [vertical slice](../progetto/roadmap-vertical-slice.md) ne fa uno solo (team, 2026-10-29);
  - ogni bar ha i suoi **malus**: ogni scagnozzo ne applica uno (uno sulla parte RPG, l'altro sulla parte pathbuilder), il capobar li applica entrambi;
  - la parte RPG potrebbe essere un **autobattler** con parametri e skill (vedi l'autobattler "programmabile" in [Meccaniche della run](./meccaniche-della-run.md#opzioni)).

## Opzioni

Idee da valutare, nessuna scelta:

| Idea | Cosa fa | Note |
|---|---|---|
| **Saltare il re** | Se non si è sicuri di vincere il combattimento, si salta il re prendendo un malus nell'incontro | Risposta probabile al rischio di incontrare un re prima di essersi potenziati. **In prova** in `Bar-Michele` (2026-11-06): fuga gratis con i cavalli, altrimenti −25% delle Lire. Malus valutati e non scelti: −10% della vita massima, un colpo del re mentre si scappa, il re rimandato più avanti |
| **Eventi del cavallo (9)** | Idee a tema cavallo (2026-11-06): **velocità** (un turno in più o il primo colpo contro il prossimo re), **fuga** (salta il prossimo combattimento), **sbirciata** (scopre per un attimo le carte coperte di riga o colonna), **salto** (il path salta la prossima carta, che resta coperta), **carica** (doppio danno al prossimo re dello stesso path), **cavallo come forma** (effetto con due cavalli nello stesso path o riga) | Piacciono velocità, fuga e salto; la **fuga è in prova**; il **salto** è da tenere in considerazione se servirà allungare o controllare i path. Lo scambio di due indici, prima idea dell'utente, non è stato scelto: chiede una scelta di due posizioni a metà path e si sovrappone alle operazioni sul mazzo di amari e shop |
| **Bonus dei quattro 7** | I 7 contano per un bonus quando escono tutti e quattro | **In prova** nell'incontro (vedi *Cosa sappiamo*). Valutati e non scelti: i 7 senza statistica propria; il bonus solo con i quattro 7 nello stesso path (troppo raro); come bonus vita piena più una fuga, o un santino gratis |
| **Shop sull'8** | Il fante apre uno shop, oppure lo si salta per delle Lire | **In prova** (shop ancora vuoto). Idea: legare lo shop al seme del fante |
| **Buchi che valgono a fine path** | I buchi contano alla fine del path, così conviene partire dagli slot delle carte tolte dal mazzo | Dà più scelta durante l'incontro (pensata per la panchina) |
| **Condizioni sul path** | Effetti speciali per coppie, tris, scale, ecc. nel path, e i **punteggi della scopa** (settebello, primiera, denari come maggioranza di un seme nei path, scopa come path chiuso in modo pulito), anche come bonus a fine incontro | Rende importante la composizione del path |
| **Ordine degli eventi speciali** | L'ordine nel path conta: uno shop preso dopo dei soldi è diverso dal contrario | Giustifica il "pathbuilder" |
| **Il re interrompe la risoluzione** | Quando un re compare nel path il nemico agisce subito: gli eventi prima del re si applicano, quelli dopo arrivano tardi. Probabilmente vale anche per gli altri eventi speciali | Sì, come direzione; **in prova** nella scena `TribunaRpg` |
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
- **Bilanciamento**: la lunghezza media dei path va da ~2,3 carte (tribuna) a 8–15 (index>40), quindi gli effetti dipendono dalla variante del mazzo, ancora in analisi. Da tenere a mente (proposta: effetti relativi, non numeri fissi, finché la variante non è scelta). L'utente (2026-11-06) è d'accordo: niente numeri fissi ma legati a un parametro, ad esempio la lunghezza del path, il livello della tappa, malus e bonus; da vedere più avanti.
- **Malus della fuga senza cavalli**: per ora −25% delle Lire, "da capire".
- **Shop**: cosa si compra e se dipende dal seme del fante.
- **Righe e colonne sul tavolo**: l'utente ha chiesto di segnarle nella UI, in vista di eventi che scambiano gli indici; non ancora fatto.
- Da rivedere più avanti: gli HP persistono tra gli incontri o si azzerano? Perdere contro uno scagnozzo chiude la run? Bastano 9 incontri per run (8 momenti di modifica del mazzo)?
- Quali eventi dà ogni carta, parametrici e speciali; come si vince e si perde un combattimento. In prova: effetti per seme e combattimento al re (vedi *Cosa sappiamo*); in `Bar-Michele` anche 7, 8 e 9 speciali, vittoria e sconfitta a vita 0 (vedi *Seconda prova*). Restano da pensare gli assi e le altre figure. Dalla call del 2026-11-05 ognuno porta alla call successiva idee per le **36 carte evento** e, facoltativo, per un flusso **bar + incontro (modifica del mazzo e/o disposizione delle carte) + bar** e per le forme. Nota dell'agent: in quel flusso "incontro" sembra la tappa tra due bar, non la partita sul tavolo del [glossario](./concept.md#glossario): da chiarire.
- **Momento del combattimento nel path**: quando il re si incontra nella risoluzione (in prova) o a fine path, dopo tutte le carte? Punto aperto (utente, 2026-10-28); il 2026-11-06 l'utente lo tiene durante il path "per adesso".
- **Velocità**: una statistica che dà più turni prima di quello del re. Idea per dopo, non implementata; è anche uno degli eventi del cavallo che piacciono (vedi *Opzioni*).
