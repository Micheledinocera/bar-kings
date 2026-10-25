---
tipo: info
aggiornato: 2026-10-25
verificato: 2026-10-25
fonti:
  - raw/compiled/art-style/2026-10-25-implementazione.md
  - raw/compiled/art-style/2026-10-25-scelte-design.md
---

# HUD scripts

Scripts in `Assets/Scripts/Shared/Hud/`: the HUD of the Bar scene (`Assets/Scenes/Bar.unity`, [scenes](./scenes.md#bar)), the first art style being tried ([Direzione artistica](../arte/direzione-artistica.md), decision in analysis). Every view gets a serialized `tableDeckManager` and only **listens** to the events of [TableDeckManager](./table.md#tabledeckmanager) (`GameStarted`, `PathStarted`, `MovePlayed`, `PathPlayed`, `EncounterEnded`) and reads its `Table`: none changes the game. They subscribe in `OnEnable` and unsubscribe in `OnDisable`. Only the cards are shown: the events of the cards are not decided ([Parte RPG](../business/parte-rpg.md)). Texts are ASCII, as [GameLogFormatter](./stats.md#gamelogformatter) (the Bar font assets are static: a missing character would not be added).

| Class | Path | Kind |
|---|---|---|
| [PathReceipt](#pathreceipt) | `Assets/Scripts/Shared/Hud/PathReceipt.cs` | static class |
| [ReceiptView](#receiptview) | `Assets/Scripts/Shared/Hud/ReceiptView.cs` | MonoBehaviour |
| [KingCounterView](#kingcounterview) | `Assets/Scripts/Shared/Hud/KingCounterView.cs` | MonoBehaviour |
| [TombolaCard](#tombolacard) | `Assets/Scripts/Shared/Hud/TombolaCard.cs`, `Assets/Scripts/Shared/Hud/TombolaCell.cs` | plain C# class, enum |
| [TombolaCardView, TombolaCellView](#tombolacardview-tombolacellview) | `Assets/Scripts/Shared/Hud/TombolaCardView.cs`, `Assets/Scripts/Shared/Hud/TombolaCellView.cs` | MonoBehaviour |
| [PathRoadView, RoadStopView](#pathroadview-roadstopview) | `Assets/Scripts/Shared/Hud/PathRoadView.cs`, `Assets/Scripts/Shared/Hud/RoadStopView.cs` | MonoBehaviour (DOTween) |
| [EncounterEndView](#encounterendview) | `Assets/Scripts/Shared/Hud/EncounterEndView.cs` | MonoBehaviour (DOTween) |

## PathReceipt

`Assets/Scripts/Shared/Hud/PathReceipt.cs`. Text of the receipt, in pieces so it can be written while the path plays: `Header(pathNumber)` ("scontrino non fiscale - path n. N" and a dashed line), `Line(number, card)` (" 1  7 SPADE"), `Footer(moves)` (how the path ended, "CICLO CHIUSO", "FINITO IN UN BUCO" or "SU UN DOPPIONE", and the number of cards; a bench card filling the starting slot on a line of its own), `Empty` (before the first path). `CardLabel(card)`: "ASSO DENARI", "7 SPADE", "FANTE/CAVALLO/RE COPPE".

- **Used by**: [ReceiptView](#receiptview), [RoadStopView](#pathroadview-roadstopview) (`CardLabel`).

## ReceiptView

`Assets/Scripts/Shared/Hud/ReceiptView.cs`. On `Scontrino` (right column of the Bar HUD). `PathStarted` opens a new receipt (header), `MovePlayed` adds a line for each card as it is turned (the bench move is skipped), `PathPlayed` writes the footer; `GameStarted` shows `Empty`. Serialized: `tableDeckManager`, `text` (the TMP `Righe`, auto-size 10–17 for long paths).

## KingCounterView

`Assets/Scripts/Shared/Hud/KingCounterView.cs`. On `ReScoperti`: "Re scoperti N di 4" and one crown per king (`crowns`, 4 Images with the `Corona` sprite), lit with `faceUpColor` (Nicotina) for each king face up (`TableState.FaceUpKings`), else `faceDownColor`. Refreshed on `GameStarted` and `PathPlayed`: at the **end** of a path, not card by card (the table state is already final when the path starts).

## TombolaCard

`Assets/Scripts/Shared/Hud/TombolaCard.cs`. Plain C#: the tombola card from a [TableState](./table.md#tablestate). `Cells` (40, slot order, base grid only; slots beyond 40 are ignored): `TombolaCell.Empty` (a card), `Hole`, `ShapeHole` (a hole of a complete shape). `CompleteShapes`: [TableShapes](./stats.md#tableshape-tableshapes-shapekind)`.Complete` with every hole counted (no path history). `Summary()`: count per shape kind ("colonna: 2 | quadrato 2x2: 3"), or "nessuna forma, per ora". `TombolaCell` (`Assets/Scripts/Shared/Hud/TombolaCell.cs`) is the enum.

- **Used by**: [TombolaCardView](#tombolacardview-tombolacellview), [EncounterEndView](#encounterendview).

## TombolaCardView, TombolaCellView

`Assets/Scripts/Shared/Hud/TombolaCardView.cs` on `Cartella`: `cells` (40 [TombolaCellView](#tombolacardview-tombolacellview), instances of `Assets/Prefab/TombolaCell.prefab` under `Griglia`, a `GridLayoutGroup` of 10 columns whose dark red background draws the grid lines), `summary` (TMP `Forme`). Refreshed on `GameStarted` and `PathPlayed` (end of the path); the cell order is the serialized array, not the hierarchy.

`Assets/Scripts/Shared/Hud/TombolaCellView.cs` on the prefab root: `Show(cell)` enables `dot` for a hole and `ring` (gold) for a hole of a complete shape.

## PathRoadView, RoadStopView

`Assets/Scripts/Shared/Hud/PathRoadView.cs` on `Strada` (the road at the bottom of the Bar HUD, 220 px high). `PathStarted`: clears the road and places a hidden stop per card of the path (`stopPrefab`, `Assets/Prefab/RoadStop.prefab`, children of `road`, spaced by `maxSpacing` or less to fit long paths, `margin` from the edges). `MovePlayed`: shows the stop of that card and walks the `walker` (`Omino`) to it with a hop (`stepDuration`, `stepEase`, `hopHeight`; DOTween `Sequence`, `SetLink`). A walk still going on is killed and the stops before are lit, so at the fast speed the omino keeps up with the table. `GameStarted`: clears the road.

`Assets/Scripts/Shared/Hud/RoadStopView.cs` on the prefab root: the card sprite (`CardData.immagineFronte`), its label ([PathReceipt](#pathreceipt)`.CardLabel`), `SetReached` (alpha of a `CanvasGroup`: `notReachedAlpha` until the omino arrives).

## EncounterEndView

`Assets/Scripts/Shared/Hud/EncounterEndView.cs` on `FineIncontro` (a panel over the table, "Quarto re!"). `EncounterEnded`: writes the paths played and the shapes made ([TombolaCard](#tombolacard)) in `body` and fades the `group` (`CanvasGroup`) in (`fadeDuration`, `fadeEase`); `GameStarted` hides it. The panel stays active with alpha 0; it blocks raycasts only while shown. The rule is in [TableDeckManager](./table.md#tabledeckmanager) (`endAtFourthKing`).
