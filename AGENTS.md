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
- **Comments and identifiers**: English. Raw notes and the wiki outside `docs/wiki/tecnico/`: Italian.

## Project wiki and workflow

- **Conventions** (branches, commits, PRs): `CONTRIBUTING.md`.
- **Project wiki** (game rules, product, decisions, project code): `docs/wiki/`, starting from `docs/wiki/index.md`. Rules for reading and writing it: `docs/CLAUDE.md`.
- **Order of sources when coding**: this file, then `docs/wiki/tecnico/` for the project's code, then the business and decision pages before implementing a game rule. If a rule is missing, ask instead of inventing it.
- **Decisions not taken**: before writing code that implements a decision still `aperta` or `in analisi` (radar in `docs/wiki/index.md`), stop, name the decision and wait for an explicit ok; warn that the ingest will then record it in the wiki as `presa`, with what was implemented as the choice. Rule: `docs/CLAUDE.md`, *Decisions*.
- **Task workflow**: `/start <name>` opens a feature, notes go in `docs/raw/<name>/`, `/report <topic>` writes a dated document for people in `docs/output/` (git-ignored, temporary, cleaned by hand; the agent never reads that folder), `/close` merges `develop`, updates the wiki (`/ingest`), runs the checks and opens the PR toward `develop`. Analysis-only features are named `study-<topic>`.
- **Study features touch no code**: on a `feature/study-*` branch, change only `docs/`, `.claude/` and `*.md` files. If asked to implement something, don't: propose to close the study (`/close`) and open a new feature (`/start <name>`).
