# Sistemazione della build (2026-10-16)

## File riscritti dalla build

Due build di fila danno lo stesso identico diff: non è rumore che cambia ogni volta, è Unity che salva valori calcolati da lui. Committati una volta, le build successive lasciano `git status` pulito.

- `Assets/Settings/Universal Render Pipeline Asset.asset`: prefiltering delle varianti shader URP, ricalcolato a ogni build dalle feature usate.
- `Assets/UniversalRenderPipelineGlobalSettings.asset`: lista delle runtime settings URP incluse nel player.
- `ProjectSettings/ProjectSettings.asset`: il batching di default per Standalone (static on, dynamic off). Il `preloadedAssets` con `Assets/InputSystem_Actions.inputactions` invece **non** è stabile: corretto dopo, vedi `2026-10-16-modifiche-non-salvate.md`.
- `ProjectSettings/UnityConnectSettings.asset`: `m_Enabled: 1`. Il progetto è collegato a un progetto cloud (`cloudProjectId`, organizzazione `panzattack`) e l'Editor riaccende il flag: spegnerlo non regge. I singoli servizi (Analytics, Ads, Purchasing, Performance reporting) restano spenti.

Quindi niente skill per "rimettere a posto" i file: sono committati nello stato che la build produce.

## Texture

La causa del peso non era la mancanza di compressione (già richiesta negli import), ma le dimensioni: la compressione BC/DXT vuole lati multipli di 4, e se non lo sono Unity ripiega su texture non compresse.

- 40 fronti delle carte (`Assets/Images/Cards/Fronte/`, 960x1586, 1586 non multiplo di 4): erano RGB24 da 4,4 MB. Ora max size 1024 e compressione alta qualità: 620x1024 BC7, circa 0,6 MB. Restano Sprite (le referenzia `CardData.immagineFronte`). Controllato in Play con una carta ingrandita quasi a tutto schermo: nitida.
- Delle 8 texture `gingham_check_*` solo 3 entrano nella build: `nor_gl` (base map di `Mat_Tavolo`), `rough` (bump di `Mat_Tavolo`), `disp` (bump di `Mat_Carta_Retro`). Erano scalate a 1957x2048 non compresse (circa 53 MB). Ora NPOT scale "nearest" e max 2048: 2048x2048 compresse (DXT5 / BC6H) con mipmap, circa 5,3 MB l'una. `nor_gl` era importata come Sprite: ora Default (usata solo dal materiale).
- Il retro (`Carte_Napoletane_retro.jpg`, 330x525) è piccolo, lasciato com'è.

Risultato: build da **331 MB a 145 MB**. Il resto ora è quasi tutto runtime: `UnityPlayer.dll` 36 MB, `Managed/` 36 MB (Mono e DLL, tra cui Odin), `MonoBleedingEdge` 9 MB, texture circa 41 MB. Scendere ancora vorrebbe dire IL2CPP e stripping del codice: non fatto, da valutare se serve.

## Altro

- Company resta `DefaultCompany` (deciso dall'utente: poi si vede).
- Per far girare il Play mode con l'Editor in background (es. controlli via `unity-mcp`) basta `Application.runInBackground = true` a runtime durante la sessione, senza toccare i Player Settings: utile anche per il blocco H (simulazione che si ferma con l'Editor non a fuoco).
- Le 5 texture `gingham_check_*` non usate non entrano nella build; non cancellate.
