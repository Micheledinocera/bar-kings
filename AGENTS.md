# King Bar

**Unity game project — Unity `6000.4.9f1` (see `ProjectSettings/ProjectVersion.txt`), URP, C#.**

## Project structure

```
├── Assets/
│   ├── Scripts/          # project code (C#)
│   │   ├── Shared/       # code shared across scenes: cards, camera, animations, GameManager
│   │   └── Panchina/     # code of the Panchina scene
│   ├── Scenes/           # scenes (.unity)
│   ├── Prefab/           # prefabs
│   ├── Instances/        # asset instances (e.g. card ScriptableObjects in Instances/Cards)
│   ├── Images/           # sprites and textures
│   ├── Materials/        # materials
│   ├── Settings/         # URP and render settings
│   └── Plugins/          # third-party: Odin Inspector (Sirenix), DOTween (Demigiant) — never edit
├── Packages/manifest.json  # Unity packages (UniTask, Input System, URP, 2D, …)
├── ProjectSettings/      # Unity project settings, versioned
└── docs/                 # project wiki, raw notes, guides (see below)
```

`Library/`, `Temp/`, `Logs/`, `UserSettings/` and the generated `*.csproj` / `*.slnx` are produced by Unity: never edit them by hand.

## Writing code

- **Libraries already in use**: Odin Inspector for inspector attributes, DOTween for tweens and animations, UniTask for async (`async UniTask`, not coroutines, in new code), TextMeshPro for text, the new Input System. Prefer them over adding new packages; a new package is a decision to ask about.
- **`.meta` files**: every asset has one, and it holds the GUID that scenes and prefabs reference. Move or rename assets from the Unity Editor (or move the `.meta` with the file), never delete a `.meta` by hand, always commit it with its asset.
- **Scenes, prefabs, ScriptableObject assets** are YAML serialized by Unity: don't edit them by hand unless the change is trivial and you know the format. Say what must be done in the Editor instead.
- **Formatting**: [CSharpier](https://csharpier.com) (local tool in `dotnet-tools.json`): `dotnet tool restore`, then `dotnet csharpier format Assets/Scripts`.
- **Comments and identifiers**: the agent writes them in English. Existing names in Italian or mixed (`CardData.nome`, `valore`, `seme`, …) stay as they are: never flag or rename names chosen by people. Renaming a serialized field needs `[FormerlySerializedAs("oldName")]`. Raw notes and the wiki outside `docs/wiki/tecnico/`: Italian.

## Rules are never blocking

Some contributors are people. The rules below guide the code the agent writes; on code written by people they are applied at `/close` (small safe fixes by the agent, the rest listed in the PR notes), never by stopping work or asking to redo it.

## Scripts

- **One class per file**, file named as the class.
- **Editor code only in `Editor/` folders** (or inside `#if UNITY_EDITOR`). Runtime code never uses `UnityEditor` or `AssetDatabase`: otherwise the player build does not compile. Asset generators run from a menu item, never at runtime, and skip assets that already exist.
- **Shared code, not copies**: what the deck variants (panchina, tribuna, index>40) have in common lives in `Shared/`; a variant folder holds only what differs. The twin scenes are temporary: when one idea is chosen, the others and the code only they use are deleted.
- **Game logic apart from the view**: rules (deck, table, paths) in plain C# classes, without `MonoBehaviour`, DOTween or `Transform`, so they can be tested in Edit Mode. Game state is never read from the hierarchy (`GetChild(i)`).
- **Serialized references over lookups**: `[SerializeField] private` fields (Odin attributes when useful) instead of `public` fields, `Find` or `GetChild(n)`.
- **No mutable static state** (lists, flags, singletons) without a written reason. If needed, reset it in a `[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]` method, so it works with Enter Play Mode without domain reload.
- **Async**: `async UniTask` / `UniTaskVoid`, never `System.Threading.Tasks.Task`; `async void` only as the entry point of a Unity event, calling a `UniTask` method with `.Forget()`. Awaits tied to an object take `destroyCancellationToken`.
- **DOTween**: every tween and `Sequence` gets `SetLink(gameObject)`; durations and eases in serialized fields or a config ScriptableObject.
- **Input**: only the new Input System (the only handler enabled in Player Settings). No `UnityEngine.Input`; actions in `Assets/InputSystem_Actions.inputactions`. `OnMouseDown` in existing code works and stays until that code is rewritten.
- **Named constants** for the game numbers (deck size, king value) and the table layout.
- **No dead code or test scripts** merged into `develop`.

## Unity Editor

- **Prefabs**: one base prefab per entity (card, slot); per-variant differences are **Prefab Variants**, not copies.
- **Scenes**: every scene to try is in the Build Settings. Twin scenes are temporary (see *Scripts*).
- **Hierarchy**: no logic depends on the order of children; GameObjects have meaningful names.
- **Data assets**: ScriptableObjects in `Assets/Instances/<Type>/`, created once from an editor menu item. After regenerating, `git status` must show no GUID change in the `.meta` files.
- **Project Settings not to change without saying so**: Active Input Handling (Input System Package), Enter Play Mode Options (today: reload domain and scene), URP, Unity version.
- **Before committing scenes and prefabs**: save from the Editor, no new errors or warnings in the Console, no `Missing (Script)` or missing prefab references. One feature at a time per scene or prefab (see `CONTRIBUTING.md`).

## Project wiki and workflow

- **Conventions** (branches, commits, PRs): `CONTRIBUTING.md`.
- **Project wiki** (game rules, product, decisions, project code): `docs/wiki/`, starting from `docs/wiki/index.md`. Rules for reading and writing it: `docs/CLAUDE.md`.
- **Order of sources when coding**: this file, then `docs/wiki/tecnico/` for the project's code, then the business and decision pages before implementing a game rule. If a rule is missing, ask instead of inventing it.
- **Decisions not taken**: before writing code that implements a decision still `aperta` or `in analisi` (radar in `docs/wiki/index.md`), stop, name the decision and wait for an explicit ok; warn that the ingest will then record it in the wiki as `presa`, with what was implemented as the choice. Rule: `docs/CLAUDE.md`, *Decisions*.
- **Task workflow**: `/start <name>` opens a feature, notes go in `docs/raw/<name>/`, `/report <topic>` writes a dated document for people in `docs/output/` (git-ignored, temporary, cleaned by hand; the agent never reads that folder), `/close` merges `develop`, updates the wiki (`/ingest`), runs the checks and opens the PR toward `develop`. Analysis-only features are named `study-<topic>`.
- **Study features touch no code**: on a `feature/study-*` branch, change only `docs/`, `.claude/` and `*.md` files. If asked to implement something, don't: propose to close the study (`/close`) and open a new feature (`/start <name>`).
