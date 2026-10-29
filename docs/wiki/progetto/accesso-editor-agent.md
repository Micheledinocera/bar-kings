---
tipo: decisione
stato: presa
aggiornato: 2026-10-29
verificato: 2026-10-15
consolidato: 2026-10-16
fonti:
  - raw/compiled/gemini/2026-10-29-setup-antigravity.md
  - raw/compiled/preset-builder/2026-10-19-strumenti.md
  - raw/compiled/build/2026-10-16-modifiche-non-salvate.md
  - raw/compiled/build/2026-10-16-sistemazione.md
  - raw/compiled/panchina-continue/2026-10-15-implementazione.md
  - raw/compiled/card-generator/2026-10-11-modifiche-editor.md
  - raw/compiled/unity-mcp/2026-10-15-obiettivo.md
  - raw/compiled/unity-mcp/2026-10-15-decisione-mcp.md
  - raw/compiled/unity-mcp/2026-10-15-test-e-controllo.md
---

# Accesso dell'agent all'Unity Editor

Come l'agent fa modifiche nell'Editor e verifica il progetto (compilazione, Console) invece di passare ogni volta dall'utente.

## Decisione

- L'agent accede all'Editor tramite il **server MCP ufficiale di Unity**: pacchetto `com.unity.ai.assistant` (`Packages/manifest.json`, impostazioni in `ProjectSettings/Packages/com.unity.ai.assistant/`), relay `relay_win.exe --mcp` registrato in Claude Code come server `unity-mcp` o in Antigravity tramite plugin locale `.agents/plugins/unity/`.
- La registrazione è per macchina (scope utente di Claude Code, o cartella locale ignorata da git per Antigravity): ogni persona la fa una volta (`claude mcp add unity-mcp --scope user -- "<home>/.unity/relay/relay_win.exe" --mcp` o configurazione in `.agents/plugins/unity/mcp_config.json`).
- Funziona solo con l'Editor aperto sul progetto.
- **Regola** (`AGENTS.md`, *Writing code*): le modifiche a scene, prefab e ScriptableObject passano da `unity-mcp` (`Unity_RunCommand`), **mai** dallo YAML a mano.
  - Le modifiche all'Editor di una feature si elencano a parte rispetto al codice; l'agent le esegue **solo dopo la conferma** dell'utente. Leggere e verificare (Console, catture, gerarchia) non richiede conferma.
  - Dopo una modifica l'agent salva scena o prefab dall'Editor e legge la Console; dopo modifiche al codice controlla da solo la compilazione.
  - **Niente di non salvato**: alla fine di una sessione di lavoro sull'Editor, e prima dei controlli di `/close`, l'agent salva tutto (`EditorSceneManager.SaveOpenScenes()`, `AssetDatabase.SaveAssets()`) e guarda `git status`, invece di fidarsi dell'asterisco: Unity segna spesso una scena modificata senza cambiarla. Un file che cambia davvero e che l'agent non voleva toccare va mostrato all'utente, mai committato alla cieca.
  - MCP non collegato: l'agent chiede di aprire l'Editor, non ripiega sullo YAML.
  - I comandi `Unity_RunCommand` sono usa e getta, non si committano; le operazioni ripetibili (generatori di asset) restano voci di menu in `Editor/`, che l'agent lancia via MCP.
  - Project Settings protetti, pacchetti e `Assets/Plugins/`: si chiede prima.
- `/close` legge da solo la Console (`.claude/skills/close/SKILL.md`); all'utente resta la prova in Play mode (vedi [Definition of done](./definition-of-done.md)).

## Strumenti e limiti

| Strumento | Uso |
|---|---|
| `Unity_RunCommand` | Esegue una classe C# `CommandScript : IRunCommand` nell'Editor: modifiche, ispezione della scena, prove della logica di gioco |
| `Unity_GetConsoleLogs` | Errori, warning e log della Console |
| `Unity_Camera_Capture`, `Unity_SceneView_*` | Catture per verificare a colpo d'occhio |
| `Unity_AssetGeneration_*` | Generazione di asset (non usata finora) |

- **Cancellazioni bloccate**: `result.DestroyObject` e `AssetDatabase.DeleteAsset` falliscono ("User interactions are not supported for MCP tool calls"). GameObject: `Undo.DestroyObjectImmediate`. Asset e cartelle: si cancellano dal disco insieme al `.meta`, poi `AssetDatabase.Refresh()` via MCP.
- Warning innocui in Console a ogni connessione: "Failed to collect Windows signature" per `claude.exe`.
- `Unity_RunCommand` segnala le API deprecate di Unity 6.4 (es. `GetInstanceID()` → `GetEntityId()`, `FindFirstObjectByType` → `FindAnyObjectByType`).
- Prova del 2026-10-15: proposta della modifica, conferma, esecuzione, salvataggio e verifica dell'utente nell'Editor funzionano.
- **Play mode**: l'agent ci entra con `EditorApplication.isPlaying = true` via `Unity_RunCommand`; durante il domain reload il server non risponde per qualche secondo ("Unity not detected"), poi torna. In Play può leggere lo stato del gioco, la Console e catturare la camera; giocare e giudicare il gameplay resta all'utente. Con l'Editor non a fuoco il Play non avanza (fermo al frame 1): `Application.runInBackground = true` a runtime, durante la sessione, lo fa girare senza toccare i Player Settings.
- **Comando in sospeso a lungo**: di solito una finestra di dialogo si è aperta nell'Editor e aspetta un clic dell'utente. Non si riprova: si chiede all'utente cosa dice. Meglio non lanciare comandi Unity in parallelo a operazioni git su file serializzati da Unity, e tenere build e salvataggi in comandi separati.

## Perché

- L'utente ha scelto l'MCP di Unity e ha detto esplicitamente che la decisione è presa (2026-10-15).
- È il server ufficiale per Unity 6.4 e lavora sull'Editor aperto: Console, gerarchia, salvataggio, senza un clic dell'utente per ogni modifica.
- Le modifiche fatte da Unity sono corrette per costruzione; lo YAML a mano funzionava solo per le modifiche più semplici (primo caso: `deckDatabase` assegnato in `Index40` e `Tribuna` in card-generator).

## Alternative scartate

- **Modifiche facili nello YAML dopo conferma, il resto istruzioni passo passo per l'utente** — scartata il 2026-10-15: era la regola provvisoria in attesa di un accesso diretto; con l'MCP non serve più (fonte: raw/compiled/unity-mcp/2026-10-15-decisione-mcp.md)
- **Script editor da menu lanciati dall'utente come modo di modificare l'Editor** — scartata il 2026-10-15: serviva un clic dell'utente per ogni modifica; restano solo per i generatori di asset, che ora lancia l'agent (fonte: raw/compiled/unity-mcp/2026-10-15-decisione-mcp.md)
- **Unity in batchmode per il lavoro sull'Editor** — scartata il 2026-10-15: funziona solo a Editor chiuso e l'MCP copre già compilazione e Console; resta un'opzione solo per la build (fonte: raw/compiled/unity-mcp/2026-10-15-decisione-mcp.md)

## Domande aperte

- Il pacchetto è una pre-release (`2.20.0-pre.2`): passare alla versione stabile quando esce.
- **Unity CLI** (`unity`, pacchetto npm `@unity/cli`, in beta: 1.0.0-beta.13 del 2026-10-07, [release notes](https://docs.unity.com/en-us/unity-cli/release-notes)): potrebbe servire per `unity recompile` (controllo di compilazione da terminale), `unity test`, `unity build` nel check di `/close`, `unity logs`, e ha un suo server MCP (`unity mcp`, `unity setup claude`) che si sovrappone a `unity-mcp`. Per parlare con l'Editor aperto serve il pacchetto `com.unity.pipeline` (pacchetto nuovo: da decidere). Per ora non si adotta: se ne riparla quando diventa stabile, in una feature apposta (es. `study-unity-cli`) (utente, 2026-10-19).
