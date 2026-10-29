# Obiettivo — rpg_implementation

Iniziare la parte RPG complementare al solitario: il risultato del giro delle carte comanda la parte RPG. Nuova scena che si integra in modo autonomo con la versione di gioco "Tribuna".

Dopo un ciclo di carte (path) parte il calcolo dell'impatto sulla parte RPG: a ogni carta è associato un effetto, ma per ora si registra solo l'aumento di alcuni valori e lo si logga in console.

Per le carte da 1 a 9 (n = valore della carta), in base al seme:

- Spade: +n al danno
- Denari: +n*1000 Lire ai denari
- Coppe: +n cura
- Bastoni: +n armatura

I 10 (re): per ora si mostra solo a schermo che è successo, senza altri effetti.

Il giocatore ha le statistiche Danno, Vita, Armatura e Denari. La Vita ha un massimo, all'inizio 100: la cura oltre 100 non ha effetto. La vita iniziale è un campo nell'Inspector (default 100).

Decisione toccata: [Parte RPG](../../wiki/business/parte-rpg.md) (in analisi): questi effetti per seme sono l'opzione provata, non la decidono.
