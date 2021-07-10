using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using TMPro;
using UnityEngine.UI;

public class PetController : MonoBehaviour
{
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
    public bool spottedEnemy;
    public Transform firePoint;
    public float shootDelay;
    public float shootDelayMax;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        target = GameManager.instance.player.transform;
        rb = GetComponent<Rigidbody>();

        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(target.position, transform.position);
        if (distance <= lookRadius)
        {
            if(!spottedEnemy)
                FaceTarget();
            //botCanvas.transform.LookAt(Camera.main.transform.position);

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
                    //StartCoroutine(HitPlayerRoutine());
                }
            }
            if (distance <= agent.stoppingDistance)
            {
                anim.SetBool("walk", false);
                if (!spottedEnemy)
                    FaceTarget();
            }
        }


        if (!GameManager.instance.playerCon.enemyContact)
            spottedEnemy = false;
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
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("bot") && !anim.GetBool("walk"))
        {
            spottedEnemy = true;
            transform.GetChild(0).DOLookAt(other.gameObject.transform.position, 0.25f);
            //anim.SetTrigger("attack");
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("bot") && !anim.GetBool("walk"))
        {
            spottedEnemy = true;
            transform.GetChild(0).DOLookAt(other.gameObject.transform.position, 0.25f);
            anim.SetTrigger("attack");
            //ShootEnemy();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("bot"))
        {
            spottedEnemy = false;
        }
    }
    public void ShootEnemy()
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
}
