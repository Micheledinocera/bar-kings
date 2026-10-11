# Scelta: CardDatabase invece di Resources.Load

Per far arrivare le 40 carte ai DeckManager senza rigenerarle a runtime si è scelto (con l'ok dell'utente) un asset `CardDatabase` (ScriptableObject) con la lista ordinata delle `CardData`: l'ordine dà l'`index` della carta sul tavolo. Il generatore (solo da menu, in `Assets/Scripts/Shared/Editor/`) crea le carte mancanti e aggiorna il database; i DeckManager hanno un campo serializzato `deckDatabase` da assegnare nelle scene.

Alternativa scartata: caricare le carte con `Resources.Load`. Evita di assegnare il riferimento nelle scene, ma obbliga a spostare gli asset in una cartella `Resources/` e rende il riferimento implicito.

Rimosso `CardGeneratorPanchina`: era identico a `CardGenerator`.
