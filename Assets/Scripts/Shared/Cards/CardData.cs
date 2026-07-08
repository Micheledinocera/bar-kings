using UnityEngine;

public enum Seme { Denari, Coppe, Spade, Bastoni }
[CreateAssetMenu(fileName = "NuovaCarta", menuName = "Carte/Carta Napoletana")]
public class CardData : ScriptableObject
{
    public string nome;
    public int valore;
    public Seme seme;
    public Sprite immagineFronte;
}