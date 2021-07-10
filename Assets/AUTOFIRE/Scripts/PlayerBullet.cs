using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public float speed = 70f;
    private Transform target;
 
    public void GetTarget(Transform _target)
    {
        target = _target;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (target == null)
        {
            Destroy(this.gameObject);
            return;
        }
        Vector3 dir = target.position - transform.position;

        float distanceThisFrame = speed * Time.deltaTime;

        transform.Translate(dir.normalized * distanceThisFrame, Space.World);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("playerShield"))
        {
            Destroy(gameObject);
            Destroy(other.gameObject);
            GameObject pop = Instantiate(GameManager.instance.hitFX, transform.position, Quaternion.identity);
            Destroy(pop, 1f);
            GameManager.instance.playerCon.shieldLevel--;
            if (GameManager.instance.playerCon.shieldLevel <= 0)
                GameManager.instance.playerCon.hasShield = false;
        }
        if (other.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
            GameObject pop = Instantiate(GameManager.instance.hitFX, transform.position, Quaternion.identity);
            Destroy(pop, 1f);
        }
    }
}

