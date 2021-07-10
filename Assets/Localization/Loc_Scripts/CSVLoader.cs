using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using System;

// www.youtube.com/watch?v=c-dzg4M20wY
public class CSVLoader
{
    private TextAsset csvFile;
    private char lineSeperator = '\n';
    private char surround = '"';
    private string[] fieldSeparator = { "\",\"" };

    public void LoadCSV()
    {
        csvFile = Resources.Load<TextAsset>("Localization");
    }
   

    public bool CheckAvailableLanguage(string attributeID)
    {
      //  Debug.Log("inside check available language... given is ..." + attributeID);
        Dictionary<string, string> dictionary = new Dictionary<string, string>();

        string[] lines = csvFile.text.Split(lineSeperator);


        string[] headers = lines[0].Split(fieldSeparator, System.StringSplitOptions.None);
       

        for (int i = 0; i < headers.Length; i++)
        {
           // Debug.Log("Headers are ...." + headers[i]);
            if (headers[i].ToLower().Contains(attributeID.ToLower()))
            {


                return true;
            }
        }


        return false;
    }

    public Dictionary<string, string> GetDictionaryValues(string attributeID)
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();

        string[] lines = csvFile.text.Split(lineSeperator);

        int attributeIndex = -1;

        string[] headers = lines[0].Split(fieldSeparator, System.StringSplitOptions.None);

        for (int i = 0; i < headers.Length; i++)
        {
            if (headers[i].Contains(attributeID))
            {
                attributeIndex = i;
                break;
            }
        }

        Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];

            string[] fields = CSVParser.Split(line);

            for (int f = 0; f < fields.Length; f++)
            {
                fields[f] = fields[f].TrimStart(' ', surround);
                fields[f] = fields[f].TrimEnd('\r',surround);
                //Debug.Log(fields[f]);
            }

            if (fields.Length > attributeIndex)
            {
                var key = fields[0];

                if (dictionary.ContainsKey(key)) { continue; }

                var value = fields[attributeIndex];

                dictionary.Add(key, value);
            }
        }

        return dictionary;
    }
}