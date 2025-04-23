using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public class OffhandUtilities : MonoBehaviour
{
    public bool equipmentModeEnabled = false;
    private PlayerController controller;
    [SerializeField] GameObject mainHand;
    private BuildingManager buildingManager;

    public GameObject offhandModel;
    public GameObject[] equipmentModels;

    public enum Equipment
    { NONE = 0, HAMMER = 1, REMOTE = 2, ACTIVATOR = 3, SYRINGE = 4 } // Correspond to model indexes
    private List<Equipment> equipment;
    public int currentEquipmentIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
        buildingManager = GetComponent<BuildingManager>();

        equipment = new List<Equipment>();

        equipment.Add(Equipment.NONE);
        equipment.Add(Equipment.HAMMER);
        equipment.Add(Equipment.REMOTE);
        equipment.Add(Equipment.ACTIVATOR);
        equipment.Add(Equipment.SYRINGE);

        equipmentModels[currentEquipmentIndex].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) // Switch to equipment mode
        {
            equipmentModeEnabled = !equipmentModeEnabled;

            mainHand.SetActive(!equipmentModeEnabled);
            offhandModel.SetActive(equipmentModeEnabled);

            if (!equipmentModeEnabled)
            {
                DisableEquipmentAbilities();
            }

        }
        if (equipmentModeEnabled)
        {
            equipmentModels[currentEquipmentIndex].SetActive(true);

            // Equipment mode input
            float scrollInput = Input.mouseScrollDelta.y;

            if (scrollInput != 0)
            {
                if (scrollInput < 0)
                {
                    scrollInput = equipment.Count - 1;
                }
                currentEquipmentIndex = (currentEquipmentIndex + (int)scrollInput) % equipment.Count;
                DisableEquipmentAbilities();

                switch (equipment[currentEquipmentIndex])
                {
                    case Equipment.NONE:
                        equipmentModels[0].SetActive(true);
                        break;
                    case Equipment.HAMMER:
                        equipmentModels[1].SetActive(true);
                        buildingManager.buildingModeActive = true;
                        break;
                    case Equipment.REMOTE:
                        buildingManager.remoteEquipped = true;
                        equipmentModels[2].SetActive(true);
                        break;
                    case Equipment.ACTIVATOR:
                        buildingManager.activatorEquipped = true;
                        equipmentModels[3].SetActive(true);
                        break;
                    case Equipment.SYRINGE:
                        equipmentModels[4].SetActive(true);
                        break;

                }

                print(equipment[currentEquipmentIndex].ToString());

                equipmentModels[currentEquipmentIndex].SetActive(true);
            }
        }
    }

    public void DisableEquipmentAbilities()
    {
        buildingManager.buildingModeActive = false;
        buildingManager.remoteEquipped = false;
        buildingManager.activatorEquipped = false;

        foreach (GameObject model in equipmentModels)
        {
            model.SetActive(false);
        }
    }
}