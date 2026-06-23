using UnityEngine;
using UnityEngine.UI; // Se usi UI

public class CardDisplay : MonoBehaviour
{
    public CardData cardData;
    public Image displayImage; // Trascina qui l'immagine che deve mostrare la carta
    public Sprite spriteRetro;

    public void Setup(CardData data)
    {
        cardData = data;
        displayImage.sprite = spriteRetro;
    }

    public void OnMouseDown()
    {
        // Qui aggiungeremo la logica per scoprire la carta
        displayImage.sprite = cardData.immagineFronte;
    }

    public void Flip()
    {
        // Esempio: cambi l'immagine da "retro" a "fronte"
        displayImage.sprite = cardData.immagineFronte;
        Debug.Log(cardData.nome + " è stata scoperta!");
    }

}