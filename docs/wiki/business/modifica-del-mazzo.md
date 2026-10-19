---
tipo: decisione
stato: in analisi
aggiornato: 2026-10-19
verificato: 2026-10-07
consolidato: 2026-10-19
fonti:
  - raw/compiled/preset-builder/2026-10-19-obiettivo.md
  - raw/compiled/demo-1/2026-10-16-opzioni.md
  - raw/compiled/demo-1/2026-10-16-obiettivo.md
  - raw/compiled/index40/2026-10-16-simulazione-tre-modi.md
  - raw/compiled/index40/2026-10-16-regole.md
  - raw/compiled/index40/2026-10-16-obiettivo.md
  - raw/compiled/stats/2026-10-16-simulazione-preset.md
  - raw/compiled/stats/2026-10-16-sessione.md
  - raw/compiled/stats/2026-10-15-task.md
  - raw/compiled/stats/2026-10-15-obiettivo.md
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

- Tutte e tre le idee hanno una POC giocabile (2026-10-16), da mettere in visione per scegliere.
- Ognuna ha la sua scena di prova: `Assets/Scenes/Panchina.unity`, `Assets/Scenes/Tribuna.unity`, `Assets/Scenes/Index40.unity`. Una mini demo le mette in visione: si parte dalla scena `Assets/Scenes/Menu.unity` con un bottone per ciascuna, e da ognuna si torna al Menu ([tecnico/scenes](../tecnico/scenes.md)). In ogni scena un pannello di opzioni sceglie il preset della prossima partita e la velocità delle animazioni (piano, normale, veloce).
- In tutte e tre si può modificare una carta cambiandone il valore o il seme.
- Le scene gemelle sono **temporanee**: servono a provare un'idea senza rompere la precedente. Quando se ne sceglie una, le altre (e il codice che usano solo loro) si eliminano.
- Stato del codice al 2026-10-16: le tre scene usano lo stesso codice ([tecnico/table](../tecnico/table.md)) e cambiano solo in cosa succede alle carte oltre le 40. Il mazzo si costruisce da un **preset** ([tecnico/deck](../tecnico/deck.md)): i 36 non-re con alcune carte tolte e alcuni doppioni aggiunti (mai re), più i 4 re sempre presenti; quali carte togliere e duplicare si estrae a ogni partita (o sono le prime N, vedi sotto), con un seed che permette di rigiocarla. Preset in prova: 32, 45, 60 e 71 carte non-re (al massimo 40 doppioni; il 71 era un 80 con 49 doppioni, ridotto per stare negli 80 slot di index>40). Panchina parte da 32, tribuna da 71, index>40 da 45. In panchina e tribuna sul tavolo vanno 36 non-re e i 4 re (doppioni e carte mancanti sono casi di test voluti); sotto le 40 carte i buchi sono gli **ultimi** slot (lì non c'è una logica di indice della carta); le carte in eccesso formano il **mazzo panchina** o vengono buttate. Un pulsante Reset rifà la partita senza uscire dal Play.
- **Preset personalizzati** (2026-10-19): oltre ai 4 preset di prova se ne creano altri dall'Editor (`Tools/Preset Builder`, [tecnico/deck](../tecnico/deck.md#deckpresetbuilderwindow)) scegliendo carte tolte, doppioni e come sceglierle; si aggiungono ai preset del pannello opzioni e della simulazione. Primo preset personalizzato: `Prime18` (18 tolte, 18 doppioni, le "prime": 40 carte).
- **Scelta delle carte in prova** (2026-10-19, non decisa; codice: [CardPick](../tecnico/deck.md#cardpick)): ogni preset dice se le carte da togliere e da duplicare sono **casuali** (estratte a ogni partita, come nei 4 preset di prova) o le **prime N** in ordine di tavolo (indice della carta: denari dall'asso in poi, poi coppe, spade, bastoni), quindi sempre le stesse; il tavolo si mescola comunque.
- **Regole in prova nella POC di index>40** (2026-10-16, non decise; fonte: raw/compiled/index40/2026-10-16-regole.md):
  - il tavolo cresce di righe da 10, **al massimo 4** in più (80 slot): ogni carta aggiunta va all'**indice successivo disponibile** (dopo i 40 di base), per ora fisso; il codice prevede già regole di "modifica di indice" in fase di creazione del mazzo;
  - una carta aggiunta va solo al suo slot, mai sull'originale: i doppioni non si sovrappongono;
  - gli slot di base non sono mai meno di 40: le carte tolte lasciano un **buco** nello slot del loro indice;
  - fine della partita come nelle altre scene: quando non restano carte coperte.
- **Dati dalla simulazione** (2026-10-16, autoplay con partenza casuale dei path, strategia di prova; tasto Simula nelle tre scene, [tecnico/stats](../tecnico/stats.md)). Con le regole in prova:
  - **tribuna**: un mazzo più grande conta poco, perché le carte oltre le 40 si buttano e cambia solo quanti doppioni capitano sul tavolo (da ~9 con 45 carte a ~11 con 71); la lunghezza media dei path scende solo da 2,58 a 2,28;
  - **panchina**: con 71 carte i path crollano (media 1,79, 42 path a partita contro 17,7 con 45), perché le carte in eccesso rientrano nei buchi una alla volta;
  - **index>40**: **ogni path si chiude** (ogni carta ha uno slot suo, non incontra doppioni, e nessuna carta ha l'indice di un buco): pochi cicli lunghi, 4–5 path a partita, lunghezza media da 8,8 (32) a 15,2 (71); i 4 re sono tutti scoperti già intorno al 3° path;
  - sotto le 40 carte (32) panchina e tribuna si comportano uguali: niente carte in eccesso, 4 buchi in fondo;
  - in panchina e tribuna i 4 re risultano tutti scoperti intorno al 10°–15° path. Tabella completa: `docs/raw/compiled/index40/2026-10-16-simulazione-tre-modi.md`.
- **Regole in prova nella POC della tribuna** (2026-10-15, non decise): stesso mazzo e stesse regole del path della panchina, ma le carte oltre le 40 vengono **buttate** e i **buchi non si riempiono** (la carta che arriva su un buco ci va scoperta e ferma il path).
- **Regola del path in prova nella POC della panchina** (2026-10-11, non decisa): il path si chiude quando la carta in mano appartiene allo slot di partenza; se invece il posto della carta in mano è già scoperto (c'è il suo doppione), la carta finisce **sopra** il doppione e lo slot di partenza resta un **buco**. Se il posto della carta in mano è un **buco**, la carta ci va **scoperta** e il path si interrompe; anche in questo caso lo slot di partenza diventa un nuovo buco (2026-10-15, non decisa).
- **Riempimento dei buchi in prova nella POC della panchina** (2026-10-15, non deciso): quando un path si ferma, il buco rimasto nello slot di partenza viene riempito dalla prima carta del mazzo panchina, **coperta**; la carta si gioca poi come le altre. Se la panchina è finita si continua senza riempire i buchi. Finché la panchina ha carte i buchi si riempiono subito, quindi una carta arriva su un buco solo a panchina vuota. Fine della partita: non definita.

## Opzioni

| Idea | Carte oltre le 40 | Doppioni | Fine | Effetto sui path |
|---|---|---|---|---|
| **Panchina** | Restano disponibili: ogni volta che si crea un "buco" (un doppione) una carta della panchina lo riempie, coperta (o scoperta?) | Finiscono uno sull'altro | Si gioca finché ci sono buchi da riempire | Li accorcia |
| **Tribuna** | Non disponibili: i buchi non si riempiono. Si abbozzano "forme" fatte dai buchi (l'unica idea compatibile con meno di 40 carte) | Finiscono uno sull'altro | Si gioca fino a 40 carte | Li accorcia |
| **Index>40** | Hanno un indice incrementale (es. un 2 di bastoni aggiunto ha indice 41); si possono creare configurazioni con indici diversi | Non si sovrappongono: hanno indici diversi | In prova: finché restano carte coperte | Li allunga (in prova: ogni path si chiude) |

Panchina: se nella configurazione iniziale delle 36 non escono doppioni, la panchina non si usa (o si inventa altro). Oltre un certo numero di carte in panchina, un'ipotesi è combinare panchina e tribuna.

## Alternative scartate

- **Path fermato su un doppione: la carta in mano va nello slot di partenza** (al posto della carta cliccata) — scartata il 2026-10-11 dopo la prova nella POC: la carta deve finire sopra il suo doppione, lasciando il buco (fonte: raw/compiled/panchina-start/2026-10-11-fix-path.md)

## Domande aperte

- Panchina: la regola in prova per la carta che arriva su un buco (lo riempie scoperta, il path si ferma) è quella definitiva?
- Panchina: le carte che riempiono i buchi entrano coperte o scoperte? In prova: coperte. Cosa succede oltre un tot di carte in panchina?
- Panchina: quando finisce la partita (es. panchina vuota e nessuna carta coperta)?
- Tribuna: con le regole in prova la dimensione del mazzo oltre le 40 carte pesa poco (le carte in più si buttano). Serve un'altra regola perché pesi, per esempio mettere apposta i doppioni sul tavolo invece di pescarli a caso? (La scelta delle prime N carte, in prova, fissa quali carte si duplicano, non dove finiscono.)
- Index>40: con l'indice successivo disponibile ogni path si chiude e la partita sono 4–5 cicli lunghi. Quali regole di "modifica di indice" provare per avere configurazioni diverse?
- Quale idea si tiene, o se si combinano: dipende dalle prove nelle tre scene.
