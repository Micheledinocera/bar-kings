using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

// The deck presets to try, shared by the options panel and the simulation of the table scenes. Asset in
// Assets/Instances/DeckPresetLibrary.asset, created by the menu Tools/Genera Preset Mazzo; Tools/Preset Builder adds
// the custom presets to it.
[CreateAssetMenu(fileName = "DeckPresetLibrary", menuName = "Carte/Deck Preset Library")]
public class DeckPresetLibrary : ScriptableObject
{
    [SerializeField, Required]
    private List<DeckPreset> presets = new();

    // Without the entries of presets deleted from the project (they stay in the list as empty references).
    public IReadOnlyList<DeckPreset> Presets => presets.Where(preset => preset != null).ToList();

#if UNITY_EDITOR
    // Adds the preset once and drops the entries of deleted presets; true when the list changed.
    public bool Add(DeckPreset preset)
    {
        bool changed = presets.RemoveAll(entry => entry == null) > 0;
        if (preset != null && !presets.Contains(preset))
        {
            presets.Add(preset);
            changed = true;
        }
        return changed;
    }
#endif
}
