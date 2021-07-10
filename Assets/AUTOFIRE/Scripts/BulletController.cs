using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float speed;


    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 4f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
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
