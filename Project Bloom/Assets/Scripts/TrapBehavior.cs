using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapBehavior : MonoBehaviour
{

    // Trap Attributes
    [SerializeField] bool isDamageTimed;           // Does damage occur at specific intervals? Ie. do all targets get hit simultaneously?
    [SerializeField] float damage;                  // Damage dealt
    [SerializeField] float triggerRate;             // Damage interval (If <=0, trap is single use)
    [SerializeField] float uses;                    // Total use time is triggerRate * uses

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other) // Should only fire if the object is a trap
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (uses > 0)
            {
                if (isDamageTimed)
                {

                }
                else
                {

                }
            }
            else
            {
                // Depleted State, Stop Coroutines and All Functionality
            }
        }
    }
}
