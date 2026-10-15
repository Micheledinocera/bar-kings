# Obiettivo — panchina-reprise

Adesso voglio fare un test con più di 40 carte, quindi aggiungere solo doppioni nell'inizializzazione. Come da decisioni prese:

- le carte in eccesso rimangono nel mazzo a disposizione
- quando alla fine del path rimane un "buco" per via di un doppione vengono prese carte dal mazzo "panchina" e riempiono il buco, le carte che riempiono vengono piazzate coperte
- se finiscono le carte dal mazzo "panchina" si continua senza riempire i buchi

Riprende la decisione [Modifica del mazzo: panchina, tribuna, index>40](../../wiki/business/modifica-del-mazzo.md) (`in analisi`) e il codice in [tecnico/panchina](../../wiki/tecnico/panchina.md).
