---
tipo: decisione
stato: in analisi
aggiornato: 2026-10-15
verificato: 2026-10-07
consolidato: 2026-10-15
fonti:
  - raw/compiled/tribuna/2026-10-15-implementazione.md
  - raw/compiled/tribuna/2026-10-15-obiettivo.md
  - raw/compiled/panchina-reprise/2026-10-15-implementazione.md
  - raw/compiled/panchina-reprise/2026-10-15-obiettivo.md
  - raw/compiled/panchina-continue/2026-10-15-implementazione.md
  - raw/compiled/unity-mcp/2026-10-15-test-e-controllo.md
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
- Stato del codice al 2026-10-15: `ScenaIndex40` è ancora una copia identica della prima POC. Panchina e tribuna usano lo stesso codice ([tecnico/table](../tecnico/table.md)) e cambiano solo nelle carte in eccesso. La panchina costruisce un mazzo **oltre le 40 carte** aggiungendo solo doppioni (9, asso–nove di denari, mai re): 40 vanno sul tavolo, sempre con i 4 re, quindi sul tavolo ci sono doppioni e carte mancanti (casi di test voluti); le carte in eccesso formano il **mazzo panchina**. Un pulsante Reset rifà la partita senza uscire dal Play.
- **Regole in prova nella POC della tribuna** (2026-10-15, non decise): stesso mazzo e stesse regole del path della panchina, ma le carte oltre le 40 vengono **buttate** e i **buchi non si riempiono** (la carta che arriva su un buco ci va scoperta e ferma il path).
- **Regola del path in prova nella POC della panchina** (2026-10-11, non decisa): il path si chiude quando la carta in mano appartiene allo slot di partenza; se invece il posto della carta in mano è già scoperto (c'è il suo doppione), la carta finisce **sopra** il doppione e lo slot di partenza resta un **buco**. Se il posto della carta in mano è un **buco**, la carta ci va **scoperta** e il path si interrompe; anche in questo caso lo slot di partenza diventa un nuovo buco (2026-10-15, non decisa).
- **Riempimento dei buchi in prova nella POC della panchina** (2026-10-15, non deciso): quando un path si ferma, il buco rimasto nello slot di partenza viene riempito dalla prima carta del mazzo panchina, **coperta**; la carta si gioca poi come le altre. Se la panchina è finita si continua senza riempire i buchi. Finché la panchina ha carte i buchi si riempiono subito, quindi una carta arriva su un buco solo a panchina vuota. Fine della partita: non definita.

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

- Panchina: la regola in prova per la carta che arriva su un buco (lo riempie scoperta, il path si ferma) è quella definitiva?
- Panchina: le carte che riempiono i buchi entrano coperte o scoperte? In prova: coperte. Cosa succede oltre un tot di carte in panchina?
- Panchina: quando finisce la partita (es. panchina vuota e nessuna carta coperta)?
- Quale idea si tiene, o se si combinano: dipende dalle prove nelle tre scene.
