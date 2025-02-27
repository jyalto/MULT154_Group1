using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Having all building utilities in the same script gives extra functionality
public class Building : MonoBehaviour
{
    // Fields
    [SerializeField] BuildingType buildingType;     // Refer to enum "BuildingType"
    [SerializeField] bool hasActivator;             // Whether a remote activator can be applied
    [SerializeField] int remoteChannel;             // The channel that triggers the trap when fired

    private Collider trapCollider;
    [SerializeField] float durability;              // Condition of building
    [SerializeField] float damage;                  // Damage dealt
    [SerializeField] float triggerRate;             // Damage interval (If <=0, trap is single use)
    [SerializeField] float uses;                    // Total use time is triggerRate * uses

    public enum BuildingType // For type-checking
    {
        BARRICADE,      // Defensive structures. Eg. Window Boards, Scrap Wall
        SENTRY,         // Counter-Offensive structures. Eg. Pesticide, Turret
        TRAP,           // Stationary damage AOE. Eg. Trimmer, Gasoline
    }

    // Start is called before the first frame update
    void Start()
    {
        trapCollider = GetComponent<Collider>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (uses > 0)
            {
                // 
            }
            else
            {
                // Broken State
            }
        }
    }

    private void Activate()
    {

    }

    private void ApplyRemoteActivator(int channel)
    {
        hasActivator = true;
        remoteChannel = channel;
    }
}
