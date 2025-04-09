using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingManager : MonoBehaviour
{
    public PlayerController playerObject;
    public Camera playerCamera;
    public List<GameObject> buildingPreviews = new List<GameObject>(); // Previews to be used
    private List<GameObject> previewObjects = new List<GameObject>(); // Physical, instantiated preview objects

    public bool buildingModeActive = false; // Feel free to manipulate externally however is easiest
    public int selectedIndex = 0;
    public float buildRange = 5.0f;

    private GameObject currentPreview;
    private Transform targetPosition;
    private float heightOffset = 0;

    public LayerMask placementMask;

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        // Pre-load ghosts
        foreach (GameObject building in buildingPreviews)
        {
            GameObject previewObject = Instantiate(building);

            previewObject.SetActive(false); // To be re-toggled for visibility when selected

            previewObjects.Add(previewObject);
        }
        currentPreview = previewObjects[selectedIndex];
        heightOffset = currentPreview.GetComponent<BuildingPreview>().mesh.GetComponent<Renderer>().bounds.extents.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && playerObject.lureDevice == null) // Toggle building mode
        {
            buildingModeActive = !buildingModeActive;
        }

        // Building Mode Functionality
        if (buildingModeActive)
        {

            // Update preview
            if (currentPreview != previewObjects[selectedIndex])
            {
                currentPreview.SetActive(false);
                currentPreview = previewObjects[selectedIndex];
                heightOffset = currentPreview.GetComponent<BuildingPreview>().mesh.GetComponent<Renderer>().bounds.extents.y; // Gets properly offset placement position based on visual bounds
            }

            if (!currentPreview.activeInHierarchy)
            {
                currentPreview.SetActive(true);
            }

            Ray ray = new Ray(transform.position + new Vector3(0, 0.5f, 0), transform.forward);

            RaycastHit hit;
            Physics.Raycast(ray, out hit, buildRange, placementMask);

            if (hit.transform != null)
            {
                currentPreview.transform.position = hit.point + new Vector3(0, heightOffset, 0);
            }

            // Building Mode Inputs
            if (Input.GetKeyDown(KeyCode.V))
            {
                UpdateIndex(1);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                UpdateIndex(-1);
            }

            if (Input.GetMouseButtonDown(1) && currentPreview.GetComponent<BuildingPreview>().canPlace)
            {
                GameObject temp = Instantiate(currentPreview.GetComponent<BuildingPreview>().buildingObject);
                temp.transform.position = currentPreview.transform.position;
            }
        }
        else
        {
            currentPreview.SetActive(false);
        }

        if (playerObject.lureDevice != null)
        {
            buildingModeActive = false;
        }
    }

    void UpdateIndex(int increment)
    {
        selectedIndex += increment;

        if (selectedIndex < 0)
        {
            selectedIndex = buildingPreviews.Count - 1;
        }
        else if (selectedIndex >= buildingPreviews.Count)
        {
            selectedIndex = selectedIndex % buildingPreviews.Count;
        }
        Debug.Log("Current trap index: " + selectedIndex + " | Trap Type: " + buildingPreviews[selectedIndex].name);
    }
}
