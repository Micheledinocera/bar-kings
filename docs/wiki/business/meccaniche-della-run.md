---
tipo: decisione
stato: in analisi
aggiornato: 2026-10-29
verificato: 2026-10-07
fonti:
  - raw/compiled/study-roadmap/2026-10-29-delucidazioni.md
  - raw/compiled/study-roadmap/2026-10-29-idee-santini-amari.md
  - raw/compiled/study-roadmap/2026-10-29-ispirazioni.md
  - raw/compiled/study-call-28-10-2026/2026-10-28-note-call.md
  - raw/compiled/study-initialize-llm-wiki/2026-06-10-idee-embrionali.md
  - raw/compiled/study-initialize-llm-wiki/2026-06-10-idea-balatro-like.md
  - raw/compiled/study-initialize-llm-wiki/2026-10-07-chiarimenti-gameplay.md
---

# Meccaniche della run

Cosa succede tra un incontro e l'altro di una run, in stile Balatro. Sono idee embrionali che in linea di massima vanno bene: nessuna è definita nei dettagli. Cosa entra nella vertical slice: [Roadmap verso la vertical slice](../progetto/roadmap-vertical-slice.md).

## Cosa sappiamo

| Elemento | Cos'è | Come si ottiene | Esempi |
|---|---|---|---|
| **Starter** | Filosofia verso cui indirizzare la run: dà funzionalità più che numeri. Uno solo; si può cambiare o potenziare in punti fissi della run (a differenza dei mazzi di Balatro). Nella vertical slice gli starter sono i **preset del mazzo** (team, 2026-10-29) | All'inizio della run (da capire se scelto o trovato in early) | Cicli; posizionamento (piazzare ovunque nella propria riga/colonna); semi (bonus a tutte le carte di un seme); aree "scoperte"; fortuna (pool di carte iniziali tra cui scegliere) |
| **Santini** | Power-up fissi con effetti anche numerici, in un numero fisso di slot; si possono comprare, scartare, distruggere. Generici o legati a una filosofia (più probabili con lo starter collegato). Sono soprattutto **effetti passivi che scattano su eventi** della parte RPG, del tavolo o del percorso tra un combattimento e l'altro, e l'effetto è **legato alla figura reale del santo** (es. San Francesco parlava agli animali → i cavalli, i 9) (team, 2026-10-29) | In punti specifici della run (da definire) | Moltiplicatore per ciclo in base alle carte; Santa Lucia evidenzia la colonna del re; Santa Caterina bonus alle donne; San Silvestro bonus con un solo re in gioco; asso garantito nel pool iniziale; bonus per i quadrati 2x2, 3x3, 4x4. Altre idee in *Idee di santini* |
| **Amari** | Consumabili ("si bevono") con effetto immediato, su una carta, sulla partita o fuori dallo scontro; slot massimi. Da decidere se si bevono durante il combattimento o tra un combattimento e l'altro; idea: la **modifica del mazzo** passa dagli effetti degli amari (team, 2026-10-29) | Regolarmente, meno comuni dei santini | Scambia due carte; il prossimo ciclo conta una carta in più; cambia il seme di una carta; buff a una carta; rimescola un re già pescato; fai apparire uno shop. Altre idee in *Idee di amari* |
| **Imbrogli** | Effetti passivi, positivi o negativi; non si comprano, capitano | Casuali, abbastanza rari | — |
| **Azioni** | Azioni del giocatore sul piazzamento | — | Swap (mettere una carta a distanza x dalla sua posizione); discard (prendere al suo posto una delle carte escluse all'inizio); peek (sbirciare dove si sta per piazzare) |

## Opzioni

**Dimensione del mazzo** (da [Modifica del mazzo](./modifica-del-mazzo.md)):

- Sempre 36 carte, potenziabili o trasformabili con "bollini" alla Balatro, oppure non modificabili.
- Più o meno di 36 carte: si piazzano comunque 36 (e i doppioni? e se sono meno?), oppure si piazzano tutte.

**Idee sparse** dalle prime call, non ancora collocate:

- Ogni seme con un effetto o attributo (denari → soldi, coppe → cure, bastoni → armatura, spade → danno), anche come debolezze e resistenze.
- La briscola come seme che attiva o potenzia la sua abilità secondaria e cambia a ogni incontro, come una "fase lunare".
- In un autobattler, una via di mezzo tra i joker di Balatro e i gambit di Final Fantasy XII: l'autobattler si "programma".
- I potenziamenti sono i numeri della smorfia.
- Le carte hanno una posizione: se ne può cambiare il valore, ma restano nello stesso slot.
- **Modifiche del mazzo tra un incontro e l'altro** (call del 2026-10-28): oltre a togliere e aggiungere carte, azioni che **modificano l'indice** delle carte (scambiare due semi, due valori o due carte), per rendere le forme dei buchi meno dipendenti dall'ordine dei semi ([Modifica del mazzo](./modifica-del-mazzo.md#forme-dei-buchi-tribuna)). Dal 2026-10-29 l'idea è che passino dagli amari (vedi *Idee di amari*).
- **Shop legato alle forme** (call del 2026-10-28): le forme fatte in un incontro migliorano il pool di un eventuale shop; il 4×4, quasi impossibile, garantisce un santino leggendario.

### Idee di santini

Prima lista dell'agent (study-roadmap, 2026-10-29), da vagliare; numeri da tarare con la simulazione. Area: RPG, Carte (tavolo, path, forme), Percorso (tra gli incontri). Santa Lucia, Santa Caterina e San Silvestro erano già tra gli esempi sopra.

| Santino | Dalla figura del santo | Quando scatta → effetto | Area |
|---|---|---|---|
| San Francesco d'Assisi | Parlava agli animali | Un cavallo (9) si risolve → effetto applicato due volte | Carte |
| Santa Lucia | Protettrice della vista | Inizio incontro → mostra per un attimo la colonna dei re | Carte |
| San Giorgio | Uccise il drago | Contro il capobar → danno doppio | RPG |
| San Martino | Divise il mantello | Primo colpo ricevuto nel combattimento → dimezzato; la metà parata va metà in armatura e metà in vita | RPG |
| San Matteo | Esattore delle tasse | Carta di denari → +50% Lire; o sconto nello shop (la "decima") | RPG / Percorso |
| San Lorenzo | Graticola; le "lacrime" del 10 agosto | Fine incontro → righe e colonne complete della cartella valgono il doppio; variante: al 4° re si scopre una carta coperta | Carte |
| Sant'Antonio da Padova | Ritrova le cose perdute | Una volta per incontro, path fermo su un buco → il buco si riempie con la carta mancante (path più lunghi, una forma in meno) | Carte |
| San Pietro | Le chiavi; rinnegò tre volte | Re incontrato → lo si "rinnega" e si salta il combattimento senza malus, 3 volte per run (*Saltare il re* in [Parte RPG](./parte-rpg.md#opzioni)) | RPG |
| San Gennaro | Il sangue che si scioglie, o è sventura | Inizio incontro → 2 volte su 3 le coppe curano il doppio, 1 su 3 non curano | RPG |
| Santa Rita da Cascia | Santa dei casi impossibili | Vita a 0 → una volta per run si resta a 1; variante: bonus enorme per il 4×4 | RPG / Carte |
| San Cristoforo | Patrono dei viaggiatori | Tappa del percorso → mostra il prossimo incontro e i suoi malus; con i bivi ne apre uno in più | Percorso |
| San Rocco | Protettore dalla peste | Inizio incontro → annulla un malus del boss | RPG / Carte |
| San Giuseppe | Falegname | Carta di bastoni → +1 armatura in più | RPG |
| Santa Caterina d'Alessandria | La ruota | Fante (8, "la donna") → bonus alle donne; con la ruota, ruota i semi delle due carte dopo nel path | Carte |
| San Silvestro | Ultimo giorno dell'anno | Ultimo re scoperto → colpo gratis al re ("botto finale"); dagli esempi: bonus con un solo re in gioco | RPG |

### Idee di amari

Prima lista dell'agent (study-roadmap, 2026-10-29). Nomi a gioco di parole sui veri amari e liquori del bar, nello stile di Vampire Survivors ([Direzione artistica](../arte/direzione-artistica.md#cosa-sappiamo)); sono parodie di marchi reali, quindi sempre storpiati, mai il nome vero (nota dell'agent). "Quando" è il momento proposto, ancora aperto.

| Amaro | Gioco di parole | Quando | Effetto | Mazzo |
|---|---|---|---|---|
| Rimescotti | Ramazzotti + rimescolare | Tra gli incontri | Scambia due semi (due righe della cartella) | Sì |
| Scambuca | Sambuca + scambiare | Tra gli incontri | Scambia due valori (due colonne); variante: due carte | Sì |
| Fernet Brancaleone | Fernet Branca + l'armata Brancaleone | Tra gli incontri | Toglie una carta dal mazzo per sempre | Sì |
| Averno | Averna + inferno | Tra gli incontri | Duplica una carta; il doppione è "maledetto" (es. −1) | Sì |
| Stregone | Strega | Tra gli incontri o durante | Cambia il seme di una carta | Sì |
| Mercante in Fiernet | Fernet + *Mercante in fiera* | Tra gli incontri | Fa apparire uno shop alla prossima tappa | No |
| Amaro del Capobar | Amaro del Capo + capobar | Prima del capobar | Il boss perde un malus a scelta | No |
| Amaro Lungo | Il caffè lungo | Durante l'incontro | Il prossimo path conta una carta in più | No |
| Sbircio Cynar | Cynar + sbirciare | Durante l'incontro | Guarda la carta di uno slot coperto (peek) | No |
| Re-Montenero | Montenegro + rimonta | Durante l'incontro | Rimescola nel tavolo un re già scoperto | No |
| Grappino del Becchino | Grappa + becchino | Durante il combattimento | Il prossimo colpo ignora l'armatura | No |
| Cerottino Martini | China Martini + cerotto | Durante il combattimento | Cura subito una parte della vita | No |
| Unicum Tappabuchi | Unicum + tappabuchi | Durante l'incontro | Riempie un buco con una carta coperta presa dalle buttate | Solo il tavolo |

## Alternative scartate

Nessuna alternativa scartata finora.

## Domande aperte

- Struttura della run: incontri, shop, punti fissi in cui cambiare lo starter o trovare santini. Prima proposta (3 bar, scagnozzi e capobar) in [Parte RPG](./parte-rpg.md); per la slice 1 bar con un percorso a tappe ([roadmap](../progetto/roadmap-vertical-slice.md)).
- Numero di slot per santini e amari.
- Esempi concreti di imbrogli.
- Amari: si bevono durante il combattimento, tra un combattimento e l'altro, o dipende dall'amaro? Da decidere al punto A della [roadmap](../progetto/roadmap-vertical-slice.md#fasi).
- Gli amari sono l'unico modo di modificare il mazzo, o resta anche una modifica gratuita a ogni tappa?
- Santini: nomi veri dei santi (l'ironia sta nell'effetto) o storpiati come gli amari? Limiti al tono satirico sui santi di devozione locale (San Gennaro, Sant'Antonio)?
- Quali santini sono rari o leggendari (il premio del 4×4)?
