using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public enum botClass
{
    follower,
    shooter,
    shooter4Dir,
    sniper,
    cloner,
    bomber
}
public class BotController : MonoBehaviour
{
    public botClass bc;
    public Animator anim;
    Rigidbody rb;
    public int[] damage;
    public GameObject[] bulletTypes;
    NavMeshAgent agent;
    public Transform target;
    public float lookRadius = 25f;
    public float shootRadius;
    public int health;
    public int maxHealth;
    public bool canAttack;
    public bool attacked;
    public bool dead;
    public bool spottedPlayer;
    [Header("Canvas")]
    public GameObject botCanvas;
    public Image ImgHealthBar;
    [Header("shooter")]
    public Transform firePoint;
    public float shootDelay;
    public float shootDelayMax;
    [Header("sniper")]
    public GameObject ray;
    [Header("cloner")]
    public int cloneLevel;
    [Header("bomber")]
    public GameObject bombPrefab;
    public float bombRadius;
    public float bombDelay;
    public float bombDelayMax;

    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();

        target = GameManager.instance.player.transform;
        rb = GetComponent<Rigidbody>();

        health = maxHealth;
        //ImgHealthBar.fillAmount = (float) health / (float) maxHealth;
    }

    void Update()
    {
        if (!GameManager.instance.startGame) return;
        if (dead) return;
        if (GameManager.instance.playerCon.dead)
        {
            agent.enabled = false;
            return;
        }      
        if (GameManager.instance.victory) return;
        //if (!GameManager.instance.gameStart) return;


        float distance = Vector3.Distance(target.position, transform.position);
        if (distance <= lookRadius)
        {
            //botCanvas.transform.LookAt(Camera.main.transform.position);

            if (!spottedPlayer)
            {
                if (bc == botClass.follower)
                {
                    SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.followguySFX);

                }
                if (bc == botClass.sniper)
                {
                    SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.sniperSFX);
                }
                if (bc == botClass.shooter)
                {
                    SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.greenShooterSFX);
                }
                if (bc == botClass.bomber)
                {
                    SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.bomberSFX);
                }
                if (bc == botClass.cloner)
                {
                    SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.giantSkeletonSFX);

                }

                spottedPlayer = true;
            }


            if (bc == botClass.follower || bc == botClass.cloner)
            {
                if (agent.isStopped)
                {
                    anim.SetBool("walk", false);
                }
                else
                {
                    anim.SetBool("walk", true);
                }

                agent.SetDestination(target.position);
                if (distance <= agent.stoppingDistance + 2f)
                {
                    canAttack = true;

                    if (canAttack && !attacked)
                    {
                        StartCoroutine(HitPlayerRoutine());
                    }
                }
                if (distance <= agent.stoppingDistance)
                {
                    FaceTarget();
                }
            }
            
        }
        if (bc == botClass.shooter)
        {
            if (distance <= shootRadius)
            {
                transform.GetChild(0).DOLookAt(target.position, 0.25f);
                ShootPlayer();
            }
        }
        if (bc == botClass.sniper)
        {
            if (distance <= shootRadius)
            {
                transform.GetChild(0).DOLookAt(target.position, 0.25f);
                SnipePlayer();
            }
            else
            {
                ray.SetActive(false);
            }
        }
        if (bc == botClass.bomber)
        {
            if (distance <= bombRadius)
            {
                transform.GetChild(0).DOLookAt(target.position, 0.25f);
                BombPlayer();
            }
        }
    }
    void FaceTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.GetChild(0).rotation = Quaternion.Slerp(transform.GetChild(0).rotation, lookRotation, Time.deltaTime * 5);

    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lookRadius);
        Gizmos.DrawWireSphere(transform.position, shootRadius);
    }

    IEnumerator HitPlayerRoutine()
    {
        canAttack = false;
        attacked = true;
        anim.SetTrigger("attack");
        var atk = SoundManager.sharedInstance.GetRandomEnemyAttackSFX();
        SoundManager.sharedInstance.PlaySFX(atk);
        GameManager.instance.playerCon.health-= damage[0];
        PlayerPrefs.SetInt(GameManager.instance.playerCon.healthKey, GameManager.instance.playerCon.health);
        UIManager.instance.healthBar.fillAmount = (float)GameManager.instance.playerCon.health / (float)GameManager.instance.playerCon.maxHealth;

        if(GameManager.instance.playerCon.health <= 0)
        {
            GameManager.instance.playerCon.anim.SetTrigger("die");
            GameManager.instance.playerCon.dead = true;
            GameManager.instance.playerCon.enabled = false;
            GameManager.instance.playerCon.myAgent.enabled = false;
            GameManager.instance.playerCon.rb.isKinematic = true;
            GameManager.instance.playerCon.muzzleFlash.SetActive(false);
            UIManager.instance.gamePanel.SetActive(false);
            StartCoroutine(UIManager.instance.GameOverPanelDelayRoutine());

            var clip = SoundManager.sharedInstance.GetRandomBotkillSFX();
            SoundManager.sharedInstance.PlaySFX(clip);
            FPG.HapticController.PlayEnemyDestroyedHaptic(clip.length);

            GameManager.instance.playerCon.health = GameManager.instance.playerCon.maxHealth;
            PlayerPrefs.SetInt(GameManager.instance.playerCon.healthKey, GameManager.instance.playerCon.health);
        }
        //GetComponentInChildren<Animator>().SetTrigger("isEating");
        //UIManager.instance.hurtFrame.SetActive(true);
        //SoundManager.Instance.PlaySFX(SoundManager.Instance.hitSFX);

        yield return new WaitForSeconds(0.35f);

        //UIManager.instance.hurtFrame.SetActive(false);//fails to turn off sometimes 

        yield return new WaitForSeconds(2f);

        //UIManager.instance.hurtFrame.SetActive(false);
        canAttack = true;
        attacked = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("bullet"))
        {
            int dmg = 1;
            health-= dmg;
            //ImgHealthBar.fillAmount = (float)health / (float)maxHealth;
            GameObject dmgPop = Instantiate(GameManager.instance.dmgTextPopupPrefab, transform.position, Quaternion.identity);
            dmgPop.transform.GetComponent<TextMeshPro>().text = "-" + dmg * 10;
            dmgPop.transform.GetComponent<TextMeshPro>().DOFade(50, .3f);
            dmgPop.transform.DOMoveY(dmgPop.transform.position.y + 1, 0.3f);
            Destroy(dmgPop, 0.3f);

            Destroy(other.gameObject);
            GameObject pop = Instantiate(GameManager.instance.hitFX, transform.position, Quaternion.identity);
            Destroy(pop, 1f);

            transform.DOShakeScale(0.2f, 0.1f);

            if (health <= 0)
            {
                Death();
            }

        }
    }

    public void ShootPlayer()
    {
        shootDelay += Time.deltaTime;
        if (shootDelay >= shootDelayMax)
        {
            anim.SetTrigger("attack");
            GameObject bulletIns = Instantiate(bulletTypes[0], firePoint.position, firePoint.rotation);
            Destroy(bulletIns, 7f);
            shootDelay = 0;
            SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.enemyShootSFX);
        }
    }
    public void SnipePlayer()
    {
        
        shootDelay += Time.deltaTime;

        if (shootDelay >= 0.5f && shootDelay <= 1.5f)
        {
            ray.SetActive(true);
        }
        if (shootDelay >= shootDelayMax)
        {
            anim.SetTrigger("attack");
            GameObject bulletIns = Instantiate(bulletTypes[0], firePoint.position, firePoint.rotation);
            bulletIns.GetComponent<BulletController>().speed = 25;
            Destroy(bulletIns, 7f);
            shootDelay = 0;
            SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.sniperShotSFX);

            ray.SetActive(false);
        }
     
    }
    public void BombPlayer()
    {
        bombDelay += Time.deltaTime;
        if (bombDelay >= bombDelayMax)
        {
            anim.SetTrigger("attack");
            GameObject bombIns = Instantiate(bombPrefab, firePoint.position, firePoint.rotation);
            bombIns.transform.DOMoveX(target.position.x,1.5f);
            bombIns.transform.DOMoveZ(target.position.z, 1.5f);
            bombIns.transform.DOMoveY(transform.position.y + 7, 0.5f).SetLoops(2,LoopType.Yoyo);
            bombIns.transform.GetComponent<MeshRenderer>().material.DOColor(Color.red, 2f);
            Destroy(bombIns, 7f);
            bombDelay = 0;
            SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.enemyShootSFX);
        }
    }
    public void GiveDamage()
    {

    }
    public void Death()
    {
        anim.SetTrigger("die");
        GameManager.instance.playerCon.enemyContact = false;
        GameManager.instance.playerCon.closestEnemy = null;
        BotSpawner.aliveEnemyCount--;
        dead = true;
        gameObject.tag = "botDead";
        gameObject.layer = 11;
        //GetComponentInChildren<Animator>().SetTrigger("isDied");
        enabled = false;
        agent.enabled = false;
        GetComponent<Collider>().enabled = false;
        rb.useGravity = false;
        GameManager.instance.player.GetComponent<PlayerController>().canShoot = false;
        Destroy(gameObject,2f);
        GameObject hit = Instantiate(GameManager.instance.popFX, transform.position, Quaternion.identity);
        //hit.GetComponent<ParticleSystem>().startColor=gameObject.GetComponent<MeshRenderer>().material.color;
        Destroy(hit, 1f);
        GameObject soul = Instantiate(GameManager.instance.enemyDeathFX, transform.position, Quaternion.identity);
        Destroy(soul, 4f);
        //coin spawn position needs to be slightly up
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
        GameObject coinIns = Instantiate(GameManager.instance.coinPrefab, pos, GameManager.instance.coinPrefab.transform.rotation);

        //Camera.main.transform.DOShakeRotation(0.35f,1,10,90);

        var clip = SoundManager.sharedInstance.GetRandomBotkillSFX();
        SoundManager.sharedInstance.PlaySFX(clip);
        FPG.HapticController.PlayEnemyDestroyedHaptic(clip.length);

        if (ray != null) ray.SetActive(false);

        if (bc == botClass.cloner && cloneLevel==0)
        {
            SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.doubleSkeletonSFX);

            for (int i = 0; i < 2; i++)
            {
                GameObject clone = Instantiate(BotSpawner.instance.enemy_cloner, transform.position, BotSpawner.instance.enemy_cloner.transform.rotation);
                //clone.transform.localScale = new Vector3(1.5f, 2, 1.5f);
                //clone.GetComponent<MeshRenderer>().material = BotSpawner.instance.clonerMat;
                clone.GetComponent<BotController>().cloneLevel = 1;
                clone.GetComponent<BotController>().health /= 2;
            }
        }
        GameManager.instance.botsKilled++;
        UIManager.instance.levelProgBar.fillAmount = (float)GameManager.instance.botsKilled / (float)GameManager.instance.botsToKill;
        GameManager.instance.CheckLevelProgression();
        FPG.Networking.getInstance().send_unpaid_user_battle_status(0, 1, 0, GameData.EnemyCount);
        //GameManager.UpdateGameData(GameData.EnemyCount);

    }
}
