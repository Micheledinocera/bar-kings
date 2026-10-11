---
tipo: decisione
stato: in analisi
aggiornato: 2026-10-11
verificato: 2026-10-07
fonti:
  - raw/compiled/panchina-start/2026-10-11-obiettivo.md
  - raw/compiled/panchina-start/2026-10-11-fix-path.md
  - raw/compiled/study-initialize-llm-wiki/2026-07-01-prima-poc.md
  - raw/compiled/study-initialize-llm-wiki/2026-06-10-idea-balatro-like.md
  - raw/compiled/study-initialize-llm-wiki/2026-10-07-chiarimenti-gameplay.md
  - raw/compiled/study-new-implementations/2026-10-11-risposte-verifiche.md
---

# Modifica del mazzo: panchina, tribuna, index>40

Come aumentare o modificare il mazzo di base, cioè cosa succede alle carte oltre le 40 (36 di base più i 4 re). Nato dopo la prima POC del solitario dei 4 re (2026-07-01).

## Cosa sappiamo

- Le tre idee sono tutte da implementare; si è iniziato a lavorare solo sulla **panchina**.
- Ognuna ha la sua scena di prova: `Assets/Scenes/Panchina.unity`, `Assets/Scenes/ScenaTribuna.unity`, `Assets/Scenes/ScenaIndex40.unity`.
- In tutte e tre si può modificare una carta cambiandone il valore o il seme.
- Le scene gemelle sono **temporanee**: servono a provare un'idea senza rompere la precedente. Quando se ne sceglie una, le altre (e il codice che usano solo loro) si eliminano.
- Stato del codice al 2026-10-11: `ScenaTribuna` e `ScenaIndex40` sono ancora copie identiche della prima POC. La panchina costruisce un mazzo di 40 carte con doppioni fissi (asso–nove di denari) e quindi carte mancanti: sono casi di test voluti. Dettagli in [tecnico/panchina](../tecnico/panchina.md).
- **Regola del path in prova nella POC della panchina** (2026-10-11, non decisa): il path si chiude quando la carta in mano appartiene allo slot di partenza; se invece il posto della carta in mano è già scoperto (c'è il suo doppione), la carta finisce **sopra** il doppione e lo slot di partenza resta un **buco**. Riempire i buchi con la panchina non è ancora implementato.

## Opzioni

| Idea | Carte oltre le 40 | Doppioni | Fine | Effetto sui path |
|---|---|---|---|---|
| **Panchina** | Restano disponibili: ogni volta che si crea un "buco" (un doppione) una carta della panchina lo riempie, coperta (o scoperta?) | Finiscono uno sull'altro | Si gioca finché ci sono buchi da riempire | Li accorcia |
| **Tribuna** | Non disponibili: i buchi non si riempiono. Si abbozzano "forme" fatte dai buchi (l'unica idea compatibile con meno di 40 carte) | Finiscono uno sull'altro | Si gioca fino a 40 carte | Li accorcia |
| **Index>40** | Hanno un indice incrementale (es. un 2 di bastoni aggiunto ha indice 41); si possono creare configurazioni con indici diversi | Non si sovrappongono: hanno indici diversi | — | Li allunga |

Panchina: se nella configurazione iniziale delle 36 non escono doppioni, la panchina non si usa (o si inventa altro). Oltre un certo numero di carte in panchina, un'ipotesi è combinare panchina e tribuna.

## Alternative scartate

- **Path fermato su un doppione: la carta in mano va nello slot di partenza** (al posto della carta cliccata) — scartata il 2026-10-11 dopo la prova nella POC: la carta deve finire sopra il suo doppione, lasciando il buco (fonte: raw/compiled/panchina-start/2026-10-11-fix-path.md)

## Domande aperte

- Panchina: le carte che riempiono i buchi entrano coperte o scoperte? Cosa succede oltre un tot di carte in panchina?
- Quale idea si tiene, o se si combinano: dipende dalle prove nelle tre scene.
