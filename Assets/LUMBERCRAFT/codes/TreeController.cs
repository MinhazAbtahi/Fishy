using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public enum TreeType
{
    normal,
    adTree
}
public class TreeController : MonoBehaviour
{
    public TreeType treeType;
    public int treeHealth;
    public GameObject leafFX;
    public GameObject treeTop;
    public GameObject spawnPos;
    public float spawnDelay;
    public float spawnDelayMax;
    public bool canSpawn;
    public UnityAction OnFarmComplete;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (canSpawn)
        {
            spawnDelay += Time.deltaTime;
            if(spawnDelay >= spawnDelayMax)
            {
                GameObject treeIns= Instantiate(GameManager.instance.treePrefab, spawnPos.transform.position, GameManager.instance.treePrefab.transform.rotation);
                treeIns.transform.parent = transform;
                canSpawn = false;
                spawnDelay = 0;
            }
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("ground"))
        {
            treeTop.transform.DOMoveY(-0.6f,0.5f);
        }
    }
}
