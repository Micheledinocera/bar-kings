# Popup del fight e statistiche del player — rpg_implementation

Richiesta dell'utente (2026-10-28):

- anche le statistiche iniziali del player si modificano dall'editor (campo `player` di `RpgStatsView`: vita massima, vita iniziale, danno, armatura; default 100, 100, 1, 1);
- dopo ogni fight un popup riassume cosa è successo (esito, turni, danni inflitti e subiti, vita rimasta, calcolo del colpo) e va chiuso ("Continua") per continuare la risoluzione del path.

Trovato provando: un clic su "Continua" faceva partire anche un path dalla carta sotto il pulsante, perché `OnMouseDown` arriva prima che la UI prenda il clic e `IsPointerOverGameObject` lì non basta. Corretto con `TableDeckManager.TablePaused`, acceso mentre il popup è aperto. Lo stesso problema potrebbe valere per il pannello delle opzioni delle altre scene: non verificato.
