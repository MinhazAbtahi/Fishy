using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;
using System.Xml.Linq;
using System.IO;

public class LocalizationManager
{
    // Start is called before the first frame update
    public string localeLanguage;
    SystemLanguage currentLanguageCode;
    public XDocument xmd;

    public static Dictionary<string, string> valueMap;
    CSVLoader csvLoader;


    static LocalizationManager sharedInstance;


    public static LocalizationManager getInstance()
    {
        if (sharedInstance == null)
        {
            sharedInstance = LocalizationManager.create();
        }

        return sharedInstance;
    }

    static LocalizationManager create()
    {
        LocalizationManager ret = new LocalizationManager();
        if (ret != null && ret.init())
        {
            return ret;
        }
        else
        {
            //ret.Dispose();
            ret = null;
            return null;
        }
    }
    public bool init()
    {
        Initialized();
        return true;
    }

    public void Initialized()
    {
        valueMap = new Dictionary<string, string>();
        csvLoader = new CSVLoader();
        csvLoader.LoadCSV();

        localeLanguage = "english"; //default
        //        LanguageType currentLanguageType = Application::getInstance()->getCurrentLanguage();
        //        CCLOG("currentLanguageType: %d", currentLanguageType);
        currentLanguageCode = Application.systemLanguage;
        Debug.Log("helloooo i should be " + currentLanguageCode.ToString());
        /*if (currentLanguageCode == SystemLanguage.Japanese) localeLanguage = "japanese";
        else if (currentLanguageCode == SystemLanguage.Korean) localeLanguage = "korean";*/

        if (csvLoader.CheckAvailableLanguage(currentLanguageCode.ToString()))
        {
            localeLanguage = currentLanguageCode.ToString().ToLower();
            Debug.Log("my locale language agter check available language " + localeLanguage);
        }
        else
        {
            Debug.Log("inside else, maybe I could not have found the language");
            localeLanguage = "english";

        }
        // localeLanguage = "korean";
        //localeLanguage = "japanese";
        PopulateValueInCSV();
        //PopulateValue();

    }
    public string getLocalizedString(string key)
    {
        string value = key;
        bool success = valueMap.TryGetValue(key, out value);
        if (!success) value = key;
        /*if (valueMap.ContainsKey(key))
        {
            value = valueMap[key];
            Debug.Log("value contain");
        }*/

        if (currentLanguageCode == SystemLanguage.Arabic && success)
        {
            value = GetRTLString(value);
        }

        return value;
    }

    public string GetRTLString(string str)
    {
        string rtl = "";
        for (int i = str.Length - 1; i >= 0; i--)
        {
            rtl += str[i];
        }
        return rtl;
    }

    public string getLocalizedStringForNumber(string key)
    {
        string value = key;

        /*if(valueMap.ContainsKey(key))
        {
            value = valueMap[key];
            Debug.Log("value contain");
        }*/

        return value;
    }
    public void CallAfterLoad()
    {

    }

    public void PopulateValueInCSV()
    {
        valueMap = csvLoader.GetDictionaryValues(localeLanguage);
        //Debug.Log("csv value loaded");
    }

    public void PopulateValue()
    {
        bool isLanguageFound = false;

        string path = Path.Combine(Application.streamingAssetsPath, "localization.plist");



        xmd = XDocument.Load(path);
        XElement plist = xmd.Element("plist");
        XElement dict = plist.Element("dict");

        var dictElements = dict.Elements();
        //Parse(this, dictElements);
        for (int i = 0; i < dictElements.Count(); i += 2)
        {
            XElement key = dictElements.ElementAt(i);
            XElement val = dictElements.ElementAt(i + 1);

            if (key.Value == localeLanguage)
            {
                //var childkey = key.Elements();
                //XElement kw = key.Element("dict");
                isLanguageFound = true;
                var childkey = val.Elements();

                //Debug.Log("bengali has --> "+ childkey.Count());
                for (int j = 0; j < childkey.Count(); j += 2)
                {
                    valueMap.Add(childkey.ElementAt(j).Value, childkey.ElementAt(j + 1).Value);
                    //Debug.Log(valueMap[childkey.ElementAt(j).Value]);
                }
                break;
            }

        }

        if (!isLanguageFound)
        {
            setEnglishAsLocalLanguage();
        }

    }

    public void setEnglishAsLocalLanguage()
    {
        XElement plist = xmd.Element("plist");
        XElement dict = plist.Element("dict");

        var dictElements = dict.Elements();
        //Parse(this, dictElements);
        for (int i = 0; i < dictElements.Count(); i += 2)
        {
            XElement key = dictElements.ElementAt(i);
            XElement val = dictElements.ElementAt(i + 1);

            if (key.Value == "english")
            {

                var childkey = val.Elements();

                for (int j = 0; j < childkey.Count(); j += 2)
                {
                    valueMap.Add(childkey.ElementAt(j).Value, childkey.ElementAt(j + 1).Value);
                    /*XElement lang = childkey.ElementAt(j);
                    if(lang.Value == "Buy the %@ Habitat")
                    {
                        XElement val2 = childkey.ElementAt(i + 1);
                        Debug.Log("debug log --> " + val2.Value);
                        break;
                    }*/

                }
                break;
            }

        }
    }

    public string getParseAbleString(string value)
    {
        string[] arr = value.Replace("%@", "{}").Split('{');
        string parseSt = "";
        int i = 0;
        foreach (string perLine in arr)
        {
            if (perLine.Length > 0)
            {
                string tempSt = "";
                if (perLine[0] == '}')
                {
                    tempSt += "{" + i.ToString() + perLine;
                }
                else
                {
                    tempSt = perLine;
                }
                parseSt += tempSt;
            }


        }
        return parseSt;
    }

}
