using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public enum ResourceStatus
{
    source,
    pickup
}
public enum ResourceType
{
    wood,
    gold,
    silver,
    diamond
}
public class Farmable : MonoBehaviour
{
    public ResourceStatus resourceStatus;
    public ResourceType resourceType;
    public float magnetRange = 4f;
    public int sourceHealth;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(resourceStatus == ResourceStatus.pickup)
        {
            if (transform != null)
            {
                if (Vector3.Distance(GameManager.instance.player.transform.GetChild(0).position, transform.position) <= magnetRange)
                {
                    transform.DOMove(GameManager.instance.player.transform.GetChild(0).position, 0.4f);
                }
            }
        }  
    }
}
