using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelicopterMovement : MonoBehaviour
{
    [SerializeField]
    float speed = 5f;

    [SerializeField]
    Vector3 targetPosition;

    public GameObject heldMissile;
    public GameObject missilePrefab;

    private int missileCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position == targetPosition)
        {
            heldMissile.SetActive(false);
            if (missileCount < 1)
            {
                Instantiate(missilePrefab, missilePrefab.transform.position, missilePrefab.transform.rotation);
                missileCount++;
            }
        }
    }

    void FixedUpdate()
    {
        if (gameObject.activeSelf)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                speed * Time.deltaTime
            );
        }
    }
}
