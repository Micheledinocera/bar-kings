---
tipo: info
aggiornato: 2026-11-06
verificato: 2026-10-07
consolidato: 2026-10-29
fonti:
  - raw/compiled/events-impl/2026-11-06-idee-eventi.md
  - raw/compiled/study-roadmap/2026-10-29-delucidazioni.md
  - raw/compiled/study-direzione-artistica/2026-10-24-obiettivo.md
  - raw/compiled/study-rpg/2026-10-20-idee.md
  - raw/compiled/study-rpg/2026-10-20-criticita-e-idee.md
  - raw/compiled/study-initialize-llm-wiki/2026-10-07-concept.md
  - raw/compiled/study-initialize-llm-wiki/2026-10-07-chiarimenti-gameplay.md
  - raw/compiled/study-initialize-llm-wiki/2026-06-10-idee-embrionali.md
  - raw/compiled/study-05-11-2026/2026-11-05-recap-call.md
---

# Concept

Cos'è il gioco, in breve. Le scelte di gameplay sono nelle decisioni linkate qui sotto.

| Voce | Valore |
|---|---|
| Nome | Non deciso (progetto Unity "King Bar", repo `bar-kings`) |
| Genere | Roguelike pathbuilder / RPG |
| Base | Il solitario dei 4 re, con il mazzo italiano da 40 carte (denari, coppe, bastoni, spade) |
| Gameplay di base | [Cicli sul solitario dei 4 re](./gameplay-di-base.md) · presa |
| Meccaniche della run | [Starter, santini, amari, imbrogli, azioni](./meccaniche-della-run.md) · in analisi |
| Modifica del mazzo | [Panchina, tribuna, index>40](./modifica-del-mazzo.md) · in analisi |
| Parte RPG | [Eventi, combattimento, bar e incontri](./parte-rpg.md) · in analisi |
| Direzione artistica | [Bar di provincia anni '70, carte alla Jacovitti o alla Felix Colgrave](../arte/direzione-artistica.md) · in analisi |

## Il ciclo in una frase

Si sceglie una carta e si girano carte fino a tornare a quella iniziale: il ciclo chiuso è un **path**, e risolvendolo ogni carta genera un evento da cui parte la parte RPG. La run, alla Balatro, modifica il mazzo e aggiunge potenziamenti tra un incontro e l'altro.

## Glossario

| Termine | Significato |
|---|---|
| Run | L'intera partita alla Balatro: i bar uno dopo l'altro (in studio: 3, vedi [Parte RPG](./parte-rpg.md)) |
| Bar | Livello della run; in studio: 2 scagnozzi e un capobar, con i suoi malus |
| Incontro | Una partita sul tavolo (contro uno scagnozzo o il capobar), finita quando tutte le carte sono girate |
| Scagnozzo, capobar | Miniboss e boss di un bar (in studio) |
| Path / ciclo | La sequenza di carte girate a partire da quella scelta, fino a tornare a quella iniziale |
| Risoluzione | Il momento in cui ogni carta del path chiuso genera un evento (parte RPG); in prova nella scena Bar-Michele: carta per carta, mentre il path si gira ([Parte RPG](./parte-rpg.md#cosa-sappiamo)) |
| Indice | Il posto del tavolo dove va una carta: nella griglia 4×10 la riga è il seme (denari, coppe, spade, bastoni) e la colonna il valore, quindi il 2 di denari ha di default l'indice 2. Le azioni che "modificano l'indice" (scambiare due semi, due valori, due carte) spostano dove va una carta ([Modifica del mazzo](./modifica-del-mazzo.md#forme-dei-buchi-tribuna)) |
| Starter | "Filosofia" della run, una sola alla volta, cambiabile in punti fissi; nella vertical slice sono i preset del mazzo |
| Santini | Power-up fissi, in slot limitati: effetti passivi che scattano su eventi, legati alla figura del santo |
| Amari | Consumabili, effetto immediato; idea: la modifica del mazzo passa da loro |
| Imbrogli | Effetti passivi casuali, positivi o negativi |
| Panchina, tribuna, index>40 | Tre modi di gestire le carte oltre le 40 (vedi [Modifica del mazzo](./modifica-del-mazzo.md)) |

## Domande aperte

- Nome del gioco: si decide più avanti.
- Pubblico, piattaforme e modello di business: non ancora discussi.
- Tutorial: da fare (call del 2026-11-05); quando e come non discusso. La [roadmap](../progetto/roadmap-vertical-slice.md) lo tiene fuori dalla vertical slice, giocata solo dal team.
