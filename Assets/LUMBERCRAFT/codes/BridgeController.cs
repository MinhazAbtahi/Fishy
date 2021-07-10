using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeController : MonoBehaviour
{
    public static BridgeController instance;
    public GameObject[] bridgeIncomplete;
    public GameObject[] bridgeComplete;
    [HideInInspector] public string bridgeDataKey = "bridgeDataKey";
    public int[] bridgeStatusData;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < bridgeStatusData.Length; i++)
        {
            bridgeStatusData[i] = PlayerPrefs.GetInt(bridgeDataKey + i);
            
            if(bridgeStatusData[i]==0)
                bridgeIncomplete[i].SetActive(true);
            else if (bridgeStatusData[i] == 1)
                bridgeComplete[i].SetActive(true);
        }
        GameManager.instance.surface.BuildNavMesh();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
