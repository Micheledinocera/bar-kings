# Roadmap verso la vertical slice

Proposta dell'agent dal radar del wiki al 2026-10-29 e dallo stato del codice (scene Bar e TribunaRpg, simulazione, known issues), rivista con le risposte del team ([delucidazioni](./2026-10-29-delucidazioni.md)). Idee per santini e amari in [idee-santini-amari](./2026-10-29-idee-santini-amari.md).

## Cos'è la vertical slice

Una **run corta ma completa**, giocata **dal team** (non da playtester esterni: niente tutorial né build pubblica), con lo stile della scena Bar. Perimetro confermato dal team il 2026-10-29.

- **Starter = preset del mazzo**: a inizio run si sceglie uno dei preset della libreria (`DeckPresetLibrary`), che fa da starter diverso. Gli starter "filosofia" di [Meccaniche della run](../../wiki/business/meccaniche-della-run.md) restano fuori.
- **1 bar** con **3 incontri**: 2 scagnozzi e il capobar ([Parte RPG](../../wiki/business/parte-rpg.md)), collegati da un **percorso** con le tappe tra un incontro e l'altro (shop, modifica del mazzo).
- **Tavolo**: tribuna, fine dell'incontro al 4° re, forme dei buchi contate a fine incontro.
- **Parte RPG**: quella in prova in `TribunaRpg` (effetti per seme, combattimento al re), con nemici diversi per incontro.
- **Santini**: effetti passivi che scattano su eventi della parte RPG, del tavolo o del percorso.
- **Amari**: consumabili; la **modifica del mazzo** (togliere, duplicare, scambiare indici) passa da qui. Da decidere se si bevono durante il combattimento o tra un combattimento e l'altro.
- **Shop** tra gli incontri, con il pool migliorato dalle forme fatte.
- **Malus** di scagnozzi e capobar su gruppi di carte, annunciati prima dell'incontro.
- **Fine**: vittoria battendo il capobar, sconfitta se la vita va a 0.
- **Fuori**: secondo e terzo bar, starter "filosofia", imbrogli, carte disegnate originali, audio definitivo, tutorial, panchina e index>40.

Perché così: è il più piccolo pezzo di gioco in cui si vede il ciclo completo **costruisci il mazzo → gioca l'incontro → vedi gli effetti → ricostruisci**, la tesi del pathbuilder. Oggi esiste solo "gioca l'incontro".

## Principi

- **Prima il ciclo, poi i contenuti**: prima una run che gira con regole grezze, poi santini, amari, shop e malus. Pochi contenuti (5–10 per tipo) bastano a capire se il ciclo regge.
- **Ogni feature finisce giocabile** e dà qualcosa da provare in call: si decide giocando.
- **Le decisioni non prese restano in prova** (regola del wiki): ogni feature dice quale opzione prova; si decide ai *punti di decisione*.
- **Simulazione presto**: vita, danno e re si bilanciano con l'autoplay, non a mano.
- **Una scena sola per l'incontro**: si smette di sviluppare sulle scene gemelle.
- **Prima le fondamenta comuni, poi in parallelo**: shop, santini, amari e malus si sviluppano in parallelo (anche da persone diverse, confermato dal team), ma tutti poggiano sullo stato della run e sugli eventi della run: quelli vengono prima.

## Le fasi, in ordine

### Fase 1 — L'incontro (una feature)

| Feature | Cosa fa | Decisioni in prova |
|---|---|---|
| `encounter-scene` | **Un'unica scena dell'incontro**: la parte RPG di `TribunaRpg` dentro la scena `Bar` (vita, danno, armatura e Lire al posto dei segnaposto, popup del combattimento nello stile). Insieme i known issues di questa scena: blocco **D** (click sotto la UI, già emerso col popup) e blocco **J** (HUD del Bar). | Parte RPG (momento del combattimento), Direzione artistica (scena divisa in due) |

### Fase 2 — La run (due feature, in sequenza)

| Feature | Cosa fa | Decisioni in prova |
|---|---|---|
| `run-loop` | **Stato della run** in una classe C# senza MonoBehaviour (mazzo, statistiche del giocatore, incontro corrente, inventario) che resta tra gli incontri; **scelta dello starter** (preset) a inizio run; **percorso** del bar: scagnozzo → tappa → scagnozzo → tappa → capobar, con re configurati per incontro (`KingConfig` diversi); vittoria e sconfitta; Menu → run → Menu. Insieme il blocco **C** (stato statico e nella gerarchia), che diventa un problema appena lo stato vive su più incontri. Primi test Edit Mode sullo stato della run. | Parte RPG (vita tra gli incontri, sconfitta = fine run), Meccaniche della run (struttura della run) |
| `run-events` | **Eventi della run** a cui si agganciano santini, amari e malus: carta risolta, path chiuso, re incontrato, colpo dato e ricevuto, combattimento vinto, incontro finito, forma fatta, tappa del percorso raggiunta, Lire spese. **Operazioni sul mazzo** nel modello della run: togli, duplica, scambia due semi, due valori o due carte (le azioni sull'indice della call del 2026-10-28). Una schermata di debug per lanciarle a mano. | Modifica del mazzo (azioni sull'indice) |

### Fase 3 — Bilanciamento di base (in parallelo alla fase 2, dopo `run-loop`)

| Feature | Cosa fa | Decisioni in prova |
|---|---|---|
| `rpg-simulation` | **Autoplay della run**: `GameSimulator` esteso agli effetti delle carte e ai combattimenti, CSV con vittorie e sconfitte per incontro, vita residua, re battuti. Insieme il blocco **H**. | Parte RPG (numeri), Modifica del mazzo |

**Punto di decisione A** (call): giocata la run grezza con le operazioni sul mazzo, si **sceglie la tribuna** per la slice (o no) e si fissano le regole base dell'incontro (fine al 4° re, vita tra gli incontri, momento del combattimento). Si decide anche **quando si bevono gli amari** (durante il combattimento, tra un combattimento e l'altro, o entrambi a seconda dell'amaro), perché cambia come si costruisce la fase 4.

### Fase 4 — I contenuti (quattro feature in parallelo)

Indipendenti tra loro dopo la fase 2. Ognuna ha la sua schermata o il suo sistema; l'unico punto condiviso è la scena dell'incontro, che non va toccata da due feature insieme (`CONTRIBUTING.md`): santini e malus vi aggiungono solo indicatori nella HUD, da mettere d'accordo prima.

| Feature | Cosa fa | Decisioni in prova |
|---|---|---|
| `santini` | Slot fissi, primo set di 5–10 santini dalla [lista di idee](./2026-10-29-idee-santini-amari.md), ognuno agganciato a un evento della run; ScriptableObject in `Assets/Instances/Santini/`. | Meccaniche della run (santini, numero di slot) |
| `amari` | Slot massimi, primo set di 5–10 amari; quelli di **modifica del mazzo** usano le operazioni di `run-events`. Momento del consumo come scelto al punto A. ScriptableObject in `Assets/Instances/Amari/`. | Meccaniche della run (amari), Modifica del mazzo |
| `shop` | Tappa del percorso che vende santini e amari con le Lire della parte RPG; il pool migliora con le forme fatte (il 4×4 garantisce un santino leggendario). Si può partire con santini e amari finti, sostituiti quando le altre due feature arrivano. | Meccaniche della run (shop legato alle forme), Modifica del mazzo (bonus delle forme) |
| `boss-malus` | Malus di scagnozzi e capobar su gruppi di carte (un seme, pari/dispari), annunciati sul percorso prima dell'incontro; il capobar applica quelli di entrambi gli scagnozzi. Eventuale **intento del nemico visibile**. | Parte RPG (malus, intento) |

**Punto di decisione B** (call + partite del team): sulla run con santini, amari, shop e malus si decide quali regole di Parte RPG e Meccaniche della run diventano `presa` per la slice, e quali contenuti tenere.

### Fase 5 — Chiusura della slice

| Feature | Cosa fa | Decisioni in prova |
|---|---|---|
| `cleanup-twin-scenes` | Se la tribuna è scelta (punto A): **eliminare** `Panchina`, `Tribuna`, `Index40`, `TribunaRpg` e il codice usato solo da loro (regola delle scene gemelle in `AGENTS.md`); blocco **F**. Può partire subito dopo il punto A. | Modifica del mazzo (`presa`) |
| `juice` | **Animazioni e feedback** alla Balatro/Broom (DOTween) sui momenti che contano: path, combattimento, forme fatte, santino che scatta, amaro bevuto. Effetti sonori segnaposto. Niente tutorial: la slice è per il team. | Direzione artistica |
| `study-slice-review` | Il team gioca la slice (build Windows dal menu che c'è già, nessuna release) e decide cosa viene dopo: secondo bar, starter "filosofia", carte disegnate, playtester esterni. | tutte |

## Le decisioni del radar

| Decisione | Stato | Cosa serve per la slice | Quando chiuderla |
|---|---|---|---|
| [Modifica del mazzo](../../wiki/business/modifica-del-mazzo.md) | in analisi | Tribuna, fine al 4° re, forme; operazioni sul mazzo tramite gli amari | **Punto A**: tribuna `presa` per la slice; bonus delle forme al punto B |
| [Parte RPG](../../wiki/business/parte-rpg.md) | in analisi | 1 bar da 3 incontri, re diversi per incontro, vita tra gli incontri, sconfitta = fine run, malus | Regole base al **punto A**, malus e intento al **punto B**; eventi speciali (figure, assi, settebello) dopo la slice |
| [Meccaniche della run](../../wiki/business/meccaniche-della-run.md) | in analisi | Starter come preset, santini, amari, shop; starter "filosofia" e imbrogli fuori | Momento degli amari al **punto A**; santini, amari e shop al **punto B** |
| [Direzione artistica](../../wiki/arte/direzione-artistica.md) | in analisi | Si continua con lo stile della scena Bar (palette Nicotina, carte napoletane) | Dopo la slice (`study-slice-review`); chi produce gli asset definitivi va deciso prima di disegnare le carte |
| [Tipi di ramo e release](../../wiki/progetto/tipi-di-ramo-e-release.md) | aperta | Niente: la slice non ha release | **Per ora solo feature** (team, 2026-10-29), fix compresi; il flusso di release si decide più avanti, quando serve una build per qualcuno fuori dal team |

## Rischi da tenere d'occhio

- **Re che escono troppo presto** (simulazione: tutti i 4 re verso il 10°–15° path in tribuna): si combatte prima di essersi potenziati. La fase 3 dice quanto è grave; risposte già in studio: saltare il re, intento visibile, e qualche santino (es. San Pietro nella lista di idee).
- **Poca scelta durante l'incontro** (si sceglie solo la carta di partenza, path medi di ~2,5 carte): se la run non diverte già al punto A, il problema è qui e non nei contenuti. Gli amari bevuti durante il combattimento sarebbero una risposta.
- **Forme troppo facili per chi gioca mirato** (73–79%): il premio delle forme va tarato con la simulazione, non a sensazione.
- **Stato condiviso tra scene**: senza il blocco C sistemato la run su più incontri si rompe; per questo C è dentro `run-loop`.
- **Quattro feature in parallelo**: reggono solo se `run-events` fissa bene gli eventi e le operazioni sul mazzo; un evento mancante scoperto dopo tocca tutte e quattro.

## Domande aperte

- Amari: si bevono durante il combattimento, tra un combattimento e l'altro, o dipende dall'amaro? (da decidere al punto A)
- Quali preset fanno da starter nella slice, e con che nome?
- Il percorso del bar è lineare (incontro, tappa, incontro…) o con bivi tra cui scegliere?
- Chi lavora su quale delle quattro feature della fase 4?
