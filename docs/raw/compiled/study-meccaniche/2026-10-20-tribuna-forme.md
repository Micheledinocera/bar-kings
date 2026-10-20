# Tribuna e bonus per forme — 2026-10-20

## Idea dell'utente

- Concentrarsi sulla **tribuna**: è l'idea che non fa "decadere velocemente" la lunghezza dei cicli all'aumentare delle carte, il tutto resta abbastanza costante (simulazione del 2026-10-16: media dei path da 2,58 con 45 carte a 2,28 con 71).
- Restano "parecchi" slot vuoti (buchi): potenziale per aggiungere **bonus in relazione alle forme** create dai buchi.

## Proposte dell'agent (da discutere, non decise)

- Il tavolo è già una griglia 4×10 per indice: **riga = seme, colonna = valore**. Le forme hanno quindi un significato da gioco di carte: riga continua = scala dello stesso seme, colonna = stesso valore in tutti i semi, blocchi 2×2, diagonali.
- **Scelta del giocatore**: il buco nasce nello slot di partenza del path quando il path si ferma su un doppione. Scegliere da quale carta partire vuol dire scegliere dove può nascere un buco: le forme diventano abilità, non solo fortuna.
- **Il mazzo torna a pesare** (domanda aperta della tribuna): più doppioni = più buchi possibili = più forme, ma path più corti. Doppioni scelti apposta = forme mirate.
- **Tema "bar"**: punteggio alla tombola (ambo, terno, quaterna, cinquina, tombola) sui buchi in una riga.
- ~~Da sistemare prima: oggi con meno di 40 carte i buchi stanno negli ultimi slot, cioè una riga di buchi gratis.~~ Superata: vale solo all'inizio della partita, vedi sotto.

## Risposte dell'utente (2026-10-20)

- Per ora il bonus va sulle **forme dei buchi**. Come implementarli si capisce dopo, quando si cominciano a implementare vere meccaniche di gameplay.
- Le forme si contano **a fine partita**, sul tavolo finale: così si hanno le forme "finali" e si risolve anche il problema delle meno di 40 carte.

## Dove finiscono i buchi (osservazione dell'utente, 2026-10-20)

- I buchi "in fondo" ci sono solo all'inizio della partita. Con le regole in prova della tribuna ogni carta scoperta va sempre nel **suo** slot (path chiuso, carta su un buco o sopra il suo doppione), e la partita finisce quando non restano carte coperte. A fine partita quindi i buchi sono **esattamente gli slot delle carte che mancano dal tavolo**: le carte tolte dal preset e gli originali rimasti fuori perché al loro posto è uscito un doppione.
- Il problema dei buchi negli ultimi slot sotto le 40 carte non esiste a fine partita: non serve spostarli.
- Conseguenza (corregge la proposta sopra sulla scelta del giocatore): la forma finale **non dipende da come si gioca i path**, ma solo da quali carte sono sul tavolo, cioè dal preset e dalla distribuzione. Le forme diventano una leva di **costruzione del mazzo** (quali carte togliere o duplicare), non di abilità durante la partita. Durante la partita la forma cambia solo con azioni che cambiano l'indice di una carta (es. modificarne valore o seme) o con meccaniche future.

## Domande aperte

- Va bene che la forma finale dipenda solo dal mazzo, o serve una scelta del giocatore durante la partita che la cambi?
