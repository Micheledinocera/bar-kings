using UnityEditor;
using UnityEngine;

// Creates the deck presets to try in Assets/Instances/DeckPresets and the library that lists them. Presets that
// already exist are kept as they are; the library keeps the presets it has (the custom ones of Tools/Preset Builder
// included) and gets the missing ones.
public static class DeckPresetGenerator
{
    const string InstancesFolder = "Assets/Instances";
    const string PresetsFolderName = "DeckPresets";
    public const string PresetsFolder = InstancesFolder + "/" + PresetsFolderName;
    const string LibraryPath = InstancesFolder + "/DeckPresetLibrary.asset";

    // Name, non-king cards removed, duplicates added: 32, 45, 60 and 71 non-king cards, plus the 4 kings. At most
    // DeckRules.MaxDuplicates, so that index>40 can deal every preset.
    static readonly (string name, int removed, int duplicates)[] Presets =
    {
        ("Preset32", 9, 5),
        ("Preset45", 5, 14),
        ("Preset60", 5, 29),
        ("Preset71", 5, 40),
    };

    [MenuItem("Tools/Genera Preset Mazzo")]
    public static void GeneratePresets()
    {
        EnsurePresetsFolder();
        DeckPresetLibrary library = LoadOrCreateLibrary();

        int created = 0;
        foreach ((string name, int removed, int duplicates) in Presets)
        {
            string assetPath = $"{PresetsFolder}/{name}.asset";
            DeckPreset preset = AssetDatabase.LoadAssetAtPath<DeckPreset>(assetPath);
            if (preset == null)
            {
                preset = ScriptableObject.CreateInstance<DeckPreset>();
                preset.Set(removed, duplicates);
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
            $"{Presets.Length} deck presets in {PresetsFolder}, {created} new; {library.Presets.Count} in {LibraryPath}."
        );
    }

    public static void EnsurePresetsFolder()
    {
        if (!AssetDatabase.IsValidFolder(PresetsFolder))
        {
            AssetDatabase.CreateFolder(InstancesFolder, PresetsFolderName);
        }
    }

    public static DeckPresetLibrary LoadOrCreateLibrary()
    {
        DeckPresetLibrary library = AssetDatabase.LoadAssetAtPath<DeckPresetLibrary>(LibraryPath);
        if (library == null)
        {
            library = ScriptableObject.CreateInstance<DeckPresetLibrary>();
            AssetDatabase.CreateAsset(library, LibraryPath);
        }
        return library;
    }
}
