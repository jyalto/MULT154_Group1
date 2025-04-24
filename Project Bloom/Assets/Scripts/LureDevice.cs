using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LureDevice : MonoBehaviour
{
    public GameObject explosion;
    TrapBehavior trapBehavior;

    private Coroutine myCoroutine = null;

    // Start is called before the first frame update
    void Start()
    {
        trapBehavior = gameObject.GetComponent<TrapBehavior>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && trapBehavior.uses == 0)
        {
            if (myCoroutine == null)
            {
                myCoroutine = StartCoroutine(DestroySelf());
            }
        }
    }

    private IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(7.5f);
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
