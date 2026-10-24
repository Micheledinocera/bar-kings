---
tipo: decisione
stato: in analisi
aggiornato: 2026-10-24
verificato: 2026-10-24
fonti:
  - raw/compiled/study-direzione-artistica/2026-10-24-obiettivo.md
  - raw/compiled/study-direzione-artistica/2026-10-24-task.md
  - raw/compiled/study-direzione-artistica/2026-10-24-ispirazioni-giochi.html
  - raw/compiled/study-direzione-artistica/2026-10-24-direzione-artistica-jacovitti/Jacovitti 375ae1bb2a0b80c29408de6dedace07d.html
  - raw/compiled/study-direzione-artistica/2026-10-24-direzione-artistica-felix-colgrave/Felix Colgrave 375ae1bb2a0b80b79765ff67889d2f73.html
  - raw/compiled/study-direzione-artistica/2026-10-24-direzione-artistica-pilgrims/Pilgrims 398ae1bb2a0b80e4a474f4a96a9f065a.html
  - raw/compiled/study-direzione-artistica/2026-10-24-direzione-artistica-sol-cesto/Sol Cesto 375ae1bb2a0b80a181e1c86f4df9276b.html
---

# Direzione artistica

Lo stile grafico del gioco: ambientazione, carte, riferimenti. Prime indicazioni dello study-direzione-artistica (2026-10-24): niente è deciso, servono come base per i primi design prototipo.

## Cosa sappiamo

- **Stile generale**: il **bar di provincia anni '70** (indicazione dell'utente). Si lega al lessico del gioco: bar, scagnozzi e capobar ([Parte RPG](../business/parte-rpg.md)), santini e amari ([Meccaniche della run](../business/meccaniche-della-run.md)), tombola per le forme dei buchi ([Modifica del mazzo](../business/modifica-del-mazzo.md)).
- **Carte**: "una cosa sui generis", come il **Mercante in fiera di Jacovitti** o i **video di Felix Colgrave** (indicazione dell'utente). Sono riferimenti di stile: le carte vanno disegnate originali, non copiate.
- **Tono di voce**: **demenziale e satirico**; si gioca sulla nostalgia ma si vuole mostrare anche il lato **grottesco** dell'Italia "squisitamente berlusconiana", fatta soprattutto di ex poveri (indicazione dell'utente).
- **Design system** (cosa deve contenere, indicazione dell'utente): soprattutto qualche **alternativa di colori** e le **note sull'animazione**, dove si guarda a giochi esistenti e "soddisfacenti" (vedi *Giochi di ispirazione* sotto).
- **Oggi** le scene usano le carte napoletane classiche (`Assets/Images/Cards/Fronte/`, vedi [tecnico/scenes](../tecnico/scenes.md)): sono segnaposto della POC.
- **Prossimo passo** (obiettivo dello studio): un minimo di design grafico su Claude Design, come prototipo per capire dove si può andare: un design per la parte **pathbuilder** (il tavolo, con eventualmente qualche idea su come rappresentare i path), uno per la parte **RPG** ed eventualmente uno **mixed**.

### Riferimenti

Le immagini sono in `docs/raw/compiled/study-direzione-artistica/`, una cartella per riferimento. La colonna "Cosa se ne legge" è la lettura dell'agent dalle immagini, da confermare.

| Riferimento | Fonte | Cosa se ne legge |
|---|---|---|
| **Jacovitti**, *Mercante in fiera* | Carta "16 Lo Sgargabonzi" | Una carta = un personaggio assurdo, pieno di dettagli e marchingegni; contorno nero spesso, colori piatti e saturi, fondo verde tratteggiato; numero e nome in grande in basso, lettering a mano |
| **Felix Colgrave** | [Canale YouTube](https://www.youtube.com/@FelixColgrave), due fotogrammi | Creature surreali e un po' inquietanti; linea pulita, colori pastello (rosa, verde, crema), occhi tondi; animazione fluida |
| **Pilgrims** (Amanita Design) | Immagine di copertina, [video](https://www.youtube.com/watch?v=5x1JGgo_udU) | Osteria buia, una lampada sul tavolo rotondo con boccali e carte, avventori grotteschi e un diavolo; tratto a carboncino irregolare, toni caldi e terrosi: è già quasi "il bar di provincia" |
| **Sol Cesto** | [Steam](https://store.steampowered.com/app/2738490/Sol_Cesto/), screenshot di gioco | Roguelite su una **griglia di caselle** con icona e numero (danno, cura, tesoro, ?), casella bloccata da catene; mostri e UI che incorniciano la griglia; contorni spessi, palette scura con accenti accesi. Vicino al tavolo 4×10 e ai buchi |

**Giochi di ispirazione** (tabella dell'utente: cosa piace e cosa no):

| Gioco | Cosa ci piace | Cosa non ci piace |
|---|---|---|
| [Balatro](https://store.steampowered.com/app/2379780/Balatro/) | Gameplay, animazioni | Direzione artistica |
| Broom ([reel](https://www.instagram.com/reels/Dd9XQiEBwQL/)) | Animazioni, stile delle carte | Direzione artistica |
| [Tic Card Toe](https://store.steampowered.com/app/4031860/Tic_Card_Toe/) | Animazioni, tipo di gameplay | Direzione artistica |

Dei tre giochi di carte si prendono le **animazioni** (e il gameplay), non l'aspetto.

## Opzioni

Due direzioni che i riferimenti tengono insieme e che i design prototipo possono confrontare (lettura dell'agent, nessuna scelta):

| Direzione | Da dove | Note |
|---|---|---|
| **Bar scuro e caldo** | Pilgrims, Sol Cesto | Ambiente in penombra, luce sul tavolo; si sposa col bar anni '70. Rischio: carte poco leggibili sul tavolo 4×10 |
| **Carte chiare e affollate** | Jacovitti, Colgrave | Colori piatti o pastello, contorno spesso, molti dettagli; ogni carta un personaggio. Rischio: troppo dettaglio a dimensione di carta sul tavolo |

Una combinazione possibile: ambiente scuro alla Pilgrims, carte chiare alla Jacovitti che spiccano sul panno.

**Come mostrare la parte RPG** (idee dell'utente, nessuna scelta):

| Idea | Cosa fa |
|---|---|
| **Solo barre** | Vita e parametri come barre, con la stessa visualizzazione della parte pathbuilder |
| **Scena divisa in due** | Sopra il tavolo, sotto la parte RPG: per esempio un omino che cammina mentre gli eventi del path si susseguono |

## Alternative scartate

- **Direzione artistica di Balatro, Broom e Tic Card Toe** — scartata il 2026-10-24: dei tre giochi piacciono gameplay e animazioni, non l'aspetto (fonte: raw/compiled/study-direzione-artistica/2026-10-24-ispirazioni-giochi.html)

## Domande aperte

- Le carte restano riconoscibili come mazzo italiano (semi e valori leggibili a colpo d'occhio) o diventano personaggi a sé, con seme e valore come indicazione secondaria?
- Ogni carta ha un disegno suo (40, più eventuali carte speciali) o il disegno dipende da seme e valore (es. figure disegnate, numeriche più semplici)?
- Leggibilità: quanto dettaglio regge una carta sul tavolo 4×10 (dimensione a schermo, piattaforma non decisa)?
- Tavolo, buchi e forme: come si disegnano (panno verde, cartella della tombola, altro)?
- Animazione: cosa prendere esattamente dalle animazioni di Balatro, Broom e Tic Card Toe; tecnica (frame by frame alla Colgrave o tween, già con DOTween nel progetto)?
- Quale idea per mostrare la parte RPG (solo barre o scena divisa)?
- Palette, font e lettering (a mano come Jacovitti e Pilgrims?), UI e audio: non ancora discussi.
- Chi produce gli asset definitivi.
