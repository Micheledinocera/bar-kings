using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class CardGenerator : EditorWindow
{
    [MenuItem("Tools/Genera Carte Napoletane")]
    public static void GenerateCards()
    {
        // Supponiamo che le tue immagini siano in una cartella chiamata "Sprites"
        string folderPath = "Assets/Images/Cards/Fronte";
        string[] filePaths = Directory.GetFiles(folderPath, "*.jpg");
        for (int i = 0; i < filePaths.Count(); i++)
        {
            string fileName = Path.GetFileNameWithoutExtension(filePaths[i]);
            CardData newCard = CreateInstance<CardData>();

            // Imposta dati base
            newCard.nome = fileName;
            newCard.index = i;
            newCard.seme = CardData.SEMI[i / 10];
            newCard.valore = (i % 10) + 1;
            newCard.isFlipped = false;
            newCard.immagineFronte = AssetDatabase.LoadAssetAtPath<Sprite>(filePaths[i]);

            AssetDatabase.CreateAsset(newCard, "Assets/Instances/Cards/" + newCard.valore + "_" + newCard.seme + ".asset");
        }
        AssetDatabase.SaveAssets();
        Debug.Log("40 Carte generate con successo!");
    }
}