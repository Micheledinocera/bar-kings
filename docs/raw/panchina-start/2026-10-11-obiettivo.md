# Obiettivo — panchina-start

Adesso possiamo cominciare a implementare le logiche di panchina, partiamo dalle logiche inserite nel file relativo. Il mazzo è già stato implementato con i doppioni per fare i test e c'è un bug ad oggi che "rompe" la path (penso che il problema sia che l'index delle carte nuove non venga letto correttamente).

## Riferimenti

- Decisione in analisi, regole della panchina: [Modifica del mazzo: panchina, tribuna, index>40](../../wiki/business/modifica-del-mazzo.md). Implementarle prende la decisione per la parte panchina: serve l'ok esplicito prima del codice.
- Domande aperte sul path: [Gameplay di base](../../wiki/business/gameplay-di-base.md) (quando si ferma un path, ruolo del re).
- Codice: [panchina](../../wiki/tecnico/panchina.md) (`DeckManagerPanchina`, `CardAnimationsPanchina`), problema noto sullo stato del tavolo letto dalla gerarchia in [known-issues, blocco C](../../wiki/tecnico/known-issues.md#block-c--game-state-in-hierarchy-and-statics).
