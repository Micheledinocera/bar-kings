using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

// Menu Tools/Preset Builder: creates a custom deck preset in Assets/Instances/DeckPresets and adds it to the preset
// library, so the options panel and the simulation of the table scenes use it next to the others. The cards are
// chosen on a grid like the table (rows = suits, columns = values, kings left out): -1 removes the card, n adds n
// copies. An existing preset is never overwritten.
public class DeckPresetBuilderWindow : OdinEditorWindow
{
    const int RemovedCell = -1;
    const int ValueColumns = DeckRules.SlotsPerRow - 1; // the kings are never removed nor duplicated
    const float CellWidth = 32;
    const float RowLabelWidth = 64;

    // One cell per non-king card, row by row: RemovedCell, 0 (kept as it is) or the copies added.
    [SerializeField, HideInInspector]
    private int[] cells = new int[DeckRules.BaseRows * ValueColumns];

    // Empty: named after the deck, e.g. Custom40, with a number when that name is taken.
    [SerializeField, PropertyOrder(1)]
    private string presetName;

    [SerializeField, HideInInspector]
    private string lastCreated;

    int ChosenRemoved => cells.Count(cell => cell == RemovedCell);

    int ChosenCopies => cells.Where(cell => cell > 0).Sum();

    [ShowInInspector, ReadOnly, PropertyOrder(2)]
    private int NonKingCards => DeckRules.BaseNonKingCards - ChosenRemoved + ChosenCopies;

    [ShowInInspector, ReadOnly, PropertyOrder(2)]
    private int DeckSize => NonKingCards + DeckRules.KingCount;

    [ShowInInspector, ReadOnly, PropertyOrder(2)]
    private string AssetPath =>
        HasCustomName
            ? $"{DeckPresetGenerator.PresetsFolder}/{presetName.Trim()}.asset"
            : AssetDatabase.GenerateUniqueAssetPath(
                $"{DeckPresetGenerator.PresetsFolder}/Custom{DeckSize}.asset"
            );

    bool HasCustomName => !string.IsNullOrWhiteSpace(presetName);

    [MenuItem("Tools/Preset Builder")]
    static void Open() => GetWindow<DeckPresetBuilderWindow>("Preset Builder").Show();

    [OnInspectorGUI, PropertyOrder(0)]
    private void DrawCardGrid()
    {
        if (cells == null || cells.Length != DeckRules.BaseRows * ValueColumns)
        {
            cells = new int[DeckRules.BaseRows * ValueColumns];
        }
        EditorGUILayout.LabelField(
            "Chosen cards: -1 removed, n = copies added",
            EditorStyles.boldLabel
        );
        using (new EditorGUILayout.HorizontalScope())
        {
            GUILayout.Space(RowLabelWidth);
            for (int column = 0; column < ValueColumns; column++)
            {
                GUILayout.Label((column + 1).ToString(), GUILayout.Width(CellWidth));
            }
        }
        Color background = GUI.backgroundColor;
        for (int row = 0; row < DeckRules.BaseRows; row++)
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(((Seme)row).ToString(), GUILayout.Width(RowLabelWidth));
                for (int column = 0; column < ValueColumns; column++)
                {
                    int cell = row * ValueColumns + column;
                    GUI.backgroundColor =
                        cells[cell] == RemovedCell ? Color.red
                        : cells[cell] > 0 ? Color.green
                        : background;
                    cells[cell] = Mathf.Clamp(
                        EditorGUILayout.IntField(cells[cell], GUILayout.Width(CellWidth)),
                        RemovedCell,
                        DeckRules.MaxDuplicates
                    );
                }
                GUI.backgroundColor = background;
            }
        }
        if (GUILayout.Button("Clear grid"))
        {
            cells = new int[DeckRules.BaseRows * ValueColumns];
        }
    }

    [Button(ButtonSizes.Large), DisableIf(nameof(HasError)), PropertyOrder(3)]
    [InfoBox("$" + nameof(Error), InfoMessageType.Error, nameof(HasError))]
    [InfoBox("$" + nameof(CreatedMessage), InfoMessageType.Info, nameof(ShowCreated))]
    private void CreatePreset()
    {
        IReadOnlyList<CardData> cards = AssetDatabase
            .LoadAssetAtPath<CardDatabase>(CardGenerator.DatabasePath)
            .Cards;
        List<CardData> removed = new();
        List<ChosenCopies> copies = new();
        for (int row = 0; row < DeckRules.BaseRows; row++)
        {
            for (int column = 0; column < ValueColumns; column++)
            {
                int cell = cells[row * ValueColumns + column];
                CardData card = cards[row * DeckRules.SlotsPerRow + column];
                if (cell == RemovedCell)
                {
                    removed.Add(card);
                }
                else if (cell > 0)
                {
                    copies.Add(new ChosenCopies(card, cell));
                }
            }
        }

        DeckPresetGenerator.EnsurePresetsFolder();
        string assetPath = AssetPath;
        DeckPreset preset = ScriptableObject.CreateInstance<DeckPreset>();
        preset.SetChosen(removed, copies);
        AssetDatabase.CreateAsset(preset, assetPath);

        DeckPresetLibrary library = DeckPresetGenerator.LoadOrCreateLibrary();
        library.Add(preset);
        EditorUtility.SetDirty(library);
        AssetDatabase.SaveAssets();
        EditorGUIUtility.PingObject(preset);
        lastCreated = assetPath;
        presetName = "";
        Debug.Log(
            $"Deck preset {assetPath}: {removed.Count} removed, {ChosenCopies} copies added, {DeckSize} cards; "
                + $"{library.Presets.Count} presets in the library."
        );
    }

    bool HasError => Error != null;

    bool ShowCreated => !HasError && !string.IsNullOrEmpty(lastCreated);

    string CreatedMessage => $"Created {lastCreated}, added to the preset library.";

    string Error
    {
        get
        {
            if (ChosenCopies > DeckRules.MaxDuplicates)
            {
                return $"Copies added: at most {DeckRules.MaxDuplicates}, so that index>40 can deal the deck.";
            }
            if (!HasCustomName)
            {
                return null;
            }
            if (presetName.Trim().IndexOfAny(System.IO.Path.GetInvalidFileNameChars()) >= 0)
            {
                return "The name is not a valid file name.";
            }
            if (AssetDatabase.LoadAssetAtPath<Object>(AssetPath) != null)
            {
                return $"{AssetPath} already exists: choose another name.";
            }
            return null;
        }
    }
}
