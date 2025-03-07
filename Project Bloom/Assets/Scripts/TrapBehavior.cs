using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class TrapBehavior : MonoBehaviour
{
    [Header("Trap Attributes")]
    [SerializeField] float damage;                  // Damage dealt
    [SerializeField] float triggerRate;             // Damage interval (If <=0, trap is single use)
    [SerializeField] float uses;                    // Total use time is triggerRate * uses

    [Header("Remote Activation")]
    [SerializeField] bool isActivatable;            // Whether a remote activator can be applied
    [SerializeField] bool hasActivator;             // Whether a remote activator is applied
    public int remoteChannel;                       // The channel that triggers the building when fired

    // Runtime Values
    private BuildingBehavior buildingBehavior;
    private List<GameObject> targets;
    private bool active;
    private float cooldown = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        buildingBehavior = GetComponent<BuildingBehavior>();
        targets = new List<GameObject>();
        active = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (buildingBehavior.durability > 0 && other.gameObject.CompareTag("Enemy"))
        {
            targets.Add(other.gameObject);

            if (uses > 0)
            {
                if (!active) // Damage all targets regularly
                {
                    active = true;
                    InvokeRepeating("InflictDamage", 0.0f, triggerRate);
                }
            }
            else
            {
                Debug.Log("Out of uses! Re-arm!");
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        targets.Remove(other.gameObject);
        if (targets.Count == 0)
        {
            active = false;
        }
    }

    private void ApplyRemoteActivator(int channel)
    {
        hasActivator = true;
        remoteChannel = channel;
    }

    private void InflictDamage()
    {
        if (targets.Count > 0)
        {
            foreach (GameObject target in targets)
            {
                // Reduce HP here
                Debug.Log("Damaged " + target.name + " for " + damage + " damage!");
            }
            uses--;
        }
        else
        {
            CancelInvoke();
        }
    }
}
