using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingPreview : MonoBehaviour
{
    public GameObject buildingObject;
    public GameObject mesh;
    public Material red;
    public Material green;

    public bool canPlace = true;
    private Collider placmentCollider;

    // Start is called before the first frame update
    void Start()
    {
        mesh.GetComponent<MeshRenderer>().material = green;
    }

    private void OnTriggerEnter(Collider other)
    {
        mesh.GetComponent<MeshRenderer>().material = red;
        canPlace = false;
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log(other);
        mesh.GetComponent<MeshRenderer>().material = red;
        canPlace = false;
    }

    private void OnTriggerExit(Collider other)
    {
        mesh.GetComponent<MeshRenderer>().material = green;
        canPlace = true;
    }
}
