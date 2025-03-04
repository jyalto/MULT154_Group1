using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

// Having all building utilities in the same script gives extra functionality
public class BuildingBehavior : MonoBehaviour
{
    // Common Attributes
    [SerializeField] BuildingType buildingType;     // Refer to enum "BuildingType"
    [SerializeField] bool isActivatable;            // Whether a remote activator can be applied
    [SerializeField] bool hasActivator;             // Whether a remote activator is applied
    public int remoteChannel;                       // The channel that triggers the building when fired

    private Collider colliderComponent;
    [SerializeField] float durability;              // Condition of building


    public enum BuildingType // For type-checking
    {
        BARRICADE,      // Defensive structures. Eg. Window Boards, Scrap Wall
        SENTRY,         // Counter-Offensive structures. Eg. Pesticide, Turret
        TRAP,           // Stationary damage. Eg. Trimmer, Gasoline
    }

    // Start is called before the first frame update
    void Start()
    {
        colliderComponent = GetComponent<Collider>();

        switch (buildingType)
        {
            case BuildingType.BARRICADE:
                break;
            case BuildingType.SENTRY:
                break;
            case BuildingType.TRAP:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Activate(int channel)
    {
        if (hasActivator && remoteChannel == channel)
        {
            switch (buildingType)
            {
                case BuildingType.BARRICADE:
                    break;
                case BuildingType.SENTRY:
                    break;
                case BuildingType.TRAP:
                    break;
            }
        }
        // Fire Coroutine to set IsActivatable back to true
    }

    private void ApplyRemoteActivator(int channel)
    {
        hasActivator = true;
        remoteChannel = channel;
    }
}
