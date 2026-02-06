using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField]
    float speed = 2.5f;

    [SerializeField]
    Vector3 targetPosition;

    public GameObject explosion;
    private GameObject whiteScreen;

    // Start is called before the first frame update
    void Start()
    {
        Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        whiteScreen = canvas.transform.Find("WhiteScreen")?.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("helicopter"))
        {
            Instantiate(explosion, transform.position, Quaternion.identity);
            whiteScreen.SetActive(true);
            Destroy(gameObject);
        }
    }
}
