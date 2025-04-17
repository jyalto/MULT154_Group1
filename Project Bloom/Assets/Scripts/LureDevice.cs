using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LureDevice : MonoBehaviour
{
    public PlayerController playerObject;
    public GameObject explosion;

    private Coroutine myCoroutine = null;

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && playerObject.lureActive == true)
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
