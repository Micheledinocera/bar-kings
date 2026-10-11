using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

// Creates the CardData assets from the card images and the CardDatabase that lists them in table order.
// Run it from the menu when the images change; cards that already exist are kept as they are.
public static class CardGenerator
{
    const string ImagesFolder = "Assets/Images/Cards/Fronte";
    const string CardsFolder = "Assets/Instances/Cards";
    const string DatabasePath = "Assets/Instances/CardDatabase.asset";
    const int CardsPerSuit = 10;

    [MenuItem("Tools/Genera Carte Napoletane")]
    public static void GenerateCards()
    {
        // Images are named 01_… to 40_…: sorted by name they go suit by suit (Seme order), ace to king.
        string[] imagePaths = Directory
            .GetFiles(ImagesFolder, "*.jpg")
            .Select(path => path.Replace('\\', '/'))
            .OrderBy(path => Path.GetFileName(path), StringComparer.Ordinal)
            .ToArray();

        List<CardData> cards = new(imagePaths.Length);
        int created = 0;
        for (int i = 0; i < imagePaths.Length; i++)
        {
            Seme seme = (Seme)(i / CardsPerSuit);
            int valore = (i % CardsPerSuit) + 1;
            string assetPath = $"{CardsFolder}/{valore}_{seme}.asset";

            CardData card = AssetDatabase.LoadAssetAtPath<CardData>(assetPath);
            if (card == null)
            {
                card = ScriptableObject.CreateInstance<CardData>();
                card.nome = Path.GetFileNameWithoutExtension(imagePaths[i]);
                card.seme = seme;
                card.valore = valore;
                card.immagineFronte = AssetDatabase.LoadAssetAtPath<Sprite>(imagePaths[i]);
                AssetDatabase.CreateAsset(card, assetPath);
                created++;
            }
            cards.Add(card);
        }

        CardDatabase database = AssetDatabase.LoadAssetAtPath<CardDatabase>(DatabasePath);
        if (database == null)
        {
            database = ScriptableObject.CreateInstance<CardDatabase>();
            AssetDatabase.CreateAsset(database, DatabasePath);
        }
        if (!database.Cards.SequenceEqual(cards))
        {
            database.SetCards(cards);
            EditorUtility.SetDirty(database);
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"{cards.Count} cards in {DatabasePath}, {created} new.");
    }
}
