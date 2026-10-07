---
tipo: decisione
stato: in analisi
aggiornato: 2026-10-07
verificato: 2026-10-07
fonti:
  - raw/compiled/study-initialize-llm-wiki/2026-07-01-prima-poc.md
  - raw/compiled/study-initialize-llm-wiki/2026-06-10-idea-balatro-like.md
  - raw/compiled/study-initialize-llm-wiki/2026-10-07-chiarimenti-gameplay.md
---

# Modifica del mazzo: panchina, tribuna, index>40

Come aumentare o modificare il mazzo di base, cioè cosa succede alle carte oltre le 40 (36 di base più i 4 re). Nato dopo la prima POC del solitario dei 4 re (2026-07-01).

## Cosa sappiamo

- Le tre idee sono tutte da implementare; si è iniziato a lavorare solo sulla **panchina**.
- Ognuna ha la sua scena di prova: `Assets/Scenes/Panchina.unity`, `Assets/Scenes/ScenaTribuna.unity`, `Assets/Scenes/ScenaIndex40.unity`.
- In tutte e tre si può modificare una carta cambiandone il valore o il seme.

## Opzioni

| Idea | Carte oltre le 40 | Doppioni | Fine | Effetto sui path |
|---|---|---|---|---|
| **Panchina** | Restano disponibili: ogni volta che si crea un "buco" (un doppione) una carta della panchina lo riempie, coperta (o scoperta?) | Finiscono uno sull'altro | Si gioca finché ci sono buchi da riempire | Li accorcia |
| **Tribuna** | Non disponibili: i buchi non si riempiono. Si abbozzano "forme" fatte dai buchi (l'unica idea compatibile con meno di 40 carte) | Finiscono uno sull'altro | Si gioca fino a 40 carte | Li accorcia |
| **Index>40** | Hanno un indice incrementale (es. un 2 di bastoni aggiunto ha indice 41); si possono creare configurazioni con indici diversi | Non si sovrappongono: hanno indici diversi | — | Li allunga |

Panchina: se nella configurazione iniziale delle 36 non escono doppioni, la panchina non si usa (o si inventa altro). Oltre un certo numero di carte in panchina, un'ipotesi è combinare panchina e tribuna.

## Alternative scartate

Nessuna alternativa scartata finora.

## Domande aperte

- Panchina: le carte che riempiono i buchi entrano coperte o scoperte? Cosa succede oltre un tot di carte in panchina?
- Quale idea si tiene, o se si combinano: dipende dalle prove nelle tre scene.
