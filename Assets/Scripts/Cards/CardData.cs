using UnityEngine;

[CreateAssetMenu(fileName = "NuovaCarta", menuName = "Carte/Carta Napoletana")]
public class CardData : ScriptableObject
{
    public string nome;
    public int valore;
    public int index;
    public bool isFlipped;
    public string seme; // "Denari", "Coppe", "Spade", "Bastoni"
    public Sprite immagineFronte;

    public static string[] SEMI={"Denari", "Coppe", "Spade", "Bastoni"};
}