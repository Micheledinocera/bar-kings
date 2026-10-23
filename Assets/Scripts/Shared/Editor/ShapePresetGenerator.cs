using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Menu Tools/Genera Preset Forme: creates the deck presets that measure the shapes of holes in the tribuna (wiki
// business/modifica-del-mazzo.md, decision in analysis) and adds them to the preset library. Each shape comes "secca"
// (only its cards removed, under 40 cards) and "piena" (as many random duplicates as cards removed, 40 cards); the
// Copie presets put n copies of the ace of Denari in a 40-card deck. Presets that already exist are kept as they are.
public static class ShapePresetGenerator
{
    const int CopiesCard = 0; // slot of the ace of Denari

    static readonly int[] CopiesCounts = { 2, 4, 8 };

    [MenuItem("Tools/Genera Preset Forme")]
    public static void GeneratePresets()
    {
        IReadOnlyList<CardData> cards = AssetDatabase
            .LoadAssetAtPath<CardDatabase>(CardGenerator.DatabasePath)
            .Cards;
        DeckPresetGenerator.EnsurePresetsFolder();
        DeckPresetLibrary library = DeckPresetGenerator.LoadOrCreateLibrary();

        List<(string name, int[] removed, int randomRemoved, int copies, int duplicates)> specs =
            new();
        foreach (int count in CopiesCounts)
        {
            specs.Add(($"Copie{count}", new int[0], count - 1, count - 1, 0));
        }
        foreach ((string name, int[] slots) in Shapes())
        {
            specs.Add(($"{name}Secca", slots, 0, 0, 0));
            specs.Add(($"{name}Piena", slots, 0, 0, slots.Length));
        }
        specs.Add(("SemeSecca", Cells(0, 1, 0, DeckRules.SlotsPerRow - 1), 0, 0, 0));
        specs.Add(
            (
                "PariSecca",
                Enumerable
                    .Range(0, DeckRules.TableSlots)
                    .Where(slot =>
                        TableShapes.ValueOf(slot) % 2 == 0
                        && TableShapes.ValueOf(slot) != DeckRules.KingValue
                    )
                    .ToArray(),
                0,
                0,
                0
            )
        );

        int created = 0;
        foreach (
            (string name, int[] removed, int randomRemoved, int copies, int duplicates) in specs
        )
        {
            string assetPath = $"{DeckPresetGenerator.PresetsFolder}/{name}.asset";
            DeckPreset preset = AssetDatabase.LoadAssetAtPath<DeckPreset>(assetPath);
            if (preset == null)
            {
                preset = ScriptableObject.CreateInstance<DeckPreset>();
                preset.Set(randomRemoved, duplicates);
                preset.SetChosen(
                    removed.Select(slot => cards[slot]),
                    copies > 0
                        ? new[] { new ChosenCopies(cards[CopiesCard], copies) }
                        : new ChosenCopies[0]
                );
                AssetDatabase.CreateAsset(preset, assetPath);
                created++;
            }
            if (library.Add(preset))
            {
                EditorUtility.SetDirty(library);
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log(
            $"{specs.Count} shape presets in {DeckPresetGenerator.PresetsFolder}, {created} new; "
                + $"{library.Presets.Count} presets in the library."
        );
    }

    // The shapes drawn on the grid from the ace of Denari: column of the aces, diagonal (ace of Denari, 2 of Coppe,
    // 3 of Spade, 4 of Bastoni), squares 2x2, 3x3 and 4x4.
    static IEnumerable<(string name, int[] slots)> Shapes()
    {
        yield return ("Colonna", Cells(0, DeckRules.BaseRows, 0, 1));
        yield return (
            "Diagonale",
            Enumerable
                .Range(0, DeckRules.BaseRows)
                .Select(step => TableShapes.Slot(step, step))
                .ToArray()
        );
        yield return ("Quadrato2", Cells(0, 2, 0, 2));
        yield return ("Quadrato3", Cells(0, 3, 0, 3));
        yield return ("Quadrato4", Cells(0, 4, 0, 4));
    }

    // Slots of the rows and columns given, from the first one included to the last one excluded.
    static int[] Cells(int fromRow, int toRow, int fromColumn, int toColumn) =>
        Enumerable
            .Range(fromRow, toRow - fromRow)
            .SelectMany(row =>
                Enumerable
                    .Range(fromColumn, toColumn - fromColumn)
                    .Select(column => TableShapes.Slot(row, column))
            )
            .ToArray();
}
