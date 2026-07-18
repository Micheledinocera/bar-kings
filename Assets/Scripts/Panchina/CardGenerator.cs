using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class CardGeneratorPanchina : EditorWindow
{
    [MenuItem("Tools/Genera Carte Napoletane Panchina")]
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
            newCard.seme = (Seme)(i / 10);
            newCard.valore = (i % 10) + 1;
            newCard.immagineFronte = AssetDatabase.LoadAssetAtPath<Sprite>(filePaths[i]);

            AssetDatabase.CreateAsset(newCard, "Assets/Instances/Cards/" + newCard.valore + "_" + newCard.seme + ".asset");
            DeckManagerPanchina.cardDatabase.Add(newCard);
            DeckManagerPanchina.cardDatabaseInGame.Add(new(newCard, i));
        }
        AssetDatabase.SaveAssets();
    }
}