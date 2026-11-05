using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Inventory panel of the TribunaRpg scene: the names of the santini owned and one button per amaro. Clicking a
// button asks to drink that amaro (DrinkRequested, with its index in the list); RpgStatsView decides whether it can
// be drunk now and redraws the panel. Pure view: it keeps no game state.
public class InventoryView : MonoBehaviour
{
    [SerializeField, Required]
    private TMP_Text santiniText;

    [SerializeField, Required]
    private Transform amariContainer;

    [SerializeField, Required, AssetsOnly]
    private Button amaroButtonPrefab;

    private readonly List<Button> amaroButtons = new();

    public event Action<int> DrinkRequested;

    // Redraws the panel. canDrink off: the amaro buttons are shown but not clickable (path being resolved,
    // encounter over).
    public void Show(IReadOnlyList<Santino> santini, IReadOnlyList<Amaro> amari, bool canDrink)
    {
        List<string> names = santini.Where(s => s != null).Select(s => s.Name).ToList();
        santiniText.text =
            names.Count == 0 ? "Santini: nessuno" : "Santini:\n" + string.Join("\n", names);

        foreach (Button button in amaroButtons)
        {
            Destroy(button.gameObject);
        }
        amaroButtons.Clear();
        for (int i = 0; i < amari.Count; i++)
        {
            if (amari[i] == null)
            {
                continue;
            }
            int index = i;
            Button button = Instantiate(amaroButtonPrefab, amariContainer);
            button.name = $"Amaro {amari[i].Name}";
            button.GetComponentInChildren<TMP_Text>().text = $"Bevi {amari[i].Name}";
            button.interactable = canDrink;
            button.onClick.AddListener(() => DrinkRequested?.Invoke(index));
            amaroButtons.Add(button);
        }
    }
}
