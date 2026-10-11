# Analisi del codice — stato al 2026-10-11

Analisi di `Assets/Scripts/` (16 file, ~450 righe), delle scene, dei prefab e delle impostazioni del progetto, prima di partire con le POC delle 3 idee (panchina, tribuna, index>40). Nessun codice modificato (feature `study-`).

## Inventario

### Chi usa cosa

| Script | Classe | Usato in |
|---|---|---|
| `Assets/Scripts/Shared/Cards/CardData.cs` | `CardData` (ScriptableObject) + enum `Seme` | 40 asset in `Assets/Instances/Cards/`, tutti gli script |
| `Assets/Scripts/Shared/Cards/CardDataInGame.cs` | `CardDataInGame` (classe C#: `CardData` + `index` = posto sul tavolo) | tutti i DeckManager |
| `Assets/Scripts/Shared/Cards/CardGenerator.cs` | `CardGenerator` (EditorWindow, menu `Tools/Genera Carte Napoletane`) | chiamato a runtime da `DeckManager3D.Start` |
| `Assets/Scripts/Shared/Cards3D/CardDisplay.cs` | `CardDisplay3D` | prefab `3DCard` e `3DCardPanchina` |
| `Assets/Scripts/Shared/Cards3D/CardController.cs` | `CardController` (click → path) | prefab `3DCard` |
| `Assets/Scripts/Shared/Cards3D/DeckManager.cs` | `DeckManager3D` | scene `SampleScene`, `ScenaIndex40`, `ScenaTribuna` |
| `Assets/Scripts/Shared/Animations/CardAnimations.cs` | `CardAnimations` (statica, DOTween) | `CardController`, `DeckManager3D` |
| `Assets/Scripts/Shared/GameManager.cs` | `GameManager` (solo `static bool isClickBlocked`) | GameObject in tutte e 4 le scene |
| `Assets/Scripts/Shared/Camera/CameraOrbit.cs` | `CameraOrbit` | **nessuna scena** |
| `Assets/Scripts/Panchina/DeckManager.cs` | `DeckManagerPanchina` | scena `Panchina` |
| `Assets/Scripts/Panchina/CardController.cs` | `CardControllerPanchina` | prefab `3DCardPanchina` |
| `Assets/Scripts/Panchina/CardAnimations.cs` | `CardAnimationsPanchina` | `CardControllerPanchina`, `DeckManagerPanchina` |
| `Assets/Scripts/Panchina/CardGenerator.cs` | `CardGeneratorPanchina` (EditorWindow, menu `Tools/Genera Carte Napoletane Panchina`) | chiamato a runtime da `DeckManagerPanchina.Start` |
| `Assets/Scripts/Panchina/CardSlot.cs` | `CardSlot` (`int index`) | 40 slot della scena `Panchina` (campo mai letto dal codice) |
| `Assets/Scripts/Panchina/CardSlotContainer.cs` | `CardSlotContainer` (`List<GameObject> CardSlotList`) | scena `Panchina` (lista mai letta dal codice) |
| `Assets/Scripts/ScriptProva.cs` | `ScriptProva` | **nessuna scena**: script di prova |

### Scene e prefab

- `SampleScene`, `ScenaIndex40` e `ScenaTribuna` sono **identiche byte per byte**: le due scene gemelle sono copie della prima POC (solitario dei 4 re base con `DeckManager3D`). Non contengono ancora nulla di specifico per la loro idea.
- `Panchina` è l'unica scena lavorata: tavolo, `SlotContainer` con 40 istanze del prefab `CardSlot`, `DeckManagerPanchina` (il componente in YAML ha ancora `m_EditorClassIdentifier: Assembly-CSharp::DeckManager3DTest`, un nome precedente della classe).
- Build Settings: c'è solo `SampleScene`.
- `3DCardPanchina.prefab` è una **copia** di `3DCard.prefab` (non un Prefab Variant): differiscono solo per il controller.

### Flusso attuale

1. `Start` del DeckManager blocca i click (`GameManager.isClickBlocked`), **rigenera i 40 asset carta** chiamando il CardGenerator (codice editor) e riempie liste statiche.
2. Costruisce il mazzo:
   - `DeckManager3D` mescola le 40 carte;
   - `DeckManagerPanchina` toglie i 4 re (valore 10), aggiunge come doppioni sempre asso–nove di denari, mescola, tiene 36 carte, aggiunge i re e rimescola.
3. Istanzia le carte a pila e le anima una a una sulla griglia 4×10 (posizioni calcolate a mano, `-5 + 1.2*(i%10)`, …). Panchina sposta anche ogni carta come figlia dello slot `i`.
4. Click su una carta coperta: parte il path (`MoveCards`), la carta va al posto del suo `index`, la carta che c'era si gira e va al suo posto, e così via. Nella panchina il path si ferma anche su una carta già girata.

## Problemi trovati

Tag come in `docs/CLAUDE.md` (*Known issues*). Raggruppati per blocco, cioè per feature di codice che li sistemerebbe.

### Blocco A — Generazione delle carte e codice editor (bloccante per qualunque build)

- `bug` — `CardGenerator` e `CardGeneratorPanchina` usano `UnityEditor` e stanno fuori da una cartella `Editor/`: **la build del player non compila**. Oggi non emerge perché si prova solo in Play mode.
- `bug` — Il generatore viene chiamato a runtime in `Start`: a ogni Play ricrea i 40 `.asset` in `Assets/Instances/Cards/` con `AssetDatabase.CreateAsset`. È lento e riscrive asset versionati. Verificato: contenuto e GUID restano uguali, ma i file risultano modificati in `git status` (vedi [risposte](./2026-10-11-risposte-verifiche.md)).
- `debt` — Seme e valore sono dedotti dall'**ordine** dei file restituito da `Directory.GetFiles` (`i / 10`, `i % 10`), che non è garantito ordinato. Funziona perché i file hanno il prefisso `01_`…`40_` e NTFS li restituisce in ordine alfabetico; serve un ordinamento esplicito o il parsing del nome.
- `duplicate` — I due generatori sono identici: cambiano solo le liste statiche che riempiono. Scrivono negli stessi asset.

### Blocco B — Duplicazione Shared / Panchina

- `duplicate` — `Panchina/` copia `CardAnimations`, `CardController`, `CardGenerator` e `DeckManager` di `Shared/` con suffisso `Panchina`. Con tribuna e index>40 diventerebbero 3 copie di tutto: ogni fix va fatto 3 volte.
- `duplicate` — I prefab `3DCard` e `3DCardPanchina` sono copie, non Prefab Variant.
- `debt` — `Shared/` non è davvero condiviso: `DeckManager3D` e `CardController` sono la prima POC, usati solo dalle scene gemelle.

### Blocco C — Stato di gioco nella gerarchia e nelle statiche

- `debt` — Lo stato del tavolo (quale carta sta in quale slot) è letto dalla **gerarchia dei Transform**: `deckPosition.GetChild(0).GetChild(index)` e poi `slot.GetChild(1)`, che dipende dal fatto che il `Cube` sia il figlio 0. Durante un path le carte si muovono ma **non cambiano padre**: dopo il primo path la gerarchia non corrisponde più a dove sono le carte. Oggi il path si ferma comunque sulla carta già girata, ma qualunque regola che legga "cosa c'è nello slot X" dopo un path legge il dato sbagliato. Da verificare nel gioco.
- `dead-code` — `CardSlot.index` e `CardSlotContainer.CardSlotList` non sono mai letti: il codice usa l'ordine dei figli. I valori di `index` nella scena non seguono l'ordine dei figli, ed è voluto (test della panchina).
- `debt` — Tutto è statico e pubblico (`cardDatabase`, `deckCards`, `instance`, `GameManager.isClickBlocked`) e non viene mai svuotato. Oggi è innocuo perché in Project Settings il Play mode ricarica dominio e scena (`m_EnterPlayModeOptions: 0`). Se si passa a "Do not reload Domain" per avere un Play più veloce, le liste si raddoppiano a ogni Play. Stesso problema se si caricano due scene di fila.
- `debt` — `GameManager` è un MonoBehaviour messo nelle scene ma contiene solo un campo statico.
- `debt` — Il singleton `DeckManagerPanchina.instance` non si azzera in `OnDestroy` e non distrugge i duplicati.

### Blocco D — Input

- `to-verify` — Il progetto usa **solo il nuovo Input System** (`activeInputHandler: 1`). `CameraOrbit` usa il vecchio `Input.GetMouseButton` / `Input.GetAxis`: in Play lancerebbe `InvalidOperationException`. Oggi non è in nessuna scena.
- `to-verify` — I click passano da `OnMouseDown`, che dipende dal vecchio sistema di input. Nella scena panchina sembra funzionare: va verificato, e comunque conviene un input esplicito (Input System + raycast, o `IPointerClickHandler` con `PhysicsRaycaster`). `Assets/InputSystem_Actions.inputactions` esiste ma non è usato.

### Blocco E — Async e animazioni

- `debt` — `async void` in `Start` e `OnMouseDown`, `System.Threading.Tasks.Task` invece di `UniTask` (contro la regola di `AGENTS.md`). Solo `MoveCards` di `DeckManager*` passa un cancellation token; il path del click no, quindi se la scena si chiude a metà animazione i tween continuano o lanciano eccezioni.
- `bug` — In `MoveCardsAtom` e nel resto del path le `Sequence` non hanno `SetLink`, a differenza di `MoveAnimation`.
- `dead-code` — `FlipAnimation` (in entrambe le classi) non è mai chiamata. Restano parametri commentati, `using System` / `System.Linq` inutilizzati e la variabile `alzata` ripetuta in ogni metodo.
- `performance` — `CardDisplay3D.Setup` usa `meshRenderer.material`, che crea un'istanza di materiale per carta, mai distrutta. Accettabile per una POC; meglio `MaterialPropertyBlock`, o un materiale per carta creato una volta.

### Blocco F — Pulizia e convenzioni

- `dead-code` — `ScriptProva.cs` e `CameraOrbit.cs` (non usati).
- `debt` — Nome del file diverso dal nome della classe: `CardDisplay.cs` → `CardDisplay3D`, `DeckManager.cs` → `DeckManager3D`, `Panchina/DeckManager.cs` → `DeckManagerPanchina`, `Panchina/CardController.cs` → `CardControllerPanchina`, `Panchina/CardAnimations.cs` → `CardAnimationsPanchina`, ecc. In Console non compare nessun avviso (verificato): non è un bug, solo una trappola per chi cerca una classe.
- `debt` — Nessun namespace, nessun assembly definition (`.asmdef`): tutto finisce in `Assembly-CSharp`, che si ricompila per intero a ogni modifica.
- Identificatori misti italiano/inglese: **non è un problema**, restano così (decisione dell'utente, vedi [risposte](./2026-10-11-risposte-verifiche.md)).
- `debt` — Numeri magici: layout della griglia, `DECK_LIMIT = 40` come campo non `const`, `9` doppioni, `valore == 10` per il re.
- `debt` — CSharpier: 15 file su 16 non formattati (già in [Definition of done](../../wiki/progetto/definition-of-done.md)).
- `debt` — Nomi delle scene non uniformi (`Panchina` contro `ScenaTribuna` / `ScenaIndex40`); la cartella si chiama `Prefab` (singolare).

## Da modificare prima di partire (proposta di priorità)

1. **Blocco A** (generatore in `Editor/`, asset carta caricati da un riferimento serializzato, per esempio un ScriptableObject `DeckDefinition` con la lista delle 40 `CardData`, invece di rigenerarli): sblocca le build e smette di riscrivere asset.
2. **Blocco B + C insieme**: estrarre un **nucleo condiviso** e rendere le 3 idee varianti di una sola regola, invece che copie (vedi sotto). È il punto che conviene fare prima di iniziare tribuna e index>40, altrimenti le copie diventano 3.
3. **Formattazione + convenzioni** (blocco F) in una feature dedicata, prima delle altre, per non mescolare il diff di formattazione con quello delle feature. Risponde a una domanda aperta della Definition of done.
4. Blocchi D ed E dentro il refactor del punto 2 (input e async si riscrivono comunque).

### Architettura proposta per le 3 POC

- **Modello in C# puro** (niente MonoBehaviour, niente DOTween), testabile con Edit Mode test:
  - `Deck`: costruzione e mescolata;
  - `Table`: 40 posti, quale carta sta dove;
  - `PathResolver`: dato un posto, la sequenza delle mosse finché il path si ferma.
- **Regola della variante** dietro un'interfaccia o una strategia (`IDeckVariant` o un ScriptableObject per variante): come si costruisce il mazzo oltre le 40 carte, cosa succede ai doppioni, quando si ferma il path. Una per panchina, una per tribuna, una per index>40. È esattamente la decisione [Modifica del mazzo](../../wiki/business/modifica-del-mazzo.md) ancora `in analisi`: il codice la tiene aperta finché le prove non scelgono.
- **Vista** (MonoBehaviour): `CardView` / `SlotView` / `TableView` leggono il modello e riproducono le mosse con DOTween + UniTask. Un solo prefab carta, eventualmente con varianti.
- **Una scena per variante** che cambia solo la regola assegnata nell'Inspector, oppure una scena unica con un selettore.

Da confermare con l'utente: è una scelta di struttura del codice, non ancora presa.

## Da scrivere nel wiki tecnico (all'ingest)

Pagine proposte in `docs/wiki/tecnico/` (in inglese), secondo i formati di `docs/CLAUDE.md`:

- `shared.md`: una sezione `##` per classe di `Assets/Scripts/Shared/`;
- `panchina.md`: una sezione per classe di `Assets/Scripts/Panchina/`;
- `scenes.md`: le 4 scene, cosa contengono, il fatto che 3 sono identiche, Build Settings, prefab `3DCard` / `3DCardPanchina` / `CardSlot`, asset `Assets/Instances/Cards/`;
- `known-issues.md`: i blocchi A–F sopra;
- eventualmente una pagina di **impostazioni del progetto** rilevanti per il codice (Input System only, Enter Play Mode, render pipeline URP, cartelle) in `progetto/` o `tecnico/`.

## Domande aperte

Risposte in [2026-10-11-risposte-verifiche.md](./2026-10-11-risposte-verifiche.md).
