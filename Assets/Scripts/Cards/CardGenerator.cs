using UnityEngine;
using UnityEditor;
using System.IO;

public class CardGenerator : EditorWindow
{
    [MenuItem("Tools/Genera Carte Napoletane")]
    public static void GenerateCards()
    {
        // Supponiamo che le tue immagini siano in una cartella chiamata "Sprites"
        string folderPath = "Assets/Images/Cards/Fronte";
        string[] filePaths = Directory.GetFiles(folderPath, "*.jpg");
        int i = 0;
        foreach (string path in filePaths)
        {
            string fileName = Path.GetFileNameWithoutExtension(path);
            CardData newCard = ScriptableObject.CreateInstance<CardData>();

            // Imposta dati base
            newCard.nome = fileName;
            newCard.seme = CardData.SEMI[i / 10];
            newCard.valore = (i % 10) + 1;
            newCard.immagineFronte = AssetDatabase.LoadAssetAtPath<Sprite>(path);

            // Crea l'asset nella cartella "Data"
            AssetDatabase.CreateAsset(newCard, "Assets/Instances/Cards/" + newCard.valore + "_" + newCard.seme + ".asset");
            i++;
        }
        AssetDatabase.SaveAssets();
        Debug.Log("40 Carte generate con successo!");
    }
}