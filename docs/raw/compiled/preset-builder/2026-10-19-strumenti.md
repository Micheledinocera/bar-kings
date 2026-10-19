# Strumenti — Odin e Unity CLI

## Odin Inspector: documentazione

- Non esiste un MCP ufficiale di Odin né un `llms.txt` (`odininspector.com/llms.txt` risponde 404, verificato il 2026-10-19).
- Documentazione delle API e degli attributi: https://odininspector.com/documentation
- Da mettere nel wiki tecnico: in `docs/wiki/tecnico/index.md` la frase iniziale rimanda già alle docs ufficiali dei plugin (Odin, DOTween, UniTask, TextMeshPro) senza link; aggiungere lì il link a Odin (e volendo quelli degli altri plugin). Le regole del wiki (`docs/CLAUDE.md`) dicono di citare le docs dei plugin, mai copiarle. Riferimento per chi scrive codice con gli attributi Odin e le `OdinEditorWindow` (es. `DeckPresetBuilderWindow`).

## Unity CLI: rimandata

- Unity ha rilasciato una CLI (`unity`, pacchetto npm `@unity/cli`), ancora in beta: 1.0.0-beta.13 del 2026-10-07. Release notes: https://docs.unity.com/en-us/unity-cli/release-notes
- Potrebbe servire per: `unity recompile` (controllo di compilazione da terminale), `unity test` (test Edit Mode), `unity build` (controllo della build in `/close`), `unity logs`, un suo server MCP (`unity mcp`, `unity setup claude`).
- Per parlare con l'Editor aperto serve il pacchetto `com.unity.pipeline` (pacchetto nuovo = decisione da chiedere); il suo MCP si sovrappone a `unity-mcp`.
- Decisione dell'utente: per ora non si adotta; se ne riparla quando diventa stabile, in una feature apposta (es. `study-unity-cli`).
