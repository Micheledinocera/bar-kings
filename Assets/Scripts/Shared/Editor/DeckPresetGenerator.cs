using UnityEditor;
using UnityEngine;

// Creates the deck presets to try in Assets/Instances/DeckPresets. Presets that already exist are kept as they are.
public static class DeckPresetGenerator
{
    const string InstancesFolder = "Assets/Instances";
    const string PresetsFolderName = "DeckPresets";
    const string PresetsFolder = InstancesFolder + "/" + PresetsFolderName;

    // Name, non-king cards removed, duplicates added: 32, 45, 60 and 80 non-king cards, plus the 4 kings.
    static readonly (string name, int removed, int duplicates)[] Presets =
    {
        ("Preset32", 9, 5),
        ("Preset45", 5, 14),
        ("Preset60", 5, 29),
        ("Preset80", 5, 49),
    };

    [MenuItem("Tools/Genera Preset Mazzo")]
    public static void GeneratePresets()
    {
        if (!AssetDatabase.IsValidFolder(PresetsFolder))
        {
            AssetDatabase.CreateFolder(InstancesFolder, PresetsFolderName);
        }

        int created = 0;
        foreach ((string name, int removed, int duplicates) in Presets)
        {
            string assetPath = $"{PresetsFolder}/{name}.asset";
            if (AssetDatabase.LoadAssetAtPath<DeckPreset>(assetPath) != null)
            {
                continue;
            }
            DeckPreset preset = ScriptableObject.CreateInstance<DeckPreset>();
            preset.Set(removed, duplicates);
            AssetDatabase.CreateAsset(preset, assetPath);
            created++;
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"{Presets.Length} deck presets in {PresetsFolder}, {created} new.");
    }
}
