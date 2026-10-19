using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

// Menu Tools/Preset Builder: creates a custom deck preset in Assets/Instances/DeckPresets and adds it to the preset
// library, so the options panel and the simulation of the table scenes use it next to the others. An existing preset
// is never overwritten.
public class DeckPresetBuilderWindow : OdinEditorWindow
{
    [SerializeField, MinValue(0), MaxValue(DeckRules.BaseNonKingCards)]
    private int removedCards;

    [SerializeField, MinValue(0), MaxValue(DeckRules.MaxDuplicates)]
    private int duplicates;

    // Random: drawn at every game; First: the first cards in table order, the same at every game.
    [SerializeField, EnumToggleButtons]
    private CardPick pick;

    // Empty: named after the deck, e.g. Custom45 or Custom45First, with a number when that name is taken.
    [SerializeField]
    private string presetName;

    [SerializeField, HideInInspector]
    private string lastCreated;

    [ShowInInspector, ReadOnly]
    private int NonKingCards => DeckRules.BaseNonKingCards - removedCards + duplicates;

    [ShowInInspector, ReadOnly]
    private int DeckSize => NonKingCards + DeckRules.KingCount;

    [ShowInInspector, ReadOnly]
    private string AssetPath =>
        HasCustomName
            ? $"{DeckPresetGenerator.PresetsFolder}/{presetName.Trim()}.asset"
            : AssetDatabase.GenerateUniqueAssetPath(
                $"{DeckPresetGenerator.PresetsFolder}/Custom{NonKingCards}{(pick == CardPick.First ? "First" : "")}.asset"
            );

    bool HasCustomName => !string.IsNullOrWhiteSpace(presetName);

    [MenuItem("Tools/Preset Builder")]
    static void Open() => GetWindow<DeckPresetBuilderWindow>("Preset Builder").Show();

    [Button(ButtonSizes.Large), DisableIf(nameof(HasError))]
    [InfoBox("$" + nameof(Error), InfoMessageType.Error, nameof(HasError))]
    [InfoBox("$" + nameof(CreatedMessage), InfoMessageType.Info, nameof(ShowCreated))]
    private void CreatePreset()
    {
        DeckPresetGenerator.EnsurePresetsFolder();
        string assetPath = AssetPath;
        DeckPreset preset = ScriptableObject.CreateInstance<DeckPreset>();
        preset.Set(removedCards, duplicates, pick);
        AssetDatabase.CreateAsset(preset, assetPath);

        DeckPresetLibrary library = DeckPresetGenerator.LoadOrCreateLibrary();
        library.Add(preset);
        EditorUtility.SetDirty(library);
        AssetDatabase.SaveAssets();
        EditorGUIUtility.PingObject(preset);
        lastCreated = assetPath;
        presetName = "";
        Debug.Log(
            $"Deck preset {assetPath}: {removedCards} removed, {duplicates} duplicates ({pick}), {DeckSize} cards; "
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
            if (removedCards < 0 || removedCards > DeckRules.BaseNonKingCards)
            {
                return $"Removed cards: from 0 to {DeckRules.BaseNonKingCards}.";
            }
            if (duplicates < 0 || duplicates > DeckRules.MaxDuplicates)
            {
                return $"Duplicates: from 0 to {DeckRules.MaxDuplicates}, so that index>40 can deal the deck.";
            }
            if (duplicates > 0 && removedCards == DeckRules.BaseNonKingCards)
            {
                return "No card left to duplicate.";
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
