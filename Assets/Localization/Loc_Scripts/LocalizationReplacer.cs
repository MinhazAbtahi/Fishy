using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//public enum LanguageList
//{
//    English,
//    Japanese,
//    Korean,
//    ChineseSimplified,
//    ChineseTraditional


//}
public class LocalizationReplacer : MonoBehaviour
{
    public Image MyImage;
    public TextMeshProUGUI Mytxt;
    public TextMeshPro Mytxt2;
    public string KEY;
     LocalizationImageSet CurrentImageData;
    [Header("Regular Image")]
    public LocalizationImageSet[] localizationDatas;
    [Space]
    [Header("Alternate Image")]

    public LocalizationImageSet[] alternateLocalizationDatas;

    // Start is called before the first frame update
    void Start()
    {
        MyImage = GetComponent<Image>();
        Mytxt = GetComponent<TextMeshProUGUI>();
        Mytxt2 = GetComponent<TextMeshPro>();
        //call this the first time ever
        //LocalizationManager.getInstance();
        // SetData();
        if (Mytxt != null || Mytxt2 != null)
        {
            SetData(KEY);
        }
        else
        {
            if (MyImage != null && CurrentImageData==null)
            {
                SetNewData();
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnValidate()
    {
        CurrentImageData = null;

    }
    void SetData(string KEY)
    {
        if (Mytxt != null)
        {

            Mytxt.text = LocalizationManager.getInstance().getLocalizedString(KEY);
            Debug.Log(LocalizationManager.getInstance().getLocalizedString(KEY));
        }
        if (Mytxt2 != null)
        {
            Mytxt2.text = LocalizationManager.getInstance().getLocalizedString(KEY);
        }

    }
     
   public void SetNewData(bool alternate = false)
    {
        Debug.Log(this.name + " inside setnewdata");
        if (MyImage == null)
        {
            MyImage = GetComponent<Image>();
        }
        //var size = LocalizationManager.getInstance().;
        //localizationDatas = new LocalizationImageSet [size];
        var dataSet = (alternate ==true) ? alternateLocalizationDatas :localizationDatas ;
        foreach (var data in dataSet)
        {
            if(data.myLanguage != Application.systemLanguage)
            {
            }
            else
            {
                CurrentImageData = data;
                Debug.Log(this.name + " inside currentimagedata " + CurrentImageData.myLanguage);

                break;

            }
        }
        if (CurrentImageData!=null)
        {
            if (CurrentImageData.Image != null)
            {
                MyImage.sprite = CurrentImageData.Image;
            }
        }

    }

    public void ReloadSprite()
    {
        if (CurrentImageData != null)
        {
            MyImage.sprite = CurrentImageData.Image;
            Debug.Log("swipe to move " + MyImage.name);
        }
    }

    void SetImageData()
    {
        int i = 0;
        var systemlang = Application.systemLanguage;
        // systemlang = SystemLanguage.Japanese;
        //if (systemlang == SystemLanguage.Japanese)
        //{
        //    i = 1;
        //}

        if (systemlang == SystemLanguage.ChineseTraditional)
        {
            i = 2;
        }

        else if ((systemlang == SystemLanguage.ChineseSimplified))
        {

            i = 3;

        }
        else
        {
            i = 0;

        }
        if (i > localizationDatas.Count())
        {
            i = 0;
        }
        var data = localizationDatas[i];
        //logic for image or text
        if (MyImage != null)
        {

            SetImage(data);
        }

        else
        {

            if (Mytxt != null)
            {
                SetForcedText(data);
            }

        }
    }
    void SetImage(LocalizationImageSet data)
    {
        if (data.Image != null)
        {
            MyImage.sprite = data.Image;
        }
    }
    void SetForcedText(LocalizationImageSet data)
    {

        if (data.Str != null && data.Str.Length > 0)
        {
            Mytxt.text = data.Str;
        }
    }
}

    [System.Serializable]
    public class LocalizationImageSet
    {

        public string Name;
        public SystemLanguage myLanguage;
        public Sprite Image;
        public string Str;
    }

