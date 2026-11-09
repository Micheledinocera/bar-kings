# Idee Meccaniche ed Eventi Carte — study-eventi-aru

Documento di lavoro e brainstorming per definire il comportamento, le regole e gli effetti delle carte (36 carte evento + 4 re) durante la risoluzione del path e il collegamento con la parte RPG.

---

## 1. Visione d'insieme e Principi Guida

- **Risoluzione del path**: chiuso il ciclo sul tavolo (ritorno alla carta di partenza), il path viene percorso carta per carta. Ogni carta scatena un evento/effetto.
- **Rapporto tra Tavolo e RPG**: il tavolo (solitario dei 4 re) è il motore di generazione del path; la parte RPG (statistiche, incontri, boss/re, shop) è il luogo in cui gli effetti si manifestano.
- **Parametrico vs Fissato**: preferire formule scalari (lunghezza path, livello dello stage, moltiplicatori) rispetto a valori numerici rigidi e statici, per facilitare il bilanciamento tra le diverse varianti del mazzo.

---

## 2. Il Sistema Base di Aru

### 2.1 Carte Numeriche (da 1 a 7): Statistiche Dirette 1:1
I numeri da 1 a 7 forniscono **esattamente il loro valore numerico** nella statistica associata al seme. Semplice, leggibile e intuitivo.

| Seme | Statistica / Risorsa | Effetto delle carte 1–7 | Esempio (Carta valore 5) |
|---|---|---|---|
| **Spade** | **Attacco / Danno** | Fornisce da +1 a +7 Danno per i combattimenti | 5 di Spade = +5 Danno |
| **Bastoni** | **Armatura / Difesa** | Fornisce da +1 a +7 Scudo che assorbe i colpi | 5 di Bastoni = +5 Armatura |
| **Coppe** | **Cura / Vita** | Ripristina da +1 a +7 HP (fino al cap massimo) | 5 di Coppe = +5 HP curati |
| **Denari** | **Soldi / Lire** | Fornisce valuta (scala base: valore × 1.000 Lire) | 5 di Denari = +5.000 Lire |

*Nota di bilanciamento*: la scala dei Denari può essere 1:1 (se i prezzi sono bassi) o 1:1.000 (1.000–7.000 Lire per carta, per un feeling realistico da bar).

### 2.2 Gli Assi (1): Meccanica "Capo-Path"
- **Valore base**: L'Asso dà normalmente **+1** nella statistica del seme (1 Danno, 1 Armatura, 1 Cura, 1.000 Lire).
- **Bonus di Apertura (Capo-Path)**: Se il path **inizia scegliendo un Asso**, scatta un bonus speciale per l'intero percorso:
  - *Opzione A (Bonus di Seme)*: L'Asso che apre dona un potenziamento immediato legato al seme (es. Asso di Spade iniziale dà +3 Danno extra al primo colpo, Asso di Bastoni dà +3 Scudo iniziale, Asso di Coppe cura +3 HP, Asso di Denari regala +3.000 Lire extra).
  - *Opzione B (Path Perfetto)*: Tutte le risorse numeriche raccolte durante quel path ricevono un bonus (es. +1 a ogni carta del path, oppure raddoppia il valore dell'ultima carta che chiude il ciclo).

---

## 3. Le Figure: Ruoli e Meccaniche Speciali

### 3.1 Gli 8 (Fanti) — Lo Shop durante il Tavolo
Gli 8 aprono un negozio istantaneo a metà partita (con l'opzione "Salta lo Shop" per incassare subito Lire).

- **Impostazione generale**: Lo shop è di base **generico** (mostra consumabili, amari, cure, oggetti comuni per tutti i semi).
- **Slot Tematico del Seme**: Ogni 8 ha **1 slot/meccanica esclusiva** legata al seme della carta girata:
  - **8 di Denari**: Offerta finanziaria (es. sconto su tutti gli articoli, o investimento con rendita futura).
  - **8 di Coppe**: Ristoro del barista (cura istantanea, caffè corretto, aumento temporaneo HP max).
  - **8 di Bastoni**: Difesa e sicurezza (scudo rinforzato per il prossimo re, armatura extra).
  - **8 di Spade**: Potenza d'attacco (colpo critico, armi affilate, aumento danni permanenti).
- **I Buoni del Mercante**:
  - Tra le merci acquistabili negli shop dell'8 ci sono dei **Buoni / Voucher** da conservare per il Mercante del mazzo (vedi sez. 4):
    - *Buono Cancellazione*: permette di eliminare gratis una carta dal mazzo dal Mercante.
    - *Buono Scambio*: permette di scambiare il seme o il valore di una carta.
    - *Buono Sconto*: riduce del 50% il costo della prossima spesa dal Mercante.

---

### 3.2 I 9 (Cavalli) — Specializzati per Seme (Tavolo vs Bossfight)

I 4 Cavalli offrono una scelta strategica attiva al giocatore, bilanciata tra **influenzare la bossfight/risorse** e **influenzare il tavolo**:

#### 💰 Cavallo di Denari (9 di Denari) — Il Banco e la Scommessa
- **Scelta Sicura (Raddoppio)**: Raddoppia immediatamente tutte le Lire guadagnate in questo specifico path (2x).
- **Scelta Rischio (La Scommessa / Il Totip)**: Tenta la scommessa per **triplicare le Lire** (3x nel path), ma con il rischio di **perdere 1/4 (25%)** delle Lire accumulate nel path se la scommessa viene persa!
  *(Condizione della scommessa: Vincere al gioco delle tre carte).*

#### 🍷 Cavallo di Coppe (9 di Coppe) — Salvezza o Grande Manovra
- **Opzione Bossfight (Fuga)**: Ottieni **1 carica di Fuga** da conservare e usare per evitare un Re pericoloso senza subire penalità.
- **Opzione Tavolo (Inversione)**: **Inverti due righe intere o due colonne intere** sul tavolo, rimescolando la griglia per sbloccare nuove combinazioni di path.

#### ⚔️ Cavallo di Spade (9 di Spade) — Attacco al Re o Manovra con Bastonata
- **Opzione A (Bossfight — Danno all'armatura)**:
  - Paga una somma di Lire per infliggere un danno diretto all'armatura del prossimo Re.
- **Opzione B (Tavolo — Scambio Tattico o Sbirciata Mirata)**:
  - **Scambio di 2 carte mirate (Bastonata pesante)**: scambia liberamente di posto 2 singole carte coperte sul tavolo per creare un allineamento perfetto al costo di una somma di Lire.
  - **Sbirciata (Bastonata leggera)**: scopre temporaneamente 2–3 carte coperte a scelta sul tavolo per individuare Re o risorse chiave.

#### 🛡️ Cavallo di Bastoni (9 di Bastoni) — Difesa o Elusione
- **Opzione A (Bossfight — Barricata / Blocco)**:
  - Neutralizza completamente (100% blocco) il primo colpo del prossimo Re, assorbendo qualsiasi quantità di danno.
- **Opzione B (Tavolo — Salto dell'Ostacolo)**:
  - **Salto dell'ostacolo**: permette di "scavalcare" una carta coperta sul tavolo lasciandola coperta e continuando il path oltre di essa (ideale per non girare un Re prima del tempo).

---

## 4. Deckbuilding: Il Mercante tra gli Incontri

La modifica del mazzo avviene **prima di affrontare l'intero tavolo** (nella fase di transizione tra un bar/incontro e il successivo).

- **Il Mercante Ambulante**: Una figura/PNG che incontri tra le tappe del viaggio.
- **Servizi Offerti (a pagamento in Lire o tramite Buoni)**:
  1. **Acquisto Carte**: Aggiungi nuove carte al mazzo (per potenziare semi o valori specifici).
  2. **Cancellazione / Rimozione Carte**: Rimuovi carte deboli o ingombranti (operazione preziosa per rendere il mazzo più consistente e creare "buchi" strategici).
  3. **Scambio / Trasformazione**: Sostituisci una carta esistente con un'altra (es. cambia un 2 di Bastoni in un 2 di Spade, o inverti due valori).
- **Integrazione con gli 8 (Fanti)**:
  - Negli shop degli 8 durante il tavolo puoi comprare i **Buoni del Mercante** a prezzo vantaggioso o trovarli come ricompensa, preparando la strategia di modifica del mazzo per la tappa successiva.

---

## 5. Combinazioni e Carte Speciali

- **I quattro 7**: Quando vengono raccolti tutti e quattro i 7 nell'incontro, si raddoppiano i valori che questi danno singolarmente (es. +7 danno → +14 danno, +7 armatura → +14 armatura, +7 vita → +14 vita, +7.000 Lire → +14.000 Lire).
- **Il Settebello (7 di Denari) — Ricompensa Leggendaria**:
  - Oltre a dare +7.000 Lire, conferisce un bonus unico al giocatore (es. Jackpot speciale in Lire + scelta tra un Amaro raro o un Buono Mercante gratis, oppure ripristino totale degli HP).
- **Il Diavolo (6-6-6)**: Quando vengono trovati in una singola path tre 6, questi attivano un effetto di bruciatura su tutti i re di questo tavolo.
  - **Effetto**: Dopo ogni attacco del re, questo subisce un tot di danno da bruciatura.
  - L'effetto termina appena viene trovato un successivo 6 oppure termina la partita al tavolo.

---

## 6. Le Forme e i Buchi sul Tavolo

Durante la partita sarà possibile modificare il mazzo in modo tale che durante i path possono comparire dei buchi sulla tavola che non verranno riempiti. Se vengono creati buchi di determinate forme geometriche verranno assegnati dei bonus.

- **Origine dei Buchi**: Rimuovendo o consumando carte tramite il Mercante o speciali eventi, quelle caselle sul tavolo restano "vuote" (buchi).
- **Pattern e Forme Possibili (Idee)**:
  - **Linea 3/ Tris (3 buchi allineati in riga o colonna)**: ToDo.
  - **Linea 4 / Forza 4 (4 buchi allineati in riga o colonna)**: ToDo.
  - **Quadrato 2x2 (4 buchi)**: ToDo.
  - **Croce / T**: ToDo.
  - **Forme Grandi (3x3 o 4x4)**: ToDo.
