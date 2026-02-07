using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffhandUtilities : MonoBehaviour
{
    public bool equipmentModeEnabled = false;
    private PlayerController controller;
    private Animator playerAnim;
    [SerializeField] GameObject mainHand;
    [SerializeField] GameObject handMesh;
    private BuildingManager buildingManager;

    public GameObject offhandModel;
    public GameObject[] equipmentModels;

    public enum Equipment
    {
        NONE = 0,
        HAMMER = 1,
        // REMOTE = 2,
        // ACTIVATOR = 3,
        SYRINGE = 2,
    }

    private List<Equipment> equipment;
    public int currentEquipmentIndex = 0;

    void Start()
    {
        controller = GetComponent<PlayerController>();
        playerAnim = GetComponent<Animator>();
        buildingManager = GetComponent<BuildingManager>();

        equipment = new List<Equipment>
        {
            Equipment.NONE,
            Equipment.HAMMER,
            // Equipment.REMOTE,
            // Equipment.ACTIVATOR,
            Equipment.SYRINGE
        };

        equipmentModels[currentEquipmentIndex].SetActive(true);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.JoystickButton4))
        {
            equipmentModeEnabled = !equipmentModeEnabled;

            mainHand.SetActive(!equipmentModeEnabled);
            offhandModel.SetActive(equipmentModeEnabled);

            playerAnim.SetTrigger("unequipWeapon");
            playerAnim.SetInteger("weaponType", 0);

            if (!equipmentModeEnabled)
            {
                DisableEquipmentAbilities();

                // NEW: reset back to NONE when exiting equipment mode
                currentEquipmentIndex = 0;

                // NEW: make sure syringe logic is fully off when leaving
                controller.usingSyringe = false;
            }
        }


        if (equipmentModeEnabled)
        {
            equipmentModels[currentEquipmentIndex].SetActive(true);

            float scrollInput = -Input.mouseScrollDelta.y;

            if (Input.GetKeyDown(KeyCode.JoystickButton3))
                scrollInput = 1;

            if (scrollInput != 0)
            {
                if (scrollInput < 0)
                    scrollInput = equipment.Count - 1;

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

                    /*
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
                    */

                    case Equipment.SYRINGE:
                        controller.usingSyringe = true;

                        // FIX: Syringe should use the correct model index
                        // Previously: equipmentModels[4]
                        // But since REMOTE and ACTIVATOR are commented out,
                        // Syringe is now the 3rd tool in the list.
                        equipmentModels[2].SetActive(true);

                        playerAnim.SetInteger("weaponType", 8);
                        break;
                }

                equipmentModels[currentEquipmentIndex].SetActive(true);
            }
        }
    }

    /*public void DisableEquipmentAbilities()
    {
        buildingManager.buildingModeActive = false;
        // buildingManager.remoteEquipped = false;
        // buildingManager.activatorEquipped = false;
        controller.usingSyringe = false;

        foreach (GameObject model in equipmentModels)
            model.SetActive(false);
    }*/

    public void DisableEquipmentAbilities()
    {
        buildingManager.buildingModeActive = false;

        // Only disable syringe if we are NOT currently selecting it
        if (equipment[currentEquipmentIndex] != Equipment.SYRINGE)
            controller.usingSyringe = false;

        foreach (GameObject model in equipmentModels)
            model.SetActive(false);
    }

}
