using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

// One cell of the tombola card: a dot for a hole, ringed in gold when the hole is part of a complete shape.
public class TombolaCellView : MonoBehaviour
{
    [SerializeField, Required]
    private Image dot;

    [SerializeField, Required]
    private Image ring;

    public void Show(TombolaCell cell)
    {
        dot.enabled = cell != TombolaCell.Empty;
        ring.enabled = cell == TombolaCell.ShapeHole;
    }
}
