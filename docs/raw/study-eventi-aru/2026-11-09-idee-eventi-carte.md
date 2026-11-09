# Idee Meccaniche ed Eventi Carte — study-eventi-aru

Documento di lavoro e brainstorming per definire il comportamento, le regole e gli effetti delle carte (36 carte evento + 4 re) durante la risoluzione del path e il collegamento con la parte RPG.

---

## 1. Visione d'insieme e Principi Guida

- **Risoluzione del path**: chiuso il ciclo sul tavolo (ritorno alla carta di partenza), il path viene percorso carta per carta. Ogni carta scatena un evento/effetto.
- **Rapporto tra Tavolo e RPG**: il tavolo (solitario dei 4 re) è il motore di generazione del path; la parte RPG (statistiche, incontri, boss/re, shop) è il luogo in cui gli effetti si manifestano.
- **Parametrico vs Fissato**: preferire formule scalari (lunghezza path, livello dello stage, moltiplicatori) rispetto a valori numerici rigidi e statici, per facilitare il bilanciamento tra le diverse varianti del mazzo.

---

## 2. Il Sistema Base di Aru

### Carte Numeriche (da 1 a 7): Statistiche Dirette 1:1
I numeri da 1 a 7 forniscono **esattamente il loro valore numerico** nella statistica associata al seme. Semplice, leggibile e intuitivo.

| Seme | Statistica / Risorsa | Effetto delle carte 1–7 | Esempio (Carta valore 5) |
|---|---|---|---|
| **Spade** | **Attacco / Danno** | Fornisce da +1 a +7 Danno per i combattimenti | 5 di Spade = +5 Danno |
| **Bastoni** | **Armatura / Difesa** | Fornisce da +1 a +7 Scudo che assorbe i colpi | 5 di Bastoni = +5 Armatura |
| **Coppe** | **Cura / Vita** | Ripristina da +1 a +7 HP (fino al cap massimo) | 5 di Coppe = +5 HP curati |
| **Denari** | **Soldi / Lire** | Fornisce valuta (scala base: valore × 1.000 Lire) | 5 di Denari = +5.000 Lire |

*Nota di bilanciamento*: la scala dei Denari può essere 1:1 (se i prezzi sono bassi) o 1:1.000 (1.000–7.000 Lire per carta, per un feeling realistico da bar).

---

## 3. Le Figure: Ruoli e Meccaniche Speciali

### 3.1 I Re (10) — I Boss da Affrontare (Fissato)
I 4 Re rappresentano i boss principali dell'incontro. Affrontarli e sconfiggerli è l'obiettivo del gioco.

- **Stato**: Per ora i Re sono **semplici boss**, senza abilità speciali, poteri unici o statistiche differenziate. Il loro ruolo è unicamente essere l'avversario da sconfiggere nel combattimento.
- **Combattimento**: Scambio a turni classico tra il giocatore (che usa il danno accumulato dalle Spade e l'armatura dai Bastoni) e il Re.

---

### 3.2 Gli 8 (Fanti) — Lo Shop (Fissato)
I Fanti sono gli incontri commerciali / shop del gioco, dove spendere le Lire guadagnate.

- **Struttura base**: Girare un 8 dà accesso allo shop (con opzione di saltarlo per ottenere un bonus immediato in Lire).
- **Domanda aperta**: Unico shop generale oppure shop specializzato per seme (Denari = generale, Coppe = bar/cure, Bastoni = difesa, Spade = armeria/danno).

---

### 3.3 I 9 (Cavalli) — Specializzati per Seme (Tavolo vs Bossfight)

I 4 Cavalli offrono una scelta strategica attiva al giocatore, bilanciata tra **influenzare la bossfight/risorse** e **influenzare il tavolo**:

#### 💰 Cavallo di Denari (9 di Denari) — Il Banco e la Scommessa
- **Scelta Sicura (Raddoppio)**: Raddoppia immediatamente tutte le Lire guadagnate in questo specifico path (2x).
- **Scelta Rischio (La Scommessa / Il Totip)**: Tenta la scommessa per **triplicare le Lire** (3x nel path), ma con il rischio di **perdere 1/4 (25%)** delle Lire accumulate nel path se la scommessa viene persa!
  *(Condizione della scommessa da definire: es. se il path ha almeno N carte, se finisce su un numero pari, o lancio di moneta/dado al bar).*

#### 🍷 Cavallo di Coppe (9 di Coppe) — Salvezza o Grande Manovra
- **Opzione Bossfight (Fuga)**: Ottieni **1 carica di Fuga** da conservare e usare per evitare un Re pericoloso senza subire penalità.
- **Opzione Tavolo (Inversione)**: **Inverti due righe intere o due colonne intere** sul tavolo, rimescolando la griglia per sbloccare nuove combinazioni di path.

#### ⚔️ Cavallo di Spade (9 di Spade) — Proposte per Attacco / Tavolo
In linea con il seme dell'attacco e del fendente:
- **Opzione A (Bossfight — Carica Offensiva)**:
  - Il prossimo attacco contro il Re infligge **danno raddoppiato (2x)**, oppure ottieni **1 turno di iniziativa** (attacchi per primo prima che il Re possa sferrare il suo colpo).
- **Opzione B (Tavolo — Scambio Tattico o Sbirciata Mirata)**:
  - **Scambio di 2 carte mirate**: scambia liberamente di posto 2 singole carte coperte sul tavolo per creare un allineamento perfetto.
  - *Oppure* **Sbirciata (Scout)**: scopre temporaneamente 2–3 carte coperte a scelta sul tavolo per individuare Re o risorse chiave.

#### 🛡️ Cavallo di Bastoni (9 di Bastoni) — Proposte per Difesa / Controllo
In linea con il seme dello scudo, del blocco e dell'ostacolo:
- **Opzione A (Bossfight — Barricata / Blocco)**:
  - Neutralizza completamente (100% blocco) il primo colpo del prossimo Re, assorbendo qualsiasi quantità di danno.
- **Opzione B (Tavolo — Salto dell'Ostacolo o Blocco Carta)**:
  - **Salto dell'ostacolo**: permette di "scavalcare" una carta coperta sul tavolo lasciandola coperta e continuando il path oltre di essa (ideale per non girare un Re prima del tempo).
  - *Oppure* **Blocco sul tavolo**: "congela" o protegge una carta sul tavolo in modo che non si giri al prossimo path.

---

## 4. Dinamiche di Incontro e Flusso di Gioco

- **Forme geometriche e Combo**: premiare path che formano percorsi particolari sul tavolo (linee, quadrati 2x2, croci).
- **Sinergie con Santini e Amari**: come gli eventi delle carte innescano i santini posseduti nell'inventario.
- **Condizioni di vittoria dell'incontro**: girare tutti e 4 i re, oppure girare l'intero mazzo, oppure sopravvivere a un numero fisso di path.

---

## 5. Appunti e Proposte Libere

*(Spazio aperto per annotare spunti, varianti, commenti o regole da approfondire insieme)*
