# Riassunto della sessione — rpg_implementation (2026-10-28)

Punto di partenza per una conversazione futura su questa feature. Dettagli delle richieste nelle altre note della cartella: `2026-10-28-obiettivo-rpg.md`, `2026-10-28-fight.md`, `2026-10-28-popup-e-config.md`.

## Stato del lavoro

- **Ramo**: `feat/rpg_implementation` (creato dall'utente, non `feature/*`: le skill `/ingest` e `/close` si aspettano `feature/<nome>`, da chiarire prima di chiudere). L'utente ha chiesto di **non fare operazioni git**: niente è committato.
- **git-flow** non è installato su questa macchina (`git flow` assente, `gitflow.branch.develop` non configurato).
- **unity-mcp** registrato e funzionante (relay `C:/Users/Luigi/.unity/relay/relay_win.exe --mcp`).
- **Wiki**: non ancora aggiornato con `/ingest`, salvo una domanda aperta aggiunta a mano in `docs/wiki/business/parte-rpg.md` (momento del fight nel path). Mancano: pagina `tecnico/` per `Assets/Scripts/Shared/Rpg/`, scena `TribunaRpg` in `tecnico/scenes.md`, `EndEncounter`/`TablePaused` in `tecnico/table.md`, cartella `Rpg/` nella struttura di `AGENTS.md`.
- File modificati o nuovi (git status): `Assets/Scenes/Menu.unity`, `Assets/Scenes/TribunaRpg.unity`, `ProjectSettings/EditorBuildSettings.asset`, `Assets/Scripts/Shared/Table/TableDeckManager.cs`, `Assets/Scripts/Shared/Rpg/` (13 script), `docs/wiki/business/parte-rpg.md`, `docs/raw/rpg_implementation/`. `Assets/Instances/DeckPresets/Standard.asset` è nuovo ma **non creato dall'agent**: chiedere all'utente se fa parte della feature.

## Cosa è stato fatto

### Scena `TribunaRpg`

- `Assets/Scenes/TribunaRpg.unity`: copia di Tribuna (Tribuna resta invariata), in fondo alle Build Settings; nel Menu un pulsante `TribunaRpgButton` ("Tribuna RPG") sotto "Tribuna", con i 5 pulsanti riavvicinati (y = 140, 15, -110, -235, -360).
- Sotto `UI`:
  - `RpgStats` (in basso a destra, 440×300): testi `Statistiche` ed `Eventi`, componente `RpgStatsView`;
  - `FightPopup` (ultimo figlio, a schermo intero, inattivo all'avvio): sfondo che blocca i raycast, `Riquadro` 700×450 con `Titolo`, `Riepilogo` (auto-size 16–26) e `Continua` (istanza di `Assets/Prefab/OptionButton.prefab`), componente `FightPopup`.
- Il mazzo è quello di Tribuna: `deckPreset` vuoto → primo preset di `DeckPresetLibrary` (oggi `NO2-NO3+20-1`), `excessCards = Discard`, `endAtFourthKing` spento.

### Regole provate (decisione [Parte RPG](../../wiki/business/parte-rpg.md), in analisi: nessuna è presa)

- **Effetti delle carte 1–9** (n = valore), alla fine di ogni path, nell'ordine del path (la mossa dalla panchina non conta):
  - Spade +n danno; Denari +n×1000 Lire; Coppe +n vita (oltre il massimo non ha effetto); Bastoni +n armatura.
  - Toggle nell'Inspector per disattivare le cure delle Coppe e l'armatura dei Bastoni.
- **Statistiche del player** (Inspector, `Player`): vita massima 100, vita iniziale 100, danno 1, armatura 1. Reset riparte da questi valori.
- **Re (10) → fight**: statistiche uguali per tutti i re (Inspector, `King`): vita 10, armatura 0, danno 1, nuove per ogni fight.
  - **Clock** non a tempo (`FightClock`): un turno a testa per giro, prima il player poi il re. Pensato per una futura statistica di velocità (più turni prima del re), **non implementata**.
  - **Colpo**: danno dell'attaccante − armatura del bersaglio, minimo 0; l'armatura **non si consuma** (scelta dell'utente).
  - **Momento**: il fight parte quando il re si incontra nel path; le carte prima si applicano, quelle dopo solo se il re è battuto. Scelta provvisoria, segnata come punto aperto nel wiki (alternativa: fight a fine path).
  - **Sconfitta** (vita a 0): l'incontro finisce (`TableDeckManager.EndEncounter`), il tavolo ignora i click fino a Reset, le carte dopo il re non si applicano.
  - **Pareggio**: se nessuno dei due passa l'armatura dell'altro il fight finisce subito, il gioco continua. **Scelta dell'agent, non ancora confermata dall'utente.**
  - **Popup** dopo ogni fight: esito, turni, danni inflitti e subiti, vita rimasta, calcolo del colpo; va chiuso con "Continua" per continuare la risoluzione del path (un popup per ogni re del path).
- Tutto è loggato in Console (ogni carta, ogni turno, statistiche a fine path); a schermo statistiche ed esito dei fight.

### Codice

`Assets/Scripts/Shared/Rpg/` (logica in classi C# semplici, vista separata):

| Classe | Ruolo |
|---|---|
| `RpgRules` | Costanti di default (player, re, Lire per denaro) |
| `PlayerConfig`, `KingConfig` | Campi `[Serializable]` dell'Inspector; creano `PlayerStats` / `KingFighter` |
| `IFighter` | Nome, vita, vita massima, armatura, danno, `TakeDamage` |
| `PlayerStats` | Statistiche del player (anche soldi), `Heal` con tetto |
| `KingFighter` | Un re in un fight |
| `CardEffects` | `IsKing`, `Apply` (effetto per seme, rispetta i toggle) |
| `FightClock` | Ordine dei turni |
| `Fight`, `FightResult`, `FightOutcome` | Fight intero in un colpo: esito, righe dei turni, danni totali |
| `FightPopup` | Popup: `Show(...)` è un `UniTask` che finisce al clic su "Continua" |
| `RpgStatsView` | Ascolta `GameStarted` / `PathPlayed` di `TableDeckManager`, risolve il path in modo asincrono (`ResolvePath`) |

`Assets/Scripts/Shared/Table/TableDeckManager.cs`: aggiunti `EndEncounter()` (chiude l'incontro da fuori; chiamato durante `PathPlayed` fa scattare anche `EncounterEnded`) e `TablePaused` (il tavolo ignora i click mentre il popup è aperto).

## Problemi trovati

- **Clic sul popup che passa al tavolo**: `OnMouseDown` (`TableCardController`) arriva prima che la UI prenda il clic, e `IsPointerOverGameObject` lì non basta: "Continua" faceva partire anche un path dalla carta sotto. Corretto con `TablePaused`. **Da verificare**: lo stesso problema potrebbe valere per il pannello Opzioni delle altre scene.
- **Ricompilare durante il Play** azzera `TableDeckManager.instance` (problema già noto nel wiki): uscire dal Play prima di cambiare il codice.
- Per provare i fight via `unity-mcp`: chiamare `TableDeckManager.PlayPath` per riflessione su un re coperto (un path che parte dal re lo contiene sempre); `SendMessage("OnMouseDown")` non funziona.

## Da fare / domande aperte

- Confermare la regola del **pareggio**.
- Provare in Play a mano: lettura del pannello e del popup, sconfitta e Reset, toggle di cure e armatura (provati solo in Edit Mode).
- Decidere il **momento del fight** (al re o a fine path) e se la **vita persiste** tra incontri.
- Velocità e turni multipli nel clock (idea per dopo).
- Chiarire il ramo per `/close` (`feat/rpg_implementation` contro `feature/*`) e il file `Standard.asset`; poi `/close` (ingest, controlli, PR verso `develop`).
