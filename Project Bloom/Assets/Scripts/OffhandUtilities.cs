using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class OffhandUtilities : MonoBehaviour
{
    public bool equipmentModeEnabled = false;
    private PlayerController controller;
    private Animator playerAnim;
    [SerializeField] GameObject mainHand;
    [SerializeField] GameObject handMesh; // ALL INVOLVED CODE IS QUICK FIX FOR LACK OF HAND ANIMATIONS, REMOVE AFTER IMPLEMENTATION
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
        playerAnim = GetComponent<Animator>();  
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
            playerAnim.SetTrigger("unequipWeapon");

            if (!equipmentModeEnabled)
            {
                DisableEquipmentAbilities();
            }

        }
        if (equipmentModeEnabled)
        {
            //handMesh.SetActive(false);
            equipmentModels[currentEquipmentIndex].SetActive(true);

            // Equipment mode input
            float scrollInput = -Input.mouseScrollDelta.y;

            // Mode switching
            if (scrollInput != 0)
            {
                if (scrollInput < 0)
                {
                    scrollInput = equipment.Count - 1;
                }
                currentEquipmentIndex = (currentEquipmentIndex + (int)scrollInput) % equipment.Count;
                DisableEquipmentAbilities();
                playerAnim.SetTrigger("unequipWeapon");

                switch (equipment[currentEquipmentIndex])
                {
                    case Equipment.NONE:
                        equipmentModels[0].SetActive(true);
                        playerAnim.SetInteger("weaponType", 0);
                        break;
                    case Equipment.HAMMER:
                        equipmentModels[1].SetActive(true);
                        playerAnim.SetInteger("weaponType", 1);
                        buildingManager.buildingModeActive = true;
                        break;
                    case Equipment.REMOTE:
                        buildingManager.remoteEquipped = true;
                        equipmentModels[2].SetActive(true);
                        playerAnim.SetInteger("weaponType", 9);
                        break;
                    case Equipment.ACTIVATOR:
                        buildingManager.activatorEquipped = true;
                        equipmentModels[3].SetActive(true);
                        playerAnim.SetInteger("weaponType", 10);
                        break;
                    case Equipment.SYRINGE:
                        controller.usingSyringe = true;
                        equipmentModels[4].SetActive(true);
                        playerAnim.SetInteger("weaponType", 8);
                        break;

                }

                // print(equipment[currentEquipmentIndex].ToString());

                equipmentModels[currentEquipmentIndex].SetActive(true);
            }
        }
        else
        {
            //handMesh.SetActive(true);
        }
    }

    public void DisableEquipmentAbilities()
    {
        buildingManager.buildingModeActive = false;
        buildingManager.remoteEquipped = false;
        buildingManager.activatorEquipped = false;
        controller.usingSyringe = false;
        playerAnim.SetTrigger("unequipWeapon");

        foreach (GameObject model in equipmentModels)
        {
            model.SetActive(false);
        }
    }
}