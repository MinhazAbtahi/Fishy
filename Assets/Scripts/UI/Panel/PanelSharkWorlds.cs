using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelSharkWorlds : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject sharkWorldItem;
    public GameObject sharkProgressItem;
    public Transform content;
    public SharkWorldItem[] sharkWorlds;
    public int worldCount = 10;
    public int equippedIndex;
    public int currentLevel;
    public int maxLevelCompleted;
    public List<Sprite> worldImages;
    string[] WorldName = new string[]
    {
    "Enchanted Forest",
    "Forgotten Land",
    "Death Valley",
    "Waste Land",
    "Middgard",
    "Silent Forest",
    "Spirit Hill",
    "Westerworld",
    "Badlands",
    "Hell Forest"
    };
   
    void Start()
    {
        playerPrefsLoad();
        Initialize();      
    }

    private void Initialize()
    {
        sharkWorlds = new SharkWorldItem[worldCount];
        for (int worldNo=0; worldNo < worldCount; worldNo++)
        {
            var obj = Instantiate(sharkWorldItem, content);
            var sharkworlditem =  sharkWorlds[worldNo] = obj.GetComponent<SharkWorldItem>();
            sharkworlditem.Initialize(worldNo, currentLevel, this, worldImages[worldNo], WorldName[worldNo]);

            //unlock worlds
            if (worldNo  <= maxLevelCompleted / 9)
            {
                sharkworlditem.lockImage.SetActive(false);
                sharkworlditem.btnEquip.gameObject.SetActive(true);
            }


            //progress panel laod
            if (worldNo >= maxLevelCompleted / 9)
            {
                

                for (int progressNo = 0; progressNo < 3; progressNo++)
                {
                    var progressItem = Instantiate(sharkProgressItem, content).GetComponent<SharkWorldProgressItem>();
                    progressItem.init(sharkworlditem, worldNo, progressNo, maxLevelCompleted, ((50 + (progressNo * 25)) + (worldNo * 50)));
                    sharkworlditem.AddProgressItem(progressItem);
                    progressItem.txtClearLevel.text = "Clear "+3*(progressNo+1)+" Levels";
                   
                    
                }
            }

        }
       
    }

    void playerPrefsLoad()
    {
        currentLevel = PlayerPrefs.GetInt("gamelevelKey", 0);
        maxLevelCompleted = PlayerPrefs.HasKey("maxLevelCompleted") ? PlayerPrefs.GetInt("maxLevelCompleted") : 0;

        if(maxLevelCompleted<= currentLevel)
        {
            maxLevelCompleted = currentLevel;
            PlayerPrefs.SetInt("maxLevelCompleted", maxLevelCompleted);
        }

        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
