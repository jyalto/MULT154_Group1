using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    /*private GameObject Player;
    public float speed = 100.0f;

    private AudioSource audioSource;
    private Vector3 direction = new Vector3(0, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        direction = (Player.transform.position - transform.position).normalized;
        Destroy(gameObject, 20f);
        audioSource = GetComponent<AudioSource>();
        //audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }*/

    private void OnCollisionEnter (Collision collision) 
    {
        Destroy(gameObject);
    }
}
