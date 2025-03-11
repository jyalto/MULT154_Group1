using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

// A behavior script for all building prefabs. Often accompanied by a type-specific behavior script
public class BuildingBehavior : MonoBehaviour
{
    [Header("Common Attributes")]
    [SerializeField] BuildingType buildingType;     // Refer to enum "BuildingType"
    public float durability;
    public AudioClip constructionSound;

    [Header("Building Costs")]
    // TO BE IMPLEMENTED

    private Collider colliderComponent;

    public enum BuildingType // For type-checking
    {
        BARRICADE,      // Defensive structures. Eg. Window Boards, Scrap Wall
        SENTRY,         // Counter-Offensive structures. Eg. Pesticide, Turret
        TRAP,           // Stationary damage. Eg. Trimmer, Gasoline
    }

    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        gameObject.GetComponent<AudioSource>().PlayOneShot(constructionSound);

        durability = 100.0f;

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
}
