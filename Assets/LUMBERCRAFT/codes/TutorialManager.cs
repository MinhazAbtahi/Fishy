using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager instance;
    [Header("BUILDINGS")]
    public GameObject house;
    public GameObject mill;
    public GameObject shop;
    public GameObject houseComplete;
    public GameObject millComplete;
    public GameObject shopComplete;

    public GameObject house2;
    public GameObject houseComplete2;
    public GameObject house3;
    public GameObject houseComplete3;
    [Header("camera positions")]
    public GameObject houseCompleteCamPos;
    public GameObject millCompleteCamPos;
    public GameObject shopCompleteCamPos;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    public void Start()
    {
        if(PlayerPrefs.GetInt(GameManager.instance.homeKey, GameManager.instance.homeStatus) >= 1)
        {
            house.SetActive(false);
            houseComplete.SetActive(true);
        }
        else
        {
            house.SetActive(true);
            houseComplete.SetActive(false);
        }
        if (PlayerPrefs.GetInt(GameManager.instance.shopKey, GameManager.instance.shopStatus) == 1)
        {
            shop.SetActive(false);
            shopComplete.SetActive(true);
        }
        else
        {
            shop.SetActive(true);
            shopComplete.SetActive(false);
        }
        if (PlayerPrefs.GetInt(GameManager.instance.millKey, GameManager.instance.millStatus) == 1)
        {
            mill.SetActive(false);
            millComplete.SetActive(true);
        }
        else
        {
            mill.SetActive(true);
            millComplete.SetActive(false);
        }



        if (PlayerPrefs.GetInt(GameManager.instance.home2Key, GameManager.instance.home2Status) >= 1)
        {
            house2.SetActive(false);
            houseComplete2.SetActive(true);
        }
        else
        {
            house2.SetActive(true);
            houseComplete2.SetActive(false);
        }
        if (PlayerPrefs.GetInt(GameManager.instance.home3Key, GameManager.instance.home3Status) >= 1)
        {
            house3.SetActive(false);
            houseComplete3.SetActive(true);
        }
        else
        {
            house3.SetActive(true);
            houseComplete3.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
