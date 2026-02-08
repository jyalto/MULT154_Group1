using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Notebook : MonoBehaviour
{
    public GameObject playerObject;
    public GameObject clipperImg;
    public GameObject lureImg;
    public AudioClip toggleSound;

    public bool isOpen = false;
    public float openScale = 1.5f;
    public float closeScale = 1.0f;
    public float smoothing = 0.05f;

    // 0 - Paste // 1 - Cloth // 2 - Metal // 3 - Tech / 4 - Gas // 5 - Spray //
    public TextMeshProUGUI buildName;
    public TextMeshProUGUI buildDescription;

    public TextMeshProUGUI[] resources;
    public TextMeshProUGUI[] costs;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Toggle input
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.JoystickButton7))
        {
            isOpen = !isOpen;

            GetComponent<AudioSource>().pitch = Random.Range(0.9f, 1.1f);
            GetComponent<AudioSource>().PlayOneShot(toggleSound);

            if (isOpen)
                RefreshText();
        }

        // Animate notebook
        if (isOpen)
        {
            GetComponent<RectTransform>().position = Vector2.Lerp(
                GetComponent<RectTransform>().position,
                new Vector2(Screen.width / 2, Screen.height / 3),
                smoothing
            );

            GetComponent<RectTransform>().localScale = Vector3.Lerp(
                GetComponent<RectTransform>().localScale,
                new Vector3(openScale, openScale, 1),
                smoothing
            );
        }
        else
        {
            GetComponent<RectTransform>().position = Vector2.Lerp(
                GetComponent<RectTransform>().position,
                new Vector2(Screen.width / 2, -Screen.height / 3),
                smoothing
            );

            GetComponent<RectTransform>().localScale = Vector3.Lerp(
                GetComponent<RectTransform>().localScale,
                new Vector3(closeScale, closeScale, 1),
                smoothing
            );
        }
    }


    private void RefreshText()
    {
        PlayerController pc = playerObject.GetComponent<PlayerController>();
        BuildingPreview bp = playerObject.GetComponent<BuildingManager>().currentPreview.GetComponent<BuildingPreview>();

        buildName.text = bp.buildingObject.GetComponent<BuildingBehavior>().buildingName;
        buildDescription.text = bp.buildingObject.GetComponent<BuildingBehavior>().buildingDescription;

        if (buildName.text == "Lure Bomb")
        {
            clipperImg.SetActive(false);
            lureImg.SetActive(true);
        }
        else
        {
            lureImg.SetActive(false);
            clipperImg.SetActive(true);
        }

        resources[0].text = pc.CheckResource(Resource.ResourceTypes.PLANT_PASTE).ToString();
        costs[0].text = bp.costPaste.ToString();

        resources[1].text = pc.CheckResource(Resource.ResourceTypes.TOUGH_CLOTH).ToString();
        costs[1].text = bp.costCloth.ToString();

        resources[2].text = pc.CheckResource(Resource.ResourceTypes.SCRAP_METAL).ToString();
        costs[2].text = bp.costMetal.ToString();

        resources[3].text = pc.CheckResource(Resource.ResourceTypes.TECH_PARTS).ToString();
        costs[3].text = bp.costTech.ToString();

        resources[4].text = pc.CheckResource(Resource.ResourceTypes.GAS_CAN).ToString();
        costs[4].text = bp.costGas.ToString();

        resources[5].text = pc.CheckResource(Resource.ResourceTypes.WEED_SPRAY).ToString();
        costs[5].text = bp.costSpray.ToString();
    }
}
