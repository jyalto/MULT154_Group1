using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingManager : MonoBehaviour
{
    private PlayerController playerObject;
    public Camera playerCamera;
    public List<GameObject> buildingPreviews = new List<GameObject>(); // Previews to be used
    private List<GameObject> previewObjects = new List<GameObject>(); // Physical, instantiated preview objects
    public List<GameObject> placedBuildings = new List<GameObject>();

    public bool buildingModeActive = false; // Feel free to manipulate externally however is
    public bool remoteEquipped = false;
    public bool activatorEquipped = false;
    public int selectedIndex = 0;
    [Range(0, 2)] public int currentRemoteChannel = 0;
    [Range(0, 2)] public int currentActivatorChannel = 0;
    public float buildRange = 8.0f;

    [SerializeField] AudioClip success;
    [SerializeField] AudioClip failure;

    public GameObject offhandObject;
    public TextMeshProUGUI remoteDisplay;
    public TextMeshProUGUI activatorDisplay;

    public GameObject currentPreview;
    private Transform targetPosition;
    private float heightOffset = 0;

    public LayerMask placementMask;
    public LayerMask buildingMask;

    // Start is called before the first frame update
    void Start()
    {
        playerObject = gameObject.GetComponent<PlayerController>();

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

        // Hammer Mode Functionality
        if (buildingModeActive)
        {
            ShowBuildPreview();

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
            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            //Ray ray = new Ray(transform.position + new Vector3(0, 0.5f, 0), transform.forward);

            RaycastHit hit;
            Physics.Raycast(ray, out hit, buildRange, placementMask);

            if (hit.transform != null)
            {
                ShowBuildPreview();
                currentPreview.transform.position = hit.point + new Vector3(0, heightOffset, 0);
            }
            else
            {
                HideBuildPreview();
            }

            // Building Mode Inputs
            if (Input.GetKeyDown(KeyCode.V))
            {
                selectedIndex = Scroll(selectedIndex, buildingPreviews.Count, 1);
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                selectedIndex = Scroll(selectedIndex, buildingPreviews.Count, -1);
            }

            if (Input.GetMouseButtonDown(0) && currentPreview.GetComponent<BuildingPreview>().canPlace)
            {
                GameObject temp = Instantiate(currentPreview.GetComponent<BuildingPreview>().buildingObject);
                temp.transform.position = currentPreview.transform.position;
                placedBuildings.Add(temp);

            }
        }
        // Remote Mode Functionailty
        else if (remoteEquipped)
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                currentRemoteChannel = Scroll(currentRemoteChannel, 3, 1);
                remoteDisplay.text = currentRemoteChannel.ToString();
                print(currentRemoteChannel.ToString());
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                currentRemoteChannel = Scroll(currentRemoteChannel, 3, -1);
                remoteDisplay.text = currentRemoteChannel.ToString();
                print(currentRemoteChannel.ToString());
            }

            if (Input.GetMouseButtonDown(0))
            {
                FireActivatorsOnChannel(currentRemoteChannel);
            }
        }
        // Activator Mode Functionailty
        else if (activatorEquipped)
        {
            if (Input.GetKeyDown(KeyCode.V))
            {
                currentActivatorChannel = Scroll(currentActivatorChannel, 3, 1);
                activatorDisplay.text = currentActivatorChannel.ToString();
                print(currentActivatorChannel.ToString());
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                currentActivatorChannel = Scroll(currentActivatorChannel, 3, -1);
                activatorDisplay.text = currentActivatorChannel.ToString();
                print(currentActivatorChannel.ToString());
            }
            if (Input.GetMouseButtonDown(0))
            {
                SetActivatorChannel(currentActivatorChannel);
            }
        }
        if (!buildingModeActive)
        {
            HideBuildPreview();
        }
    }

    void ShowBuildPreview()
    {
        if (!currentPreview.activeInHierarchy)
        {
            currentPreview.SetActive(true);
        }
    }

    void HideBuildPreview()
    {
        if (currentPreview.activeInHierarchy)
        {
            currentPreview.SetActive(false);
        }
    }

    int Scroll(int currentValue, int size, int increment)
    {
        int index = currentValue + increment;

        if (index < 0)
        {
            index = size - 1;
        }
        else if (index >= size)
        {
            index = index % size;
        } 

        return index;
    }

    void FireActivatorsOnChannel(int remoteChannel)
    {
        foreach (GameObject building in placedBuildings)
        {
            TrapBehavior trapBehavior = building.GetComponent<TrapBehavior>();
            if (building.CompareTag("Trap") && trapBehavior != null)
            {
                if (trapBehavior.activatorChannel == remoteChannel)
                {
                    trapBehavior.ActivateTrap();
                }
            }
        }
    }

    void SetActivatorChannel(int activatorChannel)
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        RaycastHit hit;
        Physics.Raycast(ray, out hit, buildRange, buildingMask);

        print(hit.collider.gameObject.name);

        if (hit.collider.CompareTag("Trap") && hit.collider.gameObject.GetComponent<TrapBehavior>().isActivatable)
        {
            hit.collider.gameObject.GetComponent<TrapBehavior>().ApplyRemoteActivator(currentActivatorChannel);
            offhandObject.GetComponent<AudioSource>().PlayOneShot(success);
        }
        else
        {
            offhandObject.GetComponent<AudioSource>().PlayOneShot(failure);
        }
    }

    void RemoveBuilding(GameObject building)
    {
        if (building != null)
        {
            placedBuildings.Remove(building);
        }
    }
}
