using UnityEngine;
using UnityEngine.UI; // Se usi UI

public class CardDisplay : MonoBehaviour
{
    public CardData cardData;
    public Image displayImage; // Trascina qui l'immagine che deve mostrare la carta

    public void Setup(CardData data)
    {
        cardData = data;
        displayImage.sprite = data.immagineFronte;
    }

    public void OnMouseDown()
    {
        // Qui aggiungeremo la logica per scoprire la carta
        Debug.Log("Hai cliccato su: " + cardData.nome);
    }
}