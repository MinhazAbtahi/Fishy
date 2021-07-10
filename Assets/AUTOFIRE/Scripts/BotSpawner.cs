using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotSpawner : MonoBehaviour
{
    public static BotSpawner instance;
    public Transform[] enemySpawnPoint;
    public GameObject[] enemy;

    public float spawnTime;
    public float spawnTimeMax;
    public int enemyCount;
    public int enemyCountMax;
    public static int aliveEnemyCount;
    public GameObject enemy_cloner;
    public Material clonerMat;


    private void Awake()
    {
        if (instance == null) instance = this;

        aliveEnemyCount = 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        SpawnTimerController();
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.startGame) return;
        if (GameManager.instance.playerCon.dead) return;
        if (GameManager.instance.victory) return;
        //if (!GameManager.instance.gameStart) return;
        if(aliveEnemyCount < TagManager.GetEnemySpawnAmount())
        {
            spawnTime += Time.deltaTime;
            if (spawnTime >= spawnTimeMax)
            {
                aliveEnemyCount++;
                enemyCount++;
                GameObject botIns = Instantiate(enemy[Random.Range(0, enemy.Length)], enemySpawnPoint[Random.Range(0, enemySpawnPoint.Length)].position, Quaternion.identity, transform);
                spawnTime = 0;
            }
        }
    }
    private void FixedUpdate()
    {
        //very inefficient code, will change later
        //GameObject[] botList;
        //botList = GameObject.FindGameObjectsWithTag("bot");
        //aliveEnemyCount = botList.Length;
    }
    public void SpawnTimerController()
    {
        spawnTimeMax = TagManager.GetEnemySpawnTime();
        //spawnTimeMax = 7;
        //spawnTimeMax = 6 - ((float)GameManager.instance.worldID / 2) - (float)GameManager.instance.levelID * 0.1f;

        if (spawnTimeMax <= 1) spawnTimeMax = 1;
    }
}

