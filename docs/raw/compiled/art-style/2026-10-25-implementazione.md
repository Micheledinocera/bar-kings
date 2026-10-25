# Implementazione — art-style

Primo stile grafico del gioco nella scena nuova **Bar** (`Assets/Scenes/Bar.unity`), copia di Tribuna. Scelta dell'utente: una scena nuova basata sulla tribuna, così se lo stile piace si continua solo su questa. Dal Menu ci si arriva con il pulsante "Bar (stile)"; la scena è nelle Build Settings dopo Index40. Le altre scene non sono toccate.

Base: palette Nicotina e mix delle tavole 1 e 3 della seconda prova del design (vedi `2026-10-25-scelte-design.md`).

## Scelte dell'utente

- **Reskin del 3D**, non rifare in 2D: restano tavolo, carte 3D e animazioni DOTween; la UI va sul Canvas.
- **Carte napoletane di oggi**, non le carte segnaposto del design.
- **Scontrino e strada mostrano solo le carte**, nessun evento (gli eventi delle carte non sono decisi, vedi parte RPG).
- **Segnaposto** per tutto quello che andrà aggiunto con la parte RPG.
- Dopo la prima prova:
  - la strada dell'omino più bassa, per dare spazio al resto;
  - i buchi non sono veri buchi: niente bruciatura, si lascia la **sagoma tratteggiata della carta** come nella tavola 1;
  - lo scontrino si scrive **una riga per volta** mentre le carte si girano, e l'omino cammina sulla strada allo stesso ritmo;
  - il gioco **si ferma al 4° re** scoperto.
- Risoluzione di riferimento **16:9**.

## Cosa c'è nella scena

- **Tavolo**: panno verde `#3D4A2A` con grana di tessuto (normal map `Assets/Images/fabric.jpg`), bordo in legno, camera quasi dall'alto (75°), sfondo Buio, una sola lampada calda sopra il tavolo, Global Volume con vignettatura e grana (al posto del rumore e dell'ombra interna del design).
- **Buchi**: sagoma tratteggiata della carta (prefab variante `CardSlot Bar`, materiale `Mat_Sagoma`).
- **Barra in alto**: "King Bar" in Caveat Brush; i pulsanti di prima (Reset, Simula, Log, Opzioni, Back) spostati nella barra e ricolorati.
- **Colonna destra**: scontrino "BAR SPORT" (carte del path in ordine e come si è chiuso), "Re scoperti N di 4" con le corone, la cartella della tombola (un pallino per buco, cerchiato in oro se fa parte di una forma completa, e la riga con le forme fatte).
- **Strada in basso**: le carte del path appese alla parete una alla volta, l'omino che ci cammina sotto.
- **Fine al 4° re**: un riquadro "Quarto re!" con i path giocati e le forme fatte; nessun path fino a Reset. Solo nella scena Bar (opzione `endAtFourthKing` del gestore del tavolo): le altre scene vanno avanti finché restano carte coperte. È la regola "fine dell'incontro al 4° re" di Modifica del mazzo, **in prova**: la decisione resta in analisi.
- **Segnaposto**: avventori dietro il tavolo (disegno del design), post-it "Il capobar dice", spazi vuoti per santini e amari, soldi/vita/armatura nella barra, "eventi e nemici: in arrivo" sulla strada.
- Il log della partita parte nascosto: lo apre il pulsante Log.
- Font: Caveat Brush (titoli, OFL) e Special Elite (testo, Apache 2.0), da Google Fonts, in `Assets/Fonts/` con le licenze; font asset TextMeshPro statici (ASCII e lettere accentate), così non cambiano a ogni Play.
- Disegni dal design come SVG in `Assets/Images/Bar/` (omino, corona, strada, sagoma, avventori), importati dal modulo vector graphics di Unity come sprite.

## Prove fatte

In Play (Game view 16:9): i pallini della cartella coincidono con i buchi del tavolo; scontrino e strada si scrivono carta per carta (catturato a metà path e alla fine); in due partite a caso il 4° re è uscito al path 25 e al 21, e nella seconda, con 4 carte ancora coperte, un altro click è stato ignorato. Console pulita.

## Da tenere a mente

- La Game view non 16:9 stringe il layout: la UI scala sull'altezza.
- A scala ridotta le righe della cartella vengono un po' irregolari (linee di 1-2 pixel).
- Re scoperti e cartella si aggiornano a fine path, non carta per carta.
