using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameManager gameManager;
    public Transform player;

    public int health = 10;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        gameManager.enemyCount++;
    }

    void Update()
    {
        if (health > 0)
        {
            Chase();
        }
        else
        {
            Destroy(gameObject);
        }
        print(health);
    }

    private void Chase()
    {
        agent.SetDestination(player.position);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bullet"))
        {
            Bullet bullet = other.GetComponent<Bullet>();

            if (bullet != null)
            {
                health -= bullet.damage; 
                Destroy(other.gameObject);
            }
        }
        
    }

    private void OnDestroy()
    {
        gameManager.killedEnemies++;
        gameManager.enemyCount--;
    }
}
