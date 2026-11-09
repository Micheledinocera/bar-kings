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

- **Combattimento**: scambio di turni di danno (il giocatore usa il danno accumulato e l'armatura assorbe i colpi del re).
- **Identità distinta per seme (proposta da confermare)**:
  - **Re di Spade**: Il Rissoso — infligge danni elevati, richiede alta difesa o abbattimento rapido.
  - **Re di Bastoni**: Il Buttafuori Corazzato — possiede molta armatura/scudo prima di intaccare i suoi HP.
  - **Re di Coppe**: L'Oste / Bevitore — si rigenera salute ogni tot turni o applica malus di sbornia.
  - **Re di Denari**: Il Boss del Bar / Usuraio — estorce Lire ad ogni colpo subito o offre una "tangente" per farsi da parte.
- **Timing del combattimento**:
  - *Opzione A (Attuale in prova)*: Si combatte nel momento esatto in cui il Re si gira nel path.
  - *Opzione B (Fine path)*: Il path si risolve per intero accumulando tutte le risorse, e l'eventuale combattimento avviene al termine del path.

---

### 3.2 Gli 8 (Fanti) — Lo Shop (Fissato)
I Fanti sono gli incontri commerciali / shop del gioco, dove spendere le Lire guadagnate.

- **Opzione 1 (Unico Shop Generale)**:
  - Girare un qualsiasi 8 apre il negozio del bar con una selezione mista (consumabili, amari, potenziamenti permanenti).
  - Possibilità di scegliere "Salta lo Shop" ricevendo un bonus immediato in Lire.
- **Opzione 2 (Shop Specializzato per Seme)**:
  - **8 di Denari**: *Banca / Mercatino* — vende santini, amari, modifiche al mazzo.
  - **8 di Coppe**: *Il Bancone del Barista* — vende caffè, digestivi, cure istantanee e aumenti di HP max.
  - **8 di Bastoni**: *L'Artigiano / Sicurezza* — vende potenziamenti per l'armatura e scudi protettivi contro i re.
  - **8 di Spade**: *L'Allibratore / Armaiolo* — vende colpi critici, armi, o scommesse sulle condizioni del prossimo path.

---

### 3.3 I 9 (Cavalli) — Focus di Brainstorming (In Definizione)
I Cavalli sono le figure intermedie. Come possiamo valorizzarli per dare profondità e divertimento?

Di seguito 4 direzioni possibili per i 9:

#### Direzione 1: Mobilità e Salvezza (Fuga & Velocità)
- **Fuga**: accumula una carica di fuga per evitare lo scontro con un Re pericoloso.
- **Velocità / Iniziativa**: nel combattimento contro il Re, il giocatore guadagna un turno di vantaggio (attacca per primo prima che il Re possa colpire).
- **Scatto**: raddoppia l'effetto della prossima carta del path.

#### Direzione 2: Manipolazione del Tavolo (Strategia del Solitario)
- **Scambio di Indici / Posizioni**: permette al giocatore di scambiare di posto due carte sul tavolo (es. spostare una carta utile in una posizione strategica per chiudere un path lungo o una forma).
- **Sbirciata (Scout)**: scopre temporaneamente 2–3 carte coperte adiacenti, dando preziose informazioni su dove si nascondono i Re o i semi necessari.
- **Salto dell'ostacolo**: permette di scavalcare una carta indesiderata lasciandola coperta.

#### Direzione 3: Il Tema Bar — "La Corsa dei Cavalli" (Scommesse / Totip)
- Al bar c'è sempre la schedina o la corsa dei cavalli:
  - Girare un 9 apre una **mini-scommessa**: "Se questo path ha almeno 5 carte, vinci 3x Lire", oppure "Se sconfiggi il prossimo Re senza perdere HP, vinci un amaro gratis".

#### Direzione 4: Effetto Unico per Seme dei Cavalli
- **Cavallo di Spade (La Carica)**: +1 Turno di anticipo o doppio danno al primo colpo contro il Re.
- **Cavallo di Bastoni (L'Ostacolo)**: Annulla il prossimo malus o neutralizza un attacco nemico.
- **Cavallo di Coppe (Il Giro Veloce)**: Salva una carica di Fuga gratis curando anche un po' di HP.
- **Cavallo di Denari (La Corsa Vincente)**: Moltiplica tutte le Lire guadagnate in questo specifico path.

---

## 3. Dinamiche di Incontro e Flusso di Gioco

- **Forme geometriche e Combo**: premiare path che formano percorsi particolari sul tavolo (linee, quadrati 2x2, croci).
- **Sinergie con Santini e Amari**: come gli eventi delle carte innescano i santini posseduti nell'inventario.
- **Condizioni di vittoria dell'incontro**: girare tutti e 4 i re, oppure girare l'intero mazzo, oppure sopravvivere a un numero fisso di path.

---

## 4. Appunti e Proposte Libere

*(Spazio aperto per annotare spunti, varianti, commenti o regole da approfondire insieme)*
