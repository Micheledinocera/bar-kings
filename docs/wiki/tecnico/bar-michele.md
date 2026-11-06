---
tipo: info
aggiornato: 2026-11-06
verificato: 2026-11-06
fonti:
  - raw/compiled/events-impl/2026-11-06-obiettivo.md
  - raw/compiled/events-impl/2026-11-06-task.md
  - raw/compiled/events-impl/2026-11-06-idee-eventi.md
---

# Bar-Michele scripts

Scripts in `Assets/Scripts/BarMichele/`, used only by the test scene `Assets/Scenes/Bar-Michele.unity` ([scenes](./scenes.md#bar-michele)): Michele's playground to try the **card events** of the RPG part ([Parte RPG](../business/parte-rpg.md#cosa-sappiamo), decision in analysis: everything here is a rule being tried). Ideas that work are meant to be merged later, with those of other branches, into one final feature; the scene and these scripts are then deleted or folded back into `Shared/`.

Every class is marked `TEMP Bar-Michele` in its header comment. Four are **copies** of shared classes with a few differences (`AGENTS.md` asks for shared code, not copies: the duplication is deliberate, decided by the user to try freely without touching the other scenes; listed in [known issues](./known-issues.md)). The rest of the RPG part is reused from [RPG scripts](./rpg.md) (`CardEffects`, `Fight`, `PlayerStats`, `KingFighter`, santini, amari, `FightPopup`, `InventoryView`) and the HUD from [HUD scripts](./hud.md). Texts are ASCII, as [GameLogFormatter](./stats.md#gamelogformatter).

| Class | Path | Kind | Copy of |
|---|---|---|---|
| [BarRpgView](#barrpgview) | `Assets/Scripts/BarMichele/BarRpgView.cs` | MonoBehaviour | [RpgStatsView](./rpg.md#rpgstatsview) |
| [BarEvents](#barevents) | `Assets/Scripts/BarMichele/BarEvents.cs` | static class | — |
| [ChoicePopup](#choicepopup) | `Assets/Scripts/BarMichele/ChoicePopup.cs` | MonoBehaviour | — |
| [BarReceiptView](#barreceiptview) | `Assets/Scripts/BarMichele/BarReceiptView.cs` | MonoBehaviour | [ReceiptView](./hud.md#receiptview) |
| [BarPathReceipt](#barpathreceipt) | `Assets/Scripts/BarMichele/BarPathReceipt.cs` | static class | [PathReceipt](./hud.md#pathreceipt) |
| [BarEncounterEndView](#barencounterendview) | `Assets/Scripts/BarMichele/BarEncounterEndView.cs` | MonoBehaviour (DOTween) | [EncounterEndView](./hud.md#encounterendview) |

## BarRpgView

`Assets/Scripts/BarMichele/BarRpgView.cs`. The RPG part of Bar-Michele, on `UI/BarHud/Rpg`. Copy of [RpgStatsView](./rpg.md#rpgstatsview) (same stats, fights, santini, amari, Inspector fields) with these differences:

- **Card by card**: sets [TableDeckManager](./table.md#tabledeckmanager)`.MoveResolver` to `ResolveMove`, so each card is resolved as soon as it is turned and the path waits for it (`PathStarted` opens the path, `PathPlayed` writes the summary). The bench move is skipped.
- **Card events** ([BarEvents](#barevents)): cards 1–7 go to [CardEffects](./rpg.md#cardeffects) (a seven also counts toward the bonus of the four sevens, `CountSeven`, with a [FightPopup](./rpg.md#fightpopup) at the fourth); a horse (9) gives escapes; a fante (8) asks shop or skip in the [ChoicePopup](#choicepopup) (`ResolveFante`; the shop is only a message in the `FightPopup`); a king always asks escape or fight (`AskEscape`, `Escape`: an escape is free while escapes are left, otherwise it costs `BarEvents.EscapePenalty` of the Lire).
- **Defeat**: `ResolveMove` returns `false`, which stops the path and ends the encounter in `TableDeckManager`; no fight popup. `DefeatText` (public, null until a defeat, reset at every game) tells [BarEncounterEndView](#barencounterendview) what happened.
- **Stats panel**: `statsText` is `BarHud/ColonnaSinistra/Giocatore/Statistiche`, two columns: Vita, Danno, Armatura, Fughe, Lire, Sette n/4. `eventsText` is `BarHud/Strada/Eventi`.
- **Popups**: every popup (`Ask`, `ShowPopup`) sets `TableDeckManager.TablePaused` while open.
- **Serialized fields**: as RpgStatsView plus `choicePopup` (`[FormerlySerializedAs("escapeChoicePopup")]`); in the scene `player`, `king` and `inventory` copy the TribunaRpg values (armour 0, San Francesco, San Matteo, Cerottino Martini, Amaro Affilato).
- **Uses**: [BarEvents](#barevents), [ChoicePopup](#choicepopup), [FightPopup](./rpg.md#fightpopup), [InventoryView](./rpg.md#inventoryview), [CardEffects](./rpg.md#cardeffects), [Fight](./rpg.md#fight-fightclock-fightresult-fightoutcome), `PathReceipt.CardLabel` ([PathReceipt](./hud.md#pathreceipt)).
- **Used by**: [BarEncounterEndView](#barencounterendview) (`DefeatText`).

## BarEvents

`Assets/Scripts/BarMichele/BarEvents.cs`. Plain static rules of the card events being tried, with the numbers as named constants:

- `MaxParametricValue` 7 (cards up to 7 raise a stat), `FanteValue` 8, `RpgRules.HorseValue` 9.
- **Horse**: `IsHorse`, `EscapesFrom(card, santini)` = 1 + the extra applications of the santini (San Francesco gives 2).
- **King escape**: `EscapePenaltyLirePercent` 25, `EscapePenalty(lireOwned)`.
- **Fante**: `IsFante`, `LirePerGame` = (1+…+7) × `RpgRules.LirePerDenaro` = 28 000, `SkipShopLire` = `LirePerGame / SkipShopShareDivisor` (4) = 7000. Santini bonuses are not counted.
- **Sevens**: `IsSeven`, `SevensForBonus` 4, `SevensBonus` 14 (twice a seven), `ApplySevensBonus(stats)`: +14 damage, +14 armour, heal 14, +14 000 Lire.
- `AllCardsTurned(table)`: every slot of the [TableState](./table.md#tablestate) is face up or a hole.
- **Used by**: [BarRpgView](#barrpgview), [BarEncounterEndView](#barencounterendview) (`AllCardsTurned`).

## ChoicePopup

`Assets/Scripts/BarMichele/ChoicePopup.cs`. On `UI/ChoicePopup` (last child, full screen, inactive while hidden; same look as `UI/FightPopup` of Bar-Michele). `Ask(title, body, firstLabel, secondLabel, cancellationToken)` (`UniTask<bool>`) writes the texts and the two button labels, shows the popup and completes with `true` for the first button, `false` for the second.

- **Serialized fields**: `title`, `body`, `firstButton` (`PrimaScelta`), `secondButton` (`SecondaScelta`); the buttons are instances of `Assets/Prefab/OptionButton Bar.prefab`. The old field names (`escapeButton`, `fightButton`) are kept with `FormerlySerializedAs`.
- **Used by**: [BarRpgView](#barrpgview).

## BarReceiptView

`Assets/Scripts/BarMichele/BarReceiptView.cs`. On `BarHud/ColonnaDestra/Scontrino`. Copy of [ReceiptView](./hud.md#receiptview) that keeps the receipts of the game: while a path is played the receipt follows it live ("path N in corso", arrows off, the scroll view at the bottom); between paths the arrows `Precedente` / `Successivo` browse the receipts of the game ("2 di 3", the scroll view at the top). Reset clears them.

- **Serialized fields**: `tableDeckManager`, `text` (`Scorrimento/Righe`), `previousButton`, `nextButton`, `pageText` (`Pagina`), `scroll` (`Scorrimento`, a `ScrollRect` with `RectMask2D`; `Righe` has a `ContentSizeFitter` and a fixed font size, so a long path scrolls instead of shrinking).
- **Uses**: [BarPathReceipt](#barpathreceipt) for lines and footer, [PathReceipt](./hud.md#pathreceipt) for header and empty text.

## BarPathReceipt

`Assets/Scripts/BarMichele/BarPathReceipt.cs`. Copy of [PathReceipt](./hud.md#pathreceipt) for the Bar-Michele receipt: `Line(card)` is only the card ("CAVALLO SPADE", no number in the path) and `Footer(moves)` ends a closed path with "TOTALE" instead of "CICLO CHIUSO" (the other ends unchanged).

- **Used by**: [BarReceiptView](#barreceiptview).

## BarEncounterEndView

`Assets/Scripts/BarMichele/BarEncounterEndView.cs`. On `BarHud/FineIncontro`. Copy of [EncounterEndView](./hud.md#encounterendview) that titles the end panel by why the encounter ended, first match wins: "Sconfitto!" ([BarRpgView](#barrpgview)`.DefeatText`), "Tutte le carte girate!" ([BarEvents](#barevents)`.AllCardsTurned`), "Quarto re!". It also checks after every `PathPlayed`, so all the cards turned are told even when the encounter would go on; it shows once per game.

- **Serialized fields**: `tableDeckManager`, `rpgView`, `group`, `title` (`Titolo`, auto-size 40–64, no wrap), `body` (`Testo`), `fadeDuration`, `fadeEase`.
- **Uses**: [TombolaCard](./hud.md#tombolacard) for the shapes.

## Open questions

- Not done from the raw notes: marking the rows and columns of the table in the UI, for events that would swap indices ([Parte RPG](../business/parte-rpg.md#domande-aperte)).
- After a defeat the remaining moves of the path are not animated: the cards on the table no longer match the [TableState](./table.md#tablestate) until Reset. Harmless while the encounter is over; to keep in mind if a defeat stops ending the encounter.
