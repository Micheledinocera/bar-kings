# Implementazione — blocchi G ed E

## G — path che arriva su un buco

- Regola in prova (data dall'utente, non ancora presente nelle specifiche della panchina): se il posto della carta in mano è un **buco**, la carta ci va **scoperta** e il path si interrompe. Lo slot di partenza resta vuoto: diventa un nuovo buco, come quando il path finisce sopra un doppione.
- Codice: `PanchinaTable.PlayPath`, nuovo `PathEnd.FilledHole`. Ordine dei controlli: slot di partenza (chiuso) → buco → slot già scoperto (doppione) → spostamento normale.
- La decisione Modifica del mazzo resta `in analisi`: l'utente non ha detto che la regola è definitiva.
- Verificato nell'Editor via MCP con una tabella di 4 slot: il path che prima andava in errore ora finisce con `FilledHole`.

## E — async e animazioni

- `Start` e `OnMouseDown` (shared e Panchina) chiamano metodi `async UniTaskVoid` con `.Forget()`; i `MoveCards` restituiscono `UniTask`; i token sono `destroyCancellationToken`. Con `Forget()` UniTask ignora l'`OperationCanceledException`, quindi un'animazione interrotta non finisce più in Console come errore.
- `SetLink` su tutte le sequenze e i tween di `CardAnimations` (shared).
- Tolti `FlipAnimation` (shared e Panchina, mai chiamati) e gli `using` inutili.
- `CardDisplay3D.Setup` imposta la texture del fronte con un `MaterialPropertyBlock`: niente più un'istanza di materiale per carta.
- Il click shared ora sblocca i click in un `finally`, come quello di Panchina.

## Play mode via MCP

- L'agent può entrare in Play mode via MCP (`EditorApplication.isPlaying = true` con `Unity_RunCommand`): durante il domain reload il server non risponde per qualche secondo ("Unity not detected"), poi torna.
- Provato in `Panchina`: distribuzione completata (40 carte, click sbloccati), fronti delle carte con le texture giuste nella cattura della camera, Console senza errori.
- Risponde alla domanda aperta della decisione sull'accesso all'Editor: l'agent può entrare in Play, leggere lo stato e catturare la camera; la prova del gameplay (cliccare, giudicare se funziona) resta all'utente.
