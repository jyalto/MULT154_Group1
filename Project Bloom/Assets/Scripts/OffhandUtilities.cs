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
    { NONE = 0, HAMMER = 1, REMOTE = 2, SYRINGE = 3 } // Correspond to model indexes
    private List<Equipment> equipment;
    public int currentEquipmentIndex;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<PlayerController>();
        buildingManager = GetComponent<BuildingManager>();

        equipment = new List<Equipment>();
        equipment.Add(Equipment.NONE);
        equipment.Add(Equipment.HAMMER);
        equipment.Add(Equipment.REMOTE);
        equipment.Add(Equipment.SYRINGE);
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
                currentEquipmentIndex = 0;
            }
        }
        if (equipmentModeEnabled)
        {
            // Equipment mode input
            float scrollInput = Input.mouseScrollDelta.y;

            if (scrollInput != 0)
            {
                currentEquipmentIndex += (int)scrollInput;

                DisableEquipmentAbilities();

                switch (equipment[currentEquipmentIndex])
                {
                    case Equipment.NONE:
                        break;
                    case Equipment.HAMMER:
                        buildingManager.buildingModeActive = true;
                        break;
                    case Equipment.REMOTE:
                        break;
                    case Equipment.SYRINGE:
                        break;
                }
            }
        }
    }

    public void DisableEquipmentAbilities()
    {
        buildingManager.buildingModeActive = false;
    }
}