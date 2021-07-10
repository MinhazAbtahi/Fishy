using UnityEngine;
using DG.Tweening;
using LumberCraft;

public class NPC : MonoBehaviour
{
    private float rotationSpeed = 50;
    /*[SerializeField]*/private float movementSpeed = 5;
    [SerializeField] private GameObject deathFx;

    public void Start()
    {

    }

    public void Update()
    {
        Move();
    }

    private void Move()
    {
        //transform.Translate(transform.forward * movementSpeed * Time.deltaTime);
    }

    

    public void HookFish(ResourceStack resourceStack)
    {
        Vector3 stackPos = resourceStack.transform.position;
        GameObject fxClone = Instantiate(deathFx, transform.position, Quaternion.identity);
        transform.DORotate(new Vector3(-90f, transform.position.y, transform.position.z), .1f);
        transform.DOMoveY(stackPos.y + 4f, .2f).OnComplete(() =>
        {
            transform.DORotate(new Vector3(90f, transform.position.y, transform.position.z), .1f);
            transform.DOMove(stackPos, .2f).OnComplete(() => Die());
            resourceStack.FillStack();
        });
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
