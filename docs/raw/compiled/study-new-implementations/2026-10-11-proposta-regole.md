# Proposta di regole — codice (script) ed editor Unity

Proposta nata dall'[analisi del codice](./2026-10-11-analisi-codice.md). Da confermare punto per punto con l'utente. Le correzioni dell'utente sono nelle [risposte alle verifiche](./2026-10-11-risposte-verifiche.md).

**Principio sopra tutte le regole**: alcuni contributor sono umani, quindi **nessuna regola è bloccante**. Le regole guidano il codice che scrive l'agent. Su quello scritto dagli umani, al `/close` si sistema ciò che serve, senza fermare nulla e senza chiedere di rifare. Le regole su come si scrive il codice vanno in `AGENTS.md` (`docs/CLAUDE.md`: "how to write code → AGENTS.md"), non nel wiki. Ogni regola cita il problema che evita.

## Script (C#)

1. **Un file per classe, file chiamato come la classe** (`CardView.cs` → `CardView`). Evita i casi `DeckManager.cs` → `DeckManagerPanchina`.
2. **Codice editor solo in cartelle `Editor/`** (o in un asmdef solo editor). Nessun `using UnityEditor` nel codice di runtime; se è inevitabile, dentro `#if UNITY_EDITOR`. Il runtime non chiama mai `AssetDatabase`. Evita la build rotta del blocco A.
3. **Namespace** `KingBar.<Area>` (`KingBar.Shared`, `KingBar.Panchina`, …) e **assembly definition** per area (`KingBar.Shared`, `KingBar.Editor`, `KingBar.Tests`), con dipendenze esplicite (le varianti dipendono da Shared, mai il contrario).
4. **Identificatori**: l'agent scrive i nomi nuovi in inglese. I nomi esistenti in italiano (`nome`, `valore`, `seme`, …) restano, e l'agent non segnala né rinomina i nomi dati dagli umani (decisione dell'utente). Se un campo serializzato viene rinominato, si usa `[FormerlySerializedAs("vecchioNome")]`.
5. **Niente copie per variante**: il comportamento che cambia tra panchina, tribuna e index>40 sta dietro un'interfaccia o una strategia (ScriptableObject); il resto vive in `Shared/`. Una cartella di variante contiene solo ciò che è specifico della variante.
6. **Logica separata dalla vista**: regole di gioco (mazzo, tavolo, path) in classi C# pure, senza `MonoBehaviour`, DOTween o `Transform`, testabili in Edit Mode. I MonoBehaviour mostrano lo stato e passano l'input. Lo stato di gioco non si legge mai dalla gerarchia (`GetChild(i)`).
7. **Riferimenti serializzati, non ricerche**: `[SerializeField] private` (con gli attributi di Odin quando servono) invece di campi `public`; niente `GetChild(n)`, `Find`, `GetComponent` ripetuti a ogni frame o click quando un riferimento basta.
8. **Niente stato statico mutabile** (liste, flag, singleton) salvo una ragione scritta. Se serve, va azzerato esplicitamente (`[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]`) così funziona anche con "Enter Play Mode" senza domain reload.
9. **Async**: `async UniTask` / `UniTaskVoid`, mai `System.Threading.Tasks.Task`; `async void` solo come punto d'ingresso degli eventi Unity, chiamando subito un metodo `UniTask` con `.Forget()`. Ogni attesa legata a un oggetto prende `destroyCancellationToken` (o `GetCancellationTokenOnDestroy()`).
10. **DOTween**: ogni tween e ogni `Sequence` ha `SetLink(gameObject)`; durate ed ease in campi serializzati o in un ScriptableObject di configurazione, non numeri magici.
11. **Input**: solo il nuovo Input System (è l'unico attivo nel progetto). Niente `UnityEngine.Input` né `OnMouseDown`: azioni in `InputSystem_Actions.inputactions` + raycast, oppure `IPointerClickHandler` con `PhysicsRaycaster`.
12. **Costanti con nome** per i numeri delle regole (`DeckSize = 40`, `KingValue = 10`) e per il layout del tavolo.
13. **Niente codice morto né script di prova** nei rami che vanno in `develop`: gli esperimenti si cancellano o restano sul loro ramo.
14. **Formattazione**: CSharpier su tutto `Assets/Scripts/` (già in `AGENTS.md`), dopo una feature dedicata che formatta tutto una volta sola.

## Editor Unity

1. **Prefab**: un prefab base per entità (carta, slot); le differenze per variante sono **Prefab Variant**, non copie. Le modifiche di massa si fanno nel prefab, non sulle istanze in scena.
2. **Scene**: ogni scena da provare va nelle Build Settings. Le scene gemelle sono **temporanee**: servono a provare un'idea senza rompere la precedente; quando un'idea viene scelta le altre si eliminano, insieme al codice che usavano solo loro.
3. **Gerarchia**: nessuna logica dipende dall'ordine dei figli. Root di scena con nomi chiari (`Table`, `Slots`, `Deck`, `Managers`), niente `GameObject` senza nome.
4. **Asset di dati**: ScriptableObject in `Assets/Instances/<Tipo>/`; si generano da un comando di menu editor (una volta, a mano), non a runtime. Dopo una rigenerazione si controlla `git status`: i GUID nei `.meta` non devono cambiare.
5. **Cartelle**: `Assets/Prefabs/` (plurale), `Assets/Scripts/<Area>/` speculare al namespace, `Editor/` e `Tests/` dentro l'area. Si spostano solo dall'Editor (regola dei `.meta` già in `AGENTS.md`).
6. **Project Settings da non toccare senza dirlo** (vanno documentati nel wiki): Active Input Handling = Input System Package; Enter Play Mode Options (oggi ricarica dominio e scena); URP; versione di Unity.
7. **Prima di committare scene e prefab**: salvare dall'Editor (niente modifiche a mano del YAML, già in `AGENTS.md`); Console senza errori o warning nuovi; nessun riferimento mancante (`Missing (Script)`, `Missing Prefab`).
8. **Conflitti sulle scene**: una persona per scena per volta. Il contenuto che si tocca spesso va in un prefab, così il conflitto resta piccolo. Da valutare UnityYAMLMerge (Smart Merge) come merge tool di git.

## Domande per l'utente

- Si adottano tutte o una parte? Namespace e asmdef subito o più avanti?
- ~~Il rename in inglese dei campi di `CardData` si fa ora?~~ No: i nomi restano (vedi regola 4).
- ~~L'architettura modello/vista/variante va bene come direzione?~~ Sì, ordine dei lavori confermato. Va tenuta leggera: le varianti sono temporanee.
- Quali controlli di `/close` restano stop veri, e quali diventano correzioni automatiche o avvisi?
