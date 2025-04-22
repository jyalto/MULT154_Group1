using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public enum ResourceTypes
    {
        // Basic    
        PLANT_PASTE,
        TOUGH_CLOTH,
        SCRAP_METAL,
        TECH_PARTS,
        // Special
        GAS_CAN,
        WEED_SPRAY,
    }

    public ResourceTypes ResourceType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
