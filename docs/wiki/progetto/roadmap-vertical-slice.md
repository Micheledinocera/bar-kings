---
tipo: decisione
stato: in analisi
aggiornato: 2026-10-29
verificato: 2026-10-29
fonti:
  - raw/compiled/study-roadmap/2026-10-29-obiettivo.md
  - raw/compiled/study-roadmap/2026-10-29-proposta-roadmap.md
  - raw/compiled/study-roadmap/2026-10-29-delucidazioni.md
---

# Roadmap verso la vertical slice

Ordine delle feature per arrivare a una vertical slice. Perimetro confermato dal team il 2026-10-29; ordine e contenuto delle fasi sono una proposta dell'agent, da rivedere ai punti di decisione. Le idee di santini e amari sono in [Meccaniche della run](../business/meccaniche-della-run.md#idee-di-santini).

## Cosa sappiamo

### Perimetro della slice

Una **run corta ma completa**, giocata **solo dal team**: niente playtester esterni, quindi niente tutorial né build pubblica (team, 2026-10-29).

| Elemento | Nella slice |
|---|---|
| Starter | I **preset del mazzo** (`DeckPresetLibrary`, [tecnico/deck](../tecnico/deck.md#deckpresetlibrary)) scelti a inizio run, come starter diversi; gli starter "filosofia" restano fuori |
| Run | **1 bar** di **3 incontri** (2 scagnozzi e il capobar, [Parte RPG](../business/parte-rpg.md)), collegati da un **percorso** con le tappe tra un incontro e l'altro |
| Tavolo | Tribuna, fine dell'incontro al 4° re, forme dei buchi a fine incontro ([Modifica del mazzo](../business/modifica-del-mazzo.md)) |
| Parte RPG | Quella in prova in `TribunaRpg` ([tecnico/rpg](../tecnico/rpg.md)), con nemici diversi per incontro |
| Santini, amari, shop | Primo set di 5–10 santini e 5–10 amari; la modifica del mazzo passa dagli amari; shop con il pool migliorato dalle forme ([Meccaniche della run](../business/meccaniche-della-run.md)) |
| Malus | Di scagnozzi e capobar su gruppi di carte, annunciati prima dell'incontro |
| Fine | Vittoria battendo il capobar, sconfitta con la vita a 0 |
| Fuori | Secondo e terzo bar, starter "filosofia", imbrogli, carte disegnate, audio definitivo, tutorial, release, panchina e index>40 |

Perché: è il più piccolo pezzo di gioco con il ciclo completo **costruisci il mazzo → gioca l'incontro → vedi gli effetti → ricostruisci**; oggi esiste solo l'incontro.

### Principi

- Prima il ciclo, poi i contenuti: una run grezza che gira, poi santini, amari, shop e malus.
- Ogni feature finisce giocabile; si decide giocando, ai punti di decisione.
- Le decisioni non prese restano in prova: ogni feature dice quale opzione prova ([docs/CLAUDE.md](../../CLAUDE.md), *Decisions*).
- Il bilanciamento passa dalla simulazione, non dalle sensazioni.
- Una scena sola per l'incontro: niente nuovo lavoro sulle scene gemelle.
- Prima le fondamenta comuni (stato ed eventi della run), poi i contenuti in parallelo, anche da persone diverse (team, 2026-10-29).

### Fasi

| Fase | Feature | Cosa fa | Known issues | Dopo |
|---|---|---|---|---|
| 1 | `encounter-scene` | La parte RPG di `TribunaRpg` dentro la scena `Bar`: statistiche al posto dei segnaposto, popup del combattimento nello stile | D, J | — |
| 2 | `run-loop` | Stato della run in C# senza MonoBehaviour (mazzo, statistiche, incontro, inventario) tra gli incontri; scelta del preset-starter; percorso scagnozzo → tappa → scagnozzo → tappa → capobar con `KingConfig` per incontro; vittoria e sconfitta; primi test Edit Mode | C | 1 |
| 2 | `run-events` | Eventi della run per santini, amari e malus (carta risolta, path chiuso, re incontrato, colpo, combattimento vinto, incontro finito, forma fatta, tappa raggiunta, Lire spese); operazioni sul mazzo (togli, duplica, scambia due semi, due valori, due carte); schermata di debug | — | `run-loop` |
| 3 | `rpg-simulation` | Autoplay della run con effetti e combattimenti; CSV con vittorie per incontro, vita residua, re battuti | H | `run-loop` |
| A | punto di decisione | Tribuna per la slice; regole base dell'incontro (fine al 4° re, vita tra gli incontri, momento del combattimento); **quando si bevono gli amari** | — | 2, 3 |
| 4 | `santini` | Slot e primo set, agganciati agli eventi della run; ScriptableObject creati da un menu dell'Editor (`AGENTS.md`, *Data assets*) | — | A |
| 4 | `amari` | Slot e primo set; quelli di modifica del mazzo usano le operazioni di `run-events`; ScriptableObject come i santini | — | A |
| 4 | `shop` | Tappa che vende santini e amari con le Lire; pool migliorato dalle forme (4×4 = santino leggendario); può partire con contenuti finti | — | A |
| 4 | `boss-malus` | Malus su gruppi di carte annunciati sul percorso; il capobar somma quelli degli scagnozzi; eventuale intento del nemico | — | A |
| B | punto di decisione | Quali regole di Parte RPG e Meccaniche della run diventano `presa` per la slice; quali contenuti tenere | — | 4 |
| 5 | `cleanup-twin-scenes` | Se la tribuna è scelta: eliminare `Panchina`, `Tribuna`, `Index40`, `TribunaRpg` e il codice usato solo da loro | F | A |
| 5 | `juice` | Animazioni e feedback (DOTween) su path, combattimento, forme, santini e amari; suoni segnaposto | — | B |
| 5 | `study-slice-review` | Il team gioca la slice (build Windows di oggi, nessuna release) e decide il dopo: secondo bar, starter "filosofia", carte disegnate, playtester esterni | — | 5 |

Le quattro feature della fase 4 vanno in parallelo; l'unico punto condiviso è la HUD della scena dell'incontro, da non toccare con due feature insieme ([Branching, commit e pull request](./git-flow-e-convenzioni.md)).

### Decisioni del radar

| Decisione | Quando chiuderla per la slice |
|---|---|
| [Modifica del mazzo](../business/modifica-del-mazzo.md) | Punto A (tribuna); bonus delle forme al punto B |
| [Parte RPG](../business/parte-rpg.md) | Regole base al punto A; malus e intento al punto B; eventi speciali dopo la slice |
| [Meccaniche della run](../business/meccaniche-della-run.md) | Momento degli amari al punto A; santini, amari e shop al punto B |
| [Direzione artistica](../arte/direzione-artistica.md) | Dopo la slice; si continua con lo stile della scena Bar |
| [Tipi di ramo e release](./tipi-di-ramo-e-release.md) | Per ora solo feature; le release più avanti |

### Rischi

- **Re troppo presto** (tutti e 4 verso il 10°–15° path in tribuna): si combatte prima di potenziarsi. Lo misura la fase 3; risposte in studio: saltare il re, intento visibile, santini come San Pietro.
- **Poca scelta durante l'incontro** (solo la carta di partenza, path di ~2,5 carte): se la run non diverte al punto A, il problema è qui e non nei contenuti; gli amari bevuti durante l'incontro sarebbero una risposta.
- **Forme troppo facili giocando mirato** (73–79%): premi da tarare con la simulazione.
- **Stato tra le scene**: senza il blocco C la run su più incontri si rompe; per questo è dentro `run-loop`.
- **Fase 4 in parallelo**: regge solo se `run-events` fissa bene eventi e operazioni; un evento mancante scoperto dopo tocca quattro feature.

## Opzioni

Nessuna alternativa all'ordine proposto è stata discussa; si rivede ai punti di decisione A e B.

## Alternative scartate

- **Slice per playtester esterni, con tutorial e release `release/0.1.0` verso `main`** — scartata il 2026-10-29: la slice serve solo al team, le release si fanno più avanti (fonte: raw/compiled/study-roadmap/2026-10-29-delucidazioni.md)
- **Shop e malus come unico parallelo, santini e amari dentro lo shop** — scartata il 2026-10-29: anche santini e amari vanno in parallelo, come feature a sé (fonte: raw/compiled/study-roadmap/2026-10-29-delucidazioni.md)

## Domande aperte

- Il percorso del bar è lineare (incontro, tappa, incontro…) o con bivi?
- Quali preset fanno da starter nella slice, e con che nome?
- Chi lavora su quale delle quattro feature della fase 4?
- Il team approva l'ordine delle fasi così com'è?
