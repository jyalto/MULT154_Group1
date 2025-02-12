using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehavior : MonoBehaviour
{
    public Transform PatrolRoute;
    public List<Transform> Locations;
    private int locationIndex;

    private NavMeshAgent agent;

    public Transform player;


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        player = GameObject.Find("Player").transform;

        InitializePatrolRoute();
        MoveToNextPatrolLocation();
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.remainingDistance < 0.2f && !agent.pathPending)
            MoveToNextPatrolLocation();
    }
    void InitializePatrolRoute()
    {
        foreach (Transform child in PatrolRoute)
            Locations.Add(child);
    }
    void MoveToNextPatrolLocation()
    {
        // We wouldn't want to use this of course because we want mobs to approach/move in a stright line then fight
        if (Locations.Count == 0)
            return;

        agent.destination = Locations[locationIndex].position;
        locationIndex = (locationIndex + 1) % Locations.Count;

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            agent.destination = player.position;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        
    }
}
