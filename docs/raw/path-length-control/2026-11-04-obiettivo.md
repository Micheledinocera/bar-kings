# Obiettivo — path-length-control

Voglio introdurre una meccanica di controllo della lunghezza del path, soprattutto per i primi path di un incontro: quando iniziamo la partita con il mazzo non alterato la lunghezza media del path è abbastanza lunga e vorrei diminuirla senza che il player si accorga dell'intervento.

Voglio introdurre un sistema che ad ogni carta del path aumenti la probabilità che la prossima sia quella che chiude il ciclo. Ad ora dopo aver selezionato una carta all'inizio della partita si iniziano a girare fino a chiusura ma voglio fare in modo che dopo N (configurabile) si vada a chiudere il path andando a sostituire la carta target con quella che fa chiudere il path se non è già la stessa. Vorrei poter controllare ad ogni passo del ciclo l'incremento percentuale di quanto questa meccanica di accorciamento sia probabile.

Prima di procedere all'implementazione ho bisogno di feedback sulla fattibilità della cosa e sui possibili impatti che essa avrebbe sul gioco, tenendo a mente che l'obiettivo è quello di rendere il gioco più piacevole per il giocatore.

## Pagine collegate

- [Gameplay di base](../../wiki/business/gameplay-di-base.md)
- [Meccaniche della run](../../wiki/business/meccaniche-della-run.md) (decisione in analisi)
- [Modifica del mazzo: panchina, tribuna, index>40](../../wiki/business/modifica-del-mazzo.md) (decisione in analisi)
