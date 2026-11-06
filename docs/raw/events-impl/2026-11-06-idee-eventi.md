# Eventi

Aggiungi alla ui una parte parametrica del pg (hp,atk,armor, soldi) e segna le righe e le colonne perchè magari cominciamo ad implementare qualcosa relativo allo switchare "indici" (se non c'è aggiungi al glossario il fatto che l'indice è il posto dove andrebbe la carta - ad esempio il 2 di denari è di default all'indice 2, etc)

# iterazione 1 effetti

Allora la mia idea è lasciare parametrico fino al 7
- gli 8 aprono uno shop (per adesso sempre lo stesso più avanti capire come legarlo al seme)
- per i 9 fanno fare un qualcosa relativo alle forme, magari permettono di switchare due posizioni (ad esempio dove andrebbe il 2 di denari ci va il 3 e viceversa)
- i 7 non fanno niente ma quando si trovano tutti e 4 si riceve un bonus
- Le carte di denaro ti danno soldi, e quando giri tutte le carte di denari (compreso il RE) hai un bonus in soldi

## I 9 (cavalli): alternative valutate

Dubbio sullo scambio di due indici: chiede una scelta di due posizioni a metà path e si sovrappone alle operazioni sul mazzo (scambia due carte/valori/semi) previste per amari e shop tra un incontro e l'altro. Alternative proposte dall'agent, a tema cavallo:

1. **Velocità**: un turno in più o il primo colpo contro il prossimo re (si aggancia alla statistica Velocità già annotata e a `FightClock`). Piace.
2. **Fuga**: salta il prossimo combattimento con un re. Piace, **si implementa per prima**.
3. Sbirciata: scopre per un attimo le carte coperte della riga/colonna.
4. **Salto**: il path salta la prossima carta, che resta coperta. Piace: **da tenere in considerazione più avanti se serve qualcosa per allungare/controllare i path**.
5. Carica: doppio danno al prossimo re dello stesso path.
6. Cavallo come forma: effetto con due cavalli nello stesso path o riga.

## Fuga (iterazione 1, in prova)

- Il cavallo dà **solo una fuga**, nessuna statistica (parametrico fino al 7). San Francesco (cavallo applicato due volte) dà 2 fughe.
- **A scelta**: quando arriva un re e si ha almeno una fuga, il giocatore sceglie "Scappa" o "Combatti" (può tenersi la fuga per un re più forte).
- Le fughe si **accumulano per tutto l'incontro** (Reset le azzera); il re da cui si scappa è perso, nessun combattimento, ma conta comunque tra i 4 re scoperti.
