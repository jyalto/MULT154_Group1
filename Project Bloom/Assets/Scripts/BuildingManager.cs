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
    private Animator playerAnim;
    public Camera playerCamera;
    public List<GameObject> buildingPreviews = new List<GameObject>();
    private List<GameObject> previewObjects = new List<GameObject>();
    public List<GameObject> placedBuildings = new List<GameObject>();

    public bool buildingModeActive = false;

    // -----------------------------
    // REMOTE / ACTIVATOR (commented)
    // -----------------------------
    // public bool remoteEquipped = false;
    // public bool activatorEquipped = false;
    // [Range(0, 2)] public int currentRemoteChannel = 0;
    // [Range(0, 2)] public int currentActivatorChannel = 0;

    public int selectedIndex = 0;
    public float buildRange = 8.0f;

    [SerializeField] AudioClip success;
    [SerializeField] AudioClip failure;

    public GameObject offhandObject;

    // -----------------------------
    // REMOTE / ACTIVATOR UI (commented)
    // -----------------------------
    public TextMeshProUGUI remoteDisplay;
    public TextMeshProUGUI activatorDisplay;

    public GameObject currentPreview;
    private float heightOffset = 0;

    public LayerMask placementMask;
    public LayerMask buildingMask;

    // D-Pad virtual button state
    private float prevDpadX = 0f;
    private float dpadX = 0f;

    void Start()
    {
        playerObject = gameObject.GetComponent<PlayerController>();
        playerAnim = GetComponent<Animator>();

        foreach (GameObject building in buildingPreviews)
        {
            GameObject previewObject = Instantiate(building);
            previewObject.SetActive(false);
            previewObjects.Add(previewObject);
        }

        currentPreview = previewObjects[selectedIndex];
        heightOffset = currentPreview.GetComponent<BuildingPreview>().mesh.GetComponent<Renderer>().bounds.extents.y;
    }

    void Update()
    {
        // Read D-Pad axis and detect button-like presses
        prevDpadX = dpadX;
        dpadX = Input.GetAxis("DPad X");

        bool dpadRightPressed = prevDpadX <= 0.5f && dpadX > 0.5f;
        bool dpadLeftPressed = prevDpadX >= -0.5f && dpadX < -0.5f;

        // -----------------------------
        // BUILDING MODE (HAMMER)
        // -----------------------------
        if (buildingModeActive)
        {
            ShowBuildPreview();

            if (currentPreview != previewObjects[selectedIndex])
            {
                currentPreview.SetActive(false);
                currentPreview = previewObjects[selectedIndex];
                heightOffset = currentPreview.GetComponent<BuildingPreview>().mesh.GetComponent<Renderer>().bounds.extents.y;
            }

            if (!currentPreview.activeInHierarchy)
                currentPreview.SetActive(true);

            Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit;
            Physics.Raycast(ray, out hit, buildRange, placementMask);

            if (hit.transform != null)
                currentPreview.transform.position = hit.point + new Vector3(0, heightOffset, 0);
            else
                HideBuildPreview();

            // Building selection (D-Pad or keyboard)
            if (dpadRightPressed || Input.GetKeyDown(KeyCode.V))
                selectedIndex = Scroll(selectedIndex, buildingPreviews.Count, 1);

            if (dpadLeftPressed || Input.GetKeyDown(KeyCode.C))
                selectedIndex = Scroll(selectedIndex, buildingPreviews.Count, -1);

            // Place building
            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.JoystickButton5))
            {
                if (currentPreview.GetComponent<BuildingPreview>().canPlace)
                {
                    bool enoughResources = true;

                    PlayerController pc = playerObject.GetComponent<PlayerController>();
                    BuildingPreview bp = currentPreview.GetComponent<BuildingPreview>();

                    if (pc.CheckResource(Resource.ResourceTypes.PLANT_PASTE) < bp.costPaste) enoughResources = false;
                    if (pc.CheckResource(Resource.ResourceTypes.TOUGH_CLOTH) < bp.costCloth) enoughResources = false;
                    if (pc.CheckResource(Resource.ResourceTypes.SCRAP_METAL) < bp.costMetal) enoughResources = false;
                    if (pc.CheckResource(Resource.ResourceTypes.TECH_PARTS) < bp.costTech) enoughResources = false;
                    if (pc.CheckResource(Resource.ResourceTypes.GAS_CAN) < bp.costGas) enoughResources = false;
                    if (pc.CheckResource(Resource.ResourceTypes.WEED_SPRAY) < bp.costSpray) enoughResources = false;

                    if (enoughResources)
                    {
                        pc.RemoveResource(Resource.ResourceTypes.PLANT_PASTE, bp.costPaste);
                        pc.RemoveResource(Resource.ResourceTypes.TOUGH_CLOTH, bp.costCloth);
                        pc.RemoveResource(Resource.ResourceTypes.SCRAP_METAL, bp.costMetal);
                        pc.RemoveResource(Resource.ResourceTypes.TECH_PARTS, bp.costTech);
                        pc.RemoveResource(Resource.ResourceTypes.GAS_CAN, bp.costGas);
                        pc.RemoveResource(Resource.ResourceTypes.WEED_SPRAY, bp.costSpray);

                        GameObject temp = Instantiate(currentPreview.GetComponent<BuildingPreview>().buildingObject);
                        temp.transform.position = currentPreview.transform.position;
                        placedBuildings.Add(temp);
                    }
                    else
                    {
                        Debug.Log("Not enough resources!");
                    }

                    playerAnim.SetTrigger("Activate");
                }
            }
        }

        // -----------------------------
        // REMOTE MODE (commented)
        // -----------------------------
        /*
        else if (remoteEquipped)
        {
            if (dpadRightPressed || Input.GetKeyDown(KeyCode.V))
            {
                currentRemoteChannel = Scroll(currentRemoteChannel, 3, 1);
                remoteDisplay.text = currentRemoteChannel.ToString();
            }

            if (dpadLeftPressed || Input.GetKeyDown(KeyCode.C))
            {
                currentRemoteChannel = Scroll(currentRemoteChannel, 3, -1);
                remoteDisplay.text = currentRemoteChannel.ToString();
            }

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.JoystickButton5))
            {
                FireActivatorsOnChannel(currentRemoteChannel);
                playerAnim.SetTrigger("Activate");
            }
        }
        */

        // -----------------------------
        // ACTIVATOR MODE (commented)
        // -----------------------------
        /*
        else if (activatorEquipped)
        {
            if (dpadRightPressed || Input.GetKeyDown(KeyCode.V))
            {
                currentActivatorChannel = Scroll(currentActivatorChannel, 3, 1);
                activatorDisplay.text = currentActivatorChannel.ToString();
            }

            if (dpadLeftPressed || Input.GetKeyDown(KeyCode.C))
            {
                currentActivatorChannel = Scroll(currentActivatorChannel, 3, -1);
                activatorDisplay.text = currentActivatorChannel.ToString();
            }

            if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.JoystickButton5))
            {
                SetActivatorChannel(currentActivatorChannel);
                playerAnim.SetTrigger("Activate");
            }
        }
        */

        if (!buildingModeActive)
            HideBuildPreview();
    }

    void ShowBuildPreview()
    {
        if (!currentPreview.activeInHierarchy)
            currentPreview.SetActive(true);
    }

    void HideBuildPreview()
    {
        if (currentPreview.activeInHierarchy)
            currentPreview.SetActive(false);
    }

    int Scroll(int currentValue, int size, int increment)
    {
        int index = currentValue + increment;

        if (index < 0)
            index = size - 1;
        else if (index >= size)
            index = index % size;

        return index;
    }

    // -----------------------------
    // REMOTE LOGIC (commented)
    // -----------------------------
    /*
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
                    playerAnim.SetTrigger("Activate");
                }
            }
        }
    }
    */

    // -----------------------------
    // ACTIVATOR LOGIC (commented)
    // -----------------------------
    /*
    void SetActivatorChannel(int activatorChannel)
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        Physics.Raycast(ray, out hit, buildRange, buildingMask);

        if (hit.collider != null && hit.collider.CompareTag("Trap") && hit.collider.gameObject.GetComponent<TrapBehavior>().isActivatable)
        {
            hit.collider.gameObject.GetComponent<TrapBehavior>().ApplyRemoteActivator(currentActivatorChannel);
            offhandObject.GetComponent<AudioSource>().PlayOneShot(success);
            playerAnim.SetTrigger("Activate");
        }
        else
        {
            offhandObject.GetComponent<AudioSource>().PlayOneShot(failure);
        }
    }
    */
}
