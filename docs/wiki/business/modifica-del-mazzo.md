---
tipo: decisione
stato: in analisi
aggiornato: 2026-10-25
verificato: 2026-10-25
consolidato: 2026-10-25
fonti:
  - raw/compiled/art-style/2026-10-25-implementazione.md
  - raw/compiled/preset-buchi/2026-10-23-risultati.md
  - raw/compiled/preset-buchi/2026-10-23-implementazione.md
  - raw/compiled/preset-buchi/2026-10-23-risposte.md
  - raw/compiled/preset-buchi/2026-10-23-obiettivo.md
  - raw/compiled/study-tribuna/2026-10-22-discussione-call.md
  - raw/compiled/study-tribuna/2026-10-22-riassunto-call.md
  - raw/compiled/study-tribuna/2026-10-22-obiettivo.md
  - raw/compiled/study-meccaniche/2026-10-20-tribuna-forme.md
  - raw/compiled/study-meccaniche/2026-10-20-obiettivo.md
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

Come aumentare o modificare il mazzo di base, cioè cosa succede alle carte oltre le 40 (36 di base più i 4 re). Nato dopo la prima POC del solitario dei 4 re (2026-07-01). Tre idee, tutte in prova (vedi *Opzioni*); dal 2026-10-20 l'orientamento è verso la **tribuna** con un bonus sulle **forme dei buchi**. Niente è deciso.

## Cosa sappiamo

### Le scene di prova

- Ogni idea ha la sua scena: `Assets/Scenes/Panchina.unity`, `Assets/Scenes/Tribuna.unity`, `Assets/Scenes/Index40.unity`, tutte giocabili dal 2026-10-16. Una mini demo le mette in visione: si parte da `Assets/Scenes/Menu.unity` con un bottone per ciascuna e da ognuna si torna al Menu ([tecnico/scenes](../tecnico/scenes.md)). Dal 2026-10-25 c'è anche la scena **Bar**, una copia di Tribuna con il primo stile grafico ([Direzione artistica](../arte/direzione-artistica.md)) e la fine al 4° re.
- Le scene gemelle sono **temporanee**: servono a provare un'idea senza rompere la precedente. Quando se ne sceglie una, le altre (e il codice che usano solo loro) si eliminano.
- Le scene usano lo stesso codice ([tecnico/table](../tecnico/table.md)) e cambiano solo in cosa succede alle carte oltre le 40. In ogni scena un pannello di opzioni sceglie il preset della prossima partita e la velocità delle animazioni (piano, normale, veloce); un pulsante Reset rifà la partita senza uscire dal Play; in tutte si può modificare una carta cambiandone il valore o il seme.
- **Mazzo da un preset** ([tecnico/deck](../tecnico/deck.md)): i 36 non-re con alcune carte tolte e alcuni doppioni aggiunti (mai re), più i 4 re sempre presenti, con un seed che permette di rigiocare la partita. Ogni scena parte dal primo preset della libreria, o dal mazzo normale di 40 carte se la libreria è vuota; nella libreria al massimo 40 doppioni, per stare negli 80 slot di index>40. In panchina e tribuna sul tavolo vanno 36 non-re e i 4 re (doppioni e carte mancanti sono casi di test voluti); sotto le 40 carte i buchi sono gli **ultimi** slot (lì non c'è una logica di indice della carta); le carte in eccesso formano il **mazzo panchina** o vengono buttate.
- **Scelta delle carte in prova** (2026-10-19, rivista il 2026-10-23, non decisa; [DeckPreset](../tecnico/deck.md#deckpreset), [CardPick](../tecnico/deck.md#cardpick)): un preset può **scegliere** le carte da togliere e da duplicare, con il numero di copie (le stesse a ogni partita), e/o dire quante altre togliere e duplicare, **casuali** (estratte a ogni partita) o le **prime N** in ordine di tavolo (indice: denari dall'asso in poi, poi coppe, spade, bastoni); il tavolo si mescola comunque. I preset personalizzati si creano dall'Editor (`Tools/Preset Builder`, [tecnico/deck](../tecnico/deck.md#deckpresetbuilderwindow)) su una griglia come il tavolo e vanno nel pannello opzioni e nella simulazione. I quattro preset di partenza (32, 45, 60 e 71 carte non-re) sono stati tolti il 2026-10-23 e sostituiti da preset di prova delle forme ([tecnico/deck](../tecnico/deck.md#deckpreset)).

### Regole del path in prova

Tutte non decise.

- **Path** (panchina, 2026-10-11; vale anche per la tribuna): il path si chiude quando la carta in mano appartiene allo slot di partenza. Se il posto della carta in mano è già scoperto (c'è il suo doppione), la carta finisce **sopra** il doppione e lo slot di partenza resta un **buco**. Se il posto della carta in mano è un **buco** (2026-10-15), la carta ci va **scoperta** e il path si interrompe; anche qui lo slot di partenza diventa un nuovo buco.
- **Panchina, riempimento dei buchi** (2026-10-15): quando un path si ferma, il buco rimasto nello slot di partenza viene riempito dalla prima carta del mazzo panchina, **coperta**; la carta si gioca poi come le altre. Finita la panchina si continua senza riempire i buchi. Finché la panchina ha carte i buchi si riempiono subito, quindi una carta arriva su un buco solo a panchina vuota. Fine della partita: non definita.
- **Tribuna** (2026-10-15): stesso mazzo e stesse regole del path della panchina, ma le carte oltre le 40 vengono **buttate** e i **buchi non si riempiono** (la carta che arriva su un buco ci va scoperta e ferma il path).
- **Index>40** (2026-10-16; fonte: raw/compiled/index40/2026-10-16-regole.md): il tavolo cresce di righe da 10, **al massimo 4** in più (80 slot); ogni carta aggiunta va all'**indice successivo disponibile** (dopo i 40 di base), per ora fisso (il codice prevede già regole di "modifica di indice" in fase di creazione del mazzo); una carta aggiunta va solo al suo slot, mai sull'originale, quindi i doppioni non si sovrappongono; gli slot di base non sono mai meno di 40: le carte tolte lasciano un **buco** nello slot del loro indice; la partita finisce quando non restano carte coperte.
- **Fine dell'incontro al 4° re** (call del 2026-10-22, in prova per tribuna e forme): l'incontro finisce appena sono scoperti tutti e 4 i re, non quando sono girate tutte le carte. La usa sempre la simulazione ([tecnico/stats](../tecnico/stats.md)); tra le scene solo **Bar** (2026-10-25): al 4° re i path si bloccano fino al Reset e un riquadro mostra i path giocati e le forme fatte ([tecnico/table](../tecnico/table.md#tabledeckmanager), opzione `endAtFourthKing`). Panchina, Tribuna e Index40 vanno avanti finché restano carte coperte.

### Orientamento verso la tribuna

- **Studio del 2026-10-20** (study-meccaniche) e **call del 2026-10-22** (study-tribuna; fonti: raw/compiled/study-tribuna/2026-10-22-riassunto-call.md, raw/compiled/study-tribuna/2026-10-22-discussione-call.md): si è **orientati verso la tribuna**, perché è l'idea in cui la lunghezza dei path non "decade velocemente" all'aumentare delle carte (resta quasi costante, vedi i dati sotto), non cambia troppo i numeri e ci si incastrano altre meccaniche. Panchina e index>40 non si scartano e restano in prova.

### Forme dei buchi (tribuna)

Idea dell'utente del 2026-10-20 (fonte: raw/compiled/study-meccaniche/2026-10-20-tribuna-forme.md), precisata nella call del 2026-10-22 e nella feature preset-buchi (2026-10-23; fonti: raw/compiled/preset-buchi/2026-10-23-risposte.md, raw/compiled/preset-buchi/2026-10-23-risultati.md). Non decisa.

- **Il bonus va alle forme dei buchi** (non alle carte scoperte attorno), contate a **fine incontro** sul tavolo finale; i bonus valgono **per tutta la run**. Come implementarli si decide quando ci saranno vere meccaniche di gameplay. Contano solo i buchi effettivi a fine incontro (slot vuoti): uno slot coperto la cui carta manca non è un buco.
- **Le forme sono disegni sulla griglia** 4×10 per indice (riga = seme, nell'ordine denari, coppe, spade, bastoni; colonna = valore), non combinazioni di carte:
  - **colonna**: i 4 semi di un valore (tolte tutte le carte di quel valore; quella dei re non si può fare);
  - **diagonale**: 4 celle, una per riga, su valori consecutivi, in giù o in su;
  - **quadrati** 2×2, 3×3, 4×4 su righe e valori vicini (doppia coppia, 3 tris, 4 poker con gli stessi semi; il 4×4 sono 16 dei 36 non-re). Le forme grandi servono anche a bilanciare;
  - sulla griglia ci sono 74 posizioni: 10 colonne, 14 diagonali, 27 quadrati 2×2, 16 da 3×3, 7 da 4×4. Proposta dell'agent: riga continua = scala dello stesso seme, e il lessico della tombola (ambo, terno, quaterna, cinquina, tombola).
- **Come nascono i buchi**:
  - **ogni path crea al massimo un buco** (nota dell'utente): lo slot di partenza, quando il path si ferma su un doppione o su un buco; se si ferma su un buco lo riempie, quindi il numero di buchi non cambia. Una forma di k slot richiede almeno k path (il 4×4 almeno 16);
  - nei mazzi sotto le 40 carte, senza doppioni, i buchi sono sempre tanti quante le carte mancanti (nessuna carta lascia il tavolo, i buchi si spostano soltanto): conta solo dove finiscono. I buchi "in fondo" esistono solo all'inizio, quindi le forme funzionano anche con meno di 40 carte;
  - **con la fine a carte tutte girate** ogni carta scoperta è nel **suo** slot e i buchi finali sono **esattamente gli slot delle carte che mancano dal tavolo** (tolte dal preset, o originali rimasti fuori perché è uscito un loro doppione): la forma **non dipende da come si giocano i path** ma da quali carte sono sul tavolo, una leva di **costruzione del mazzo** (quali carte togliere o duplicare; più doppioni = più buchi ma path più corti). Durante la partita la cambierebbe solo un'azione che cambia l'indice di una carta (es. valore o seme) o una meccanica futura;
  - per questo **le forme sono troppo facili, non rare** (call del 2026-10-22): lo slot di una carta tolta diventa per forza un buco se un path parte da lì (la sua carta non c'è, il path non si chiude). Tolti i 4 assi, 4 path che partono dai loro slot fanno sempre la colonna;
  - **con la fine al 4° re** restano carte coperte, quindi la regola precedente non vale più: preparare il mazzo non garantisce la forma, che dipende anche da quali path si aprono e in che ordine. È la perplessità principale: forse il 4° re non frena abbastanza le forme piccole (vedi i dati).
- **Pari e dispari**: fante = 8 (pari), cavallo = 9 (dispari).
- **Malus di scagnozzi e capobar** ([Parte RPG](./parte-rpg.md)) su gruppi di carte, es. tutte le carte di un seme o i valori pari/dispari. Nota dell'agent: sono anch'essi insiemi di slot (un seme = una riga, pari/dispari = colonne alterne), quindi bonus e malus parlano la stessa lingua; per pianificare il mazzo vanno annunciati prima dell'incontro.
- **Togliere o trasformare carte**: siccome i buchi diventano centrali, deve succedere **spesso**. Per gli effetti durante l'incontro (es. una "bomba" che distrugge una carta sul tavolo), proposta dell'agent: separare **tavolo** (stato dell'incontro: la bomba fa un buco subito, che conta per le forme) e **mazzo** (stato della run: se la carta esce anche dal mazzo si decide a parte, a fine incontro o con effetti rari "permanenti").
- **Nome**: "forme dei buchi" è provvisorio e fuorviante. Orientamento verso il lessico della tombola: proposta **vincita** (altre: cartella, smorfia); da evitare "figura" (nelle carte italiane sono fante, cavallo e re). Nomi possibili delle singole forme: fila, ola, settore, o ambo/terno/quaterna/cinquina/tombola. Nella scena Bar il riquadro dei buchi si chiama "la cartella".

### Dati della simulazione

Autoplay (tasto Simula, [tecnico/stats](../tecnico/stats.md)), strategie di prova.

- **Le tre idee** (2026-10-16, partenza casuale dei path, fine a carte tutte girate; tabella: `docs/raw/compiled/index40/2026-10-16-simulazione-tre-modi.md`):
  - **tribuna**: un mazzo più grande conta poco, perché le carte oltre le 40 si buttano e cambia solo quanti doppioni capitano sul tavolo (da ~9 con 45 carte a ~11 con 71); la lunghezza media dei path scende solo da 2,58 a 2,28;
  - **panchina**: con 71 carte i path crollano (media 1,79, 42 path a partita contro 17,7 con 45), perché le carte in eccesso rientrano nei buchi una alla volta;
  - **index>40**: **ogni path si chiude** (ogni carta ha uno slot suo, non incontra doppioni, e nessuna carta ha l'indice di un buco): pochi cicli lunghi, 4–5 path a partita, lunghezza media da 8,8 (32) a 15,2 (71); i 4 re sono tutti scoperti già intorno al 3° path;
  - sotto le 40 carte (32) panchina e tribuna si comportano uguali: niente carte in eccesso, 4 buchi in fondo;
  - in panchina e tribuna i 4 re risultano tutti scoperti intorno al 10°–15° path.
- **Forme in tribuna** (2026-10-23, fine al 4° re, 300 partite per preset; autoplay a caso / **mirato**, che parte prima dagli slot delle carte tolte; tabella: `docs/raw/compiled/preset-buchi/2026-10-23-risultati.md`): con i preset casuali (32–71 carte) restano 6–8 buchi e le forme quasi non escono (4–14% delle partite, quasi sempre un 2×2 o una diagonale). Togliendo apposta le 4 carte di una forma (colonna, diagonale, 2×2) la forma esce nel 12–18% delle partite giocando a caso e nel 73–79% giocando mirato; con tanti doppioni quante le carte tolte (40 carte) nel 14–18% e nel 56%. Il 3×3 voluto esce nel 16% / 55% (mazzo senza doppioni), il 4×4 nel 20% / 39%: le forme grandi non si chiudono prima del 4° re, ma ne lasciano di più piccole. Quindi **la fine al 4° re toglie la certezza delle forme piccole** solo a chi gioca a caso: chi punta la forma la prende quasi sempre.

## Opzioni

| Idea | Carte oltre le 40 | Doppioni | Fine | Effetto sui path |
|---|---|---|---|---|
| **Panchina** | Restano disponibili: ogni volta che si crea un "buco" (un doppione) una carta della panchina lo riempie, coperta (o scoperta?) | Finiscono uno sull'altro | Si gioca finché ci sono buchi da riempire | Li accorcia |
| **Tribuna** | Non disponibili: i buchi non si riempiono. Bonus sulle forme dei buchi a fine incontro, validi per la run (in studio; l'unica idea compatibile con meno di 40 carte) | Finiscono uno sull'altro | Scena Tribuna: finché restano carte coperte; simulazione e scena Bar (in prova): al 4° re scoperto | Li accorcia |
| **Index>40** | Hanno un indice incrementale (es. un 2 di bastoni aggiunto ha indice 41); si possono creare configurazioni con indici diversi | Non si sovrappongono: hanno indici diversi | In prova: finché restano carte coperte | Li allunga (in prova: ogni path si chiude) |

Panchina: se nella configurazione iniziale delle 36 non escono doppioni, la panchina non si usa (o si inventa altro). Oltre un certo numero di carte in panchina, un'ipotesi è combinare panchina e tribuna.

## Alternative scartate

- **Path fermato su un doppione: la carta in mano va nello slot di partenza** (al posto della carta cliccata) — scartata il 2026-10-11 dopo la prova nella POC: la carta deve finire sopra il suo doppione, lasciando il buco (fonte: raw/compiled/panchina-start/2026-10-11-fix-path.md)

## Domande aperte

- Panchina: la regola in prova per la carta che arriva su un buco (lo riempie scoperta, il path si ferma) è quella definitiva?
- Panchina: le carte che riempiono i buchi entrano coperte o scoperte? In prova: coperte. Cosa succede oltre un tot di carte in panchina?
- Panchina: quando finisce la partita (es. panchina vuota e nessuna carta coperta)?
- Tribuna: con le regole in prova la dimensione del mazzo oltre le 40 carte pesa poco sui path (le carte in più si buttano). Il bonus sulle forme dei buchi può farla pesare (quali carte mancano decide la forma finale): basta, o serve anche mettere apposta i doppioni sul tavolo invece di pescarli a caso? (La scelta delle prime N carte, in prova, fissa quali carte si duplicano, non dove finiscono.)
- Tribuna: quali forme danno bonus e quanto? I dati di frequenza ci sono (vedi *Dati della simulazione*); manca la scelta.
- Tribuna: con la fine al 4° re le forme piccole restano quasi certe per chi gioca mirato (73–79% con le 4 carte tolte). Basta, o servono forme più grandi, malus, altro?
- Tribuna: le forme si sommano (il 4×4 contiene 4 colonne e quadrati più piccoli) o vale solo la più grande? La stessa forma presa in più incontri somma il bonus? C'è un limite?
- Tribuna: nome del concetto (vincita o altro) e delle singole forme.
- Malus dei boss su gruppi di carte: si applicano a fine incontro o durante? Colpiscono i buchi su quelle carte o le carte scoperte? (In prova: fante 8 pari, cavallo 9 dispari.)
- Un effetto che cambia il mazzo durante l'incontro (la bomba): vale subito sul tavolo, a fine incontro sul mazzo, o tutte e due? Su una carta coperta, il buco è nello slot fisico o in quello del suo indice?
- Togliere o trasformare carte "spesso": quando (dopo ogni incontro, nello shop, come evento) e quanto?
- Index>40: con l'indice successivo disponibile ogni path si chiude e la partita sono 4–5 cicli lunghi. Quali regole di "modifica di indice" provare per avere configurazioni diverse?
- Quale idea si tiene, o se si combinano: dipende dalle prove nelle scene.
