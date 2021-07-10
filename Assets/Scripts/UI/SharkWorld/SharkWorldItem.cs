using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Purchasing;
using UnityEngine.UI;
using TMPro;

public class SharkWorldItem : MonoBehaviour
{
    PanelSharkWorlds sharkWorldPanel;
    public Button btnEquip;
    public Image imgProdImage;
    public GameObject lockImage;
    public TextMeshProUGUI txtWorldName;

    int worldId;
    int currentLevel;
    public List<SharkWorldProgressItem> progressItems;
    public void Initialize(int id,int CurrentLevel,PanelSharkWorlds psw,Sprite worldImage, string worldName )
    {
        sharkWorldPanel = psw;
        worldId = id;
        currentLevel = CurrentLevel;
        AddListener();
        imgProdImage.sprite = worldImage;
        txtWorldName.text = worldName;
        //image
        //object
    }
    public void AddListener()
    {
        btnEquip.onClick.AddListener(() => GotToWorld());


    }
    public void AddProgressItem(SharkWorldProgressItem progressItem)
    {
        progressItems.Add(progressItem);

    }

    public List<SharkWorldProgressItem> getallProgressItem()
    {
        return progressItems;

    }
    public void GotToWorld()
    {
        if ((PlayerPrefs.HasKey("worldKey") ? PlayerPrefs.GetInt("worldKey") : 0) != worldId)
        {
            //SoundManager.SharedManager().PlaySFX(SoundManager.SharedManager().TapButton);
            PlayerPrefs.SetInt("worldKey", worldId);
            GameManager.instance.worldID = worldId;

            PlayerPrefs.SetInt("gamelevelKey", (worldId * 9) + PlayerPrefs.GetInt("levelCountWorld" + worldId));
            GameManager.instance.gameLevel = (worldId * 9) + PlayerPrefs.GetInt("levelCountWorld" + worldId);

            PlayerPrefs.SetInt("levelKey", GameManager.instance.gameLevel % 9);
            GameManager.instance.levelID = GameManager.instance.gameLevel % 9;

            UIManager.instance.UpdateWorldUI(worldId);

            Debug.Log("currentLevel =" + PlayerPrefs.GetInt("gamelevelKey"));
        }

        //Destroy(sharkWorldPanel.gameObject);
        Debug.Log("goto world number ........" + worldId);

        //GameManager.instance.SetCurrentWorld();
        UIManager.instance.UpdateWorldUI(worldId);
        sharkWorldPanel.gameObject.SetActive(false);
        UIManager.instance.HomeCallback();
    }
}
