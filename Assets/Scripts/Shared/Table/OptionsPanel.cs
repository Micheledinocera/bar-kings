using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Options of the table scenes, shown and hidden by the Options button (Toggle): the deck preset of the next game
// (used from the next Reset) and the speed of the animations (used from the next animation). On the root of the
// Options prefab, which stays active while the panel is hidden; the option buttons are built the first time the
// panel opens.
public class OptionsPanel : MonoBehaviour
{
    [SerializeField, Required]
    private TableDeckManager tableDeckManager;

    [SerializeField, Required]
    private GameObject panel;

    [SerializeField, Required]
    private Button optionButtonPrefab; // Assets/Prefab/OptionButton.prefab

    [SerializeField, Required]
    private Transform presetButtons;

    [SerializeField, Required]
    private Transform speedButtons;

    [SerializeField, Required]
    private TMP_Text presetStatus;

    [SerializeField, Required]
    private DeckPresetLibrary presetLibrary; // Assets/Instances/DeckPresetLibrary.asset

    readonly Dictionary<DeckPreset, Button> buttonsByPreset = new();
    readonly Dictionary<AnimationSpeed, Button> buttonsBySpeed = new();

    public void Toggle()
    {
        if (buttonsByPreset.Count == 0)
        {
            Build();
        }
        panel.SetActive(!panel.activeSelf);
        Refresh();
    }

    void Build()
    {
        foreach (DeckPreset preset in presetLibrary.Presets)
        {
            buttonsByPreset[preset] = AddButton(
                presetButtons,
                $"{preset.name} ({preset.NonKingCards} + {DeckRules.KingCount} re)",
                () => tableDeckManager.SetNextPreset(preset)
            );
        }
        foreach (AnimationSpeed speed in Enum.GetValues(typeof(AnimationSpeed)))
        {
            buttonsBySpeed[speed] = AddButton(
                speedButtons,
                SpeedLabel(speed),
                () => tableDeckManager.SetSpeed(speed)
            );
        }
    }

    Button AddButton(Transform parent, string label, UnityAction choose)
    {
        Button button = Instantiate(optionButtonPrefab, parent);
        button.GetComponentInChildren<TMP_Text>().text = label;
        button.onClick.AddListener(choose);
        button.onClick.AddListener(Refresh);
        return button;
    }

    // The chosen options are shown as not interactable buttons.
    void Refresh()
    {
        foreach ((DeckPreset preset, Button button) in buttonsByPreset)
        {
            button.interactable = preset != tableDeckManager.NextPreset;
        }
        foreach ((AnimationSpeed speed, Button button) in buttonsBySpeed)
        {
            button.interactable = speed != tableDeckManager.Speed;
        }
        DeckPreset current = tableDeckManager.CurrentPreset;
        presetStatus.text =
            $"In gioco: {(current != null ? current.name : "-")} | prossima partita (Reset): "
            + tableDeckManager.NextPreset.name;
    }

    static string SpeedLabel(AnimationSpeed speed) =>
        speed switch
        {
            AnimationSpeed.Slow => "Piano",
            AnimationSpeed.Fast => "Veloce",
            _ => "Normale",
        };
}
