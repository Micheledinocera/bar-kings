# Test dell'MCP e controllo del codice

## Test di funzionamento (ok)

- Processo provato: l'agent propone la modifica all'Editor, l'utente conferma, l'agent la esegue via `unity-mcp`, salva e controlla la Console, l'utente verifica nell'Editor. Prova: una scritta TextMeshPro "Test MCP" in `Panchina`, poi rimossa.
- L'agent verifica anche da solo con `Unity_Camera_Capture` (cattura dalla Main Camera).

## Limiti scoperti dell'MCP

- Le cancellazioni dall'Editor sono bloccate ("User interactions are not supported for MCP tool calls"): `result.DestroyObject` e `AssetDatabase.DeleteAsset` falliscono.
  - GameObject: funziona `Undo.DestroyObjectImmediate`.
  - Asset e cartelle: l'agent li cancella dal disco insieme al loro `.meta`, poi `AssetDatabase.Refresh()` via MCP.
- Warning innocui del pacchetto in Console: "Failed to collect Windows signature" per `claude.exe` a ogni connessione.
- Unity 6.4 segnala `Object.GetInstanceID()` come deprecato (sostituito da `GetEntityId()`).

## Sistemato in questa feature

- Rimossi i `.meta` di `Assets/Scripts/Index40` e `Assets/Scripts/Tribuna` (cartelle già cancellate).
- `Panchina.unity` risalvata: ora contiene `dealMoveDuration` e `pathMoveDuration` di `DeckManagerPanchina`.
- Build Settings: aggiunte `Panchina`, `ScenaTribuna`, `ScenaIndex40` (prima c'era solo `SampleScene`).
- Warning "The referenced script (Unknown) on this Behaviour is missing!": nessun riferimento mancante in scene, prefab e asset; probabilmente transitorio all'avvio, prima della compilazione del nuovo pacchetto.

## Da sistemare nei prossimi rilasci

- **Bug — path che arriva su un buco**: `PanchinaTable.PlayPath` lancia `ArgumentOutOfRangeException` quando la carta in mano appartiene a uno slot rimasto vuoto (buco lasciato da un path finito su un doppione): prende la carta in cima a una pila vuota. Riprodotto nell'Editor con una tabella di 4 slot. Nel gioco capita facilmente (9 doppioni). Manca la regola: cosa fa la carta che arriva su un buco (lo riempie scoperta e il path si chiude?). Decisione collegata: modifica del mazzo, `in analisi`. File: `Assets/Scripts/Panchina/PanchinaTable.cs`.
- **Async**: `DeckManagerPanchina.MoveCards` restituisce `System.Threading.Tasks.Task` invece di `UniTask`. File: `Assets/Scripts/Panchina/DeckManager.cs`.
- **Cancellazione nei path**: se la scena viene distrutta durante un'animazione, l'`OperationCanceledException` risale fino agli `async void` (`Start`, `OnMouseDown`) e finisce in Console come errore. File: `Assets/Scripts/Panchina/DeckManager.cs`, `Assets/Scripts/Panchina/CardController.cs`.
- **Pacchetto pre-release**: `com.unity.ai.assistant` è `2.20.0-pre.2`; passare alla versione stabile quando esce.

Prossimo passo deciso dall'utente: una feature di sviluppo per il bug del buco e il resto, dopo il `/close` di questa.
