using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroySelf());
    }

    private IEnumerator DestroySelf()
    {
        yield return new WaitForSeconds(0.1f);
        Collider myCollider = GetComponent<Collider>();
        myCollider.enabled = false;
        yield return new WaitForSeconds(1.9f);
        Destroy(gameObject);
    }
}
