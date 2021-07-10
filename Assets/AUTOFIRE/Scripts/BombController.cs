using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombController : MonoBehaviour
{
    public float speed;
    public float upForce;
    public Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ground"))
        {
            Destroy(gameObject);
            GameObject pop = Instantiate(GameManager.instance.hitFX, transform.position, Quaternion.identity);
            pop.transform.localScale *= 2;
            Destroy(pop, 1f);
        }
    }
}
