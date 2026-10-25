---
tipo: decisione
stato: in analisi
aggiornato: 2026-10-25
verificato: 2026-10-24
fonti:
  - raw/compiled/art-style/2026-10-25-obiettivo.md
  - raw/compiled/art-style/2026-10-25-scelte-design.md
  - raw/compiled/art-style/2026-10-25-implementazione.md
  - raw/compiled/study-direzione-artistica/2026-10-24-obiettivo.md
  - raw/compiled/study-direzione-artistica/2026-10-24-task.md
  - raw/compiled/study-direzione-artistica/2026-10-24-ispirazioni-giochi.html
  - raw/compiled/study-direzione-artistica/2026-10-24-direzione-artistica-jacovitti/Jacovitti 375ae1bb2a0b80c29408de6dedace07d.html
  - raw/compiled/study-direzione-artistica/2026-10-24-direzione-artistica-felix-colgrave/Felix Colgrave 375ae1bb2a0b80b79765ff67889d2f73.html
  - raw/compiled/study-direzione-artistica/2026-10-24-direzione-artistica-pilgrims/Pilgrims 398ae1bb2a0b80e4a474f4a96a9f065a.html
  - raw/compiled/study-direzione-artistica/2026-10-24-direzione-artistica-sol-cesto/Sol Cesto 375ae1bb2a0b80a181e1c86f4df9276b.html
---

# Direzione artistica

Lo stile grafico del gioco: ambientazione, carte, riferimenti. Prime indicazioni dello study-direzione-artistica (2026-10-24) e una prima prova in Unity, la scena Bar (feature art-style, 2026-10-25): niente è deciso.

## Cosa sappiamo

- **Stile generale**: il **bar di provincia anni '70** (indicazione dell'utente). Si lega al lessico del gioco: bar, scagnozzi e capobar ([Parte RPG](../business/parte-rpg.md)), santini e amari ([Meccaniche della run](../business/meccaniche-della-run.md)), tombola per le forme dei buchi ([Modifica del mazzo](../business/modifica-del-mazzo.md)).
- **Carte**: "una cosa sui generis", come il **Mercante in fiera di Jacovitti** o i **video di Felix Colgrave** (indicazione dell'utente). Sono riferimenti di stile: le carte vanno disegnate originali, non copiate.
- **Tono di voce**: **demenziale e satirico**; si gioca sulla nostalgia ma si vuole mostrare anche il lato **grottesco** dell'Italia "squisitamente berlusconiana", fatta soprattutto di ex poveri (indicazione dell'utente).
- **Design system** (cosa deve contenere, indicazione dell'utente): soprattutto qualche **alternativa di colori** e le **note sull'animazione**, dove si guarda a giochi esistenti e "soddisfacenti" (vedi *Giochi di ispirazione* sotto).
- **Oggi** le scene usano le carte napoletane classiche (`Assets/Images/Cards/Fronte/`, vedi [tecnico/scenes](../tecnico/scenes.md)): sono segnaposto della POC.
- **Design prototipo su Claude Design** (fatto): [King Bar — Prototipo grafico](https://claude.ai/artifact/5BrbVvSx78nCQRwABub9Uy), due prove. La **prima** ("bar di provincia anni '70, cartoon"): tavolo, combattimento RPG, mixed, le carte. La **seconda** ("grottesco, carte segnaposto"): tavola 1 pathbuilder (il tavolo con avventori, scontrino, cartella della tombola, santini e amari), tavola 2 combattimento RPG, tavola 3 mixed (tavolo sopra, omino che cammina sulla strada del path sotto), tre palette (Nicotina, Tinello anni '70, Varietà del sabato sera), carta segnaposto. All'utente la seconda prova sembra **più a fuoco**.

### Prima prova in Unity: la scena Bar

Feature art-style (2026-10-25), **in prova**: scelte dell'utente, non decisioni. Scena `Assets/Scenes/Bar.unity`, copia di Tribuna, dal Menu con "Bar (stile)"; codice e asset in [tecnico/scenes](../tecnico/scenes.md#bar) e [tecnico/hud](../tecnico/hud.md).

| Scelta | In prova |
|---|---|
| Palette | **Nicotina**: Buio `#120D09`, Legno `#3A2618`, Panno `#3D4A2A`, Scontrino `#E6DBBF`, Nicotina `#C9A227`, Fegato `#8E2F22`, Abbronzatura `#C27A48`, Bile `#8A9A3B`. Mood: fumo stagnante, panno lurido, toni marci e caldi sotto una sola lampada |
| Font | Caveat Brush (titoli, a mano) e Special Elite (testo, macchina da scrivere), come nel design |
| Composizione | Mix delle tavole **1 e 3** della seconda prova: tavolo al centro con gli avventori dietro, a destra **scontrino** (le carte del path, scritte una per volta mentre si girano), re scoperti e **cartella della tombola** (i buchi e le forme fatte), sotto la **strada del path** con l'omino che cammina sulle carte girate |
| Tecnica | **Reskin del 3D** (tavolo e carte 3D, camera quasi dall'alto, una lampada, vignettatura e grana), non un rifacimento 2D; risoluzione di riferimento 16:9 |
| Carte | Le **napoletane** di oggi, non le carte segnaposto del design |
| Buchi | **Sagoma tratteggiata della carta**: i buchi non sono veri buchi (scartata la bruciatura di sigaretta del design) |
| Parte RPG | Idea **scena divisa in due** (omino che cammina sotto il tavolo), ma solo con le carte: nessun evento. Segnaposto per avventori, malus del capobar, santini e amari, soldi, vita e armatura, eventi e nemici |

Note dell'utente dopo la prova: la strada dell'omino più bassa, per dare spazio al resto; la scena **piace molto**.

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

Una combinazione possibile: ambiente scuro alla Pilgrims, carte chiare alla Jacovitti che spiccano sul panno. **In prova** nella scena Bar: ambiente scuro con una lampada sul panno (vedi sopra).

**Come mostrare la parte RPG** (idee dell'utente, nessuna scelta):

| Idea | Cosa fa |
|---|---|
| **Solo barre** | Vita e parametri come barre, con la stessa visualizzazione della parte pathbuilder |
| **Scena divisa in due** | Sopra il tavolo, sotto la parte RPG: per esempio un omino che cammina mentre gli eventi del path si susseguono. **In prova** nella scena Bar, per ora solo con le carte |

## Alternative scartate

- **Direzione artistica di Balatro, Broom e Tic Card Toe** — scartata il 2026-10-24: dei tre giochi piacciono gameplay e animazioni, non l'aspetto (fonte: raw/compiled/study-direzione-artistica/2026-10-24-ispirazioni-giochi.html)
- **Buchi come bruciature di sigaretta sul panno** (seconda prova del design) — scartata il 2026-10-25: i buchi non sono veri buchi; per ora la sagoma tratteggiata della carta (fonte: raw/compiled/art-style/2026-10-25-implementazione.md)

## Domande aperte

- Le carte restano riconoscibili come mazzo italiano (semi e valori leggibili a colpo d'occhio) o diventano personaggi a sé, con seme e valore come indicazione secondaria?
- Ogni carta ha un disegno suo (40, più eventuali carte speciali) o il disegno dipende da seme e valore (es. figure disegnate, numeriche più semplici)?
- Leggibilità: quanto dettaglio regge una carta sul tavolo 4×10 (dimensione a schermo, piattaforma non decisa)?
- Tavolo, buchi e forme: in prova panno verde, sagoma della carta nei buchi e cartella della tombola a lato (scena Bar). Si tengono?
- Animazione: cosa prendere esattamente dalle animazioni di Balatro, Broom e Tic Card Toe; tecnica (frame by frame alla Colgrave o tween, già con DOTween nel progetto)?
- Quale idea per mostrare la parte RPG (solo barre o scena divisa)? In prova la scena divisa, senza eventi.
- Palette e font: in prova Nicotina, Caveat Brush e Special Elite; UI definitiva e audio non ancora discussi.
- Chi produce gli asset definitivi.
