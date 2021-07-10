using System.Collections.Generic;
using UnityEngine;

public class LocalizationSystem : MonoBehaviour
{
    public enum Language
    {
        English,
        French,
        Japanese
    }

    public static Language language = Language.French;

    public static Dictionary<string, string> localisedEN;
    public static Dictionary<string, string> localisedFR;

    public static bool isInit;

    public static void Init()
    {
        CSVLoader csvLoader = new CSVLoader();
        csvLoader.LoadCSV();

        localisedEN = csvLoader.GetDictionaryValues("en");
        localisedFR = csvLoader.GetDictionaryValues("fr");

        isInit = true;
    }

    public static string GetLocalisedValue(string key)
    {
        if (!isInit) { Init(); }

        string value = key;

        switch (language)
        {
            case Language.English:
                localisedEN.TryGetValue(key, out value);
                break;
            case Language.French:
                localisedFR.TryGetValue(key, out value);
                break;
        }

        return value;
    }
}