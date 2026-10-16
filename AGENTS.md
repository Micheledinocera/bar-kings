# King Bar

**Unity game project — Unity `6000.4.9f1` (see `ProjectSettings/ProjectVersion.txt`), URP, C#.**

## Project structure

```
├── Assets/
│   ├── Scripts/          # project code (C#)
│   │   └── Shared/       # code shared across scenes: cards, card generator, GameManager
│   │       ├── Deck/     # deck presets and deal of the table scenes (seeded)
│   │       ├── Menu/     # scene loading: Menu and Back buttons
│   │       ├── Stats/    # game log and autoplay simulation (CSV)
│   │       └── Table/    # table of the deck variants (Panchina, Tribuna, Index40 scenes)
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
- **Editor changes go through `unity-mcp`**: scenes, prefabs and ScriptableObject assets are YAML serialized by Unity, and the agent changes them only from the Editor through the `unity-mcp` server (`Unity_RunCommand`), **never by editing the YAML by hand**.
  - Every Editor change of a feature is listed **separately** from the code changes; the agent runs it **only after the user confirms** that list. Reading and checking (Console, scene captures, inspecting the hierarchy) need no confirmation.
  - After a change, the agent saves the scene or prefab from the Editor and checks the Console (`Unity_GetConsoleLogs`); after changing code, it checks the Console for compilation errors itself.
  - The Editor must be open on the project: if `unity-mcp` is not connected or does not respond, ask the user to open it; never fall back to the YAML.
  - `unity-mcp` refuses deletions that would ask for confirmation (`result.DestroyObject`, `AssetDatabase.DeleteAsset`): remove GameObjects with `Undo.DestroyObjectImmediate`; delete an asset or folder from disk together with its `.meta`, then `AssetDatabase.Refresh()` through `unity-mcp`.
  - Commands run through `Unity_RunCommand` are throwaway: they are not committed. Repeatable operations (asset generators) are menu items in `Editor/` (see *Scripts*), which the agent can run through `unity-mcp`.
  - Out of bounds without asking, as before: the protected Project Settings (*Unity Editor*), packages, `Assets/Plugins/`.
  - New `.meta` files for scripts and folders the agent creates: let Unity generate them (asset refresh through `unity-mcp`); write one by hand only if the Editor is unavailable: `fileFormatVersion: 2` and a new random `guid` (folders also `folderAsset: yes` and the `DefaultImporter` block).
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
- **Decisions not taken**: the game is in a POC phase, so code that implements a rule of a decision still `aperta` or `in analisi` (radar in `docs/wiki/index.md`) is a trial and does **not** take the decision: name the decision before writing that code, and the ingest records what was implemented as the option being tried. A decision becomes `presa` only when the user says explicitly that a rule is fixed. Rule: `docs/CLAUDE.md`, *Decisions*.
- **Task workflow**: `/start <name>` opens a feature, notes go in `docs/raw/<name>/`, `/report <topic>` writes a dated document for people in `docs/output/` (git-ignored, temporary, cleaned by hand; the agent never reads that folder), `/close` merges `develop`, updates the wiki (`/ingest`), runs the checks and opens the PR toward `develop`. Analysis-only features are named `study-<topic>`.
- **Study features touch no code**: on a `feature/study-*` branch, change only documentation: any `*.md` file anywhere (wiki, raw, `AGENTS.md`, `CONTRIBUTING.md`, skills, templates) and anything under `docs/` or `.claude/`. Rules, skills and docs that a study decides are updated in the study itself, not left for later. If asked to implement something, don't: propose to close the study (`/close`) and open a new feature (`/start <name>`).
