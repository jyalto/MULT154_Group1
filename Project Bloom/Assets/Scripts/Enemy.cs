using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameManager gameManager;
    public Transform player;

    public GameObject[] randomDrop;

    public float health = 10;

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
        //print(health);
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
        int randomRate = Random.Range(0, 3);
        if (randomRate == 2)
        {
            int randomNum = Random.Range(0, randomDrop.Length);
            if (randomNum == 0)
            {
                Instantiate(randomDrop[randomNum], new Vector3(transform.position.x, 10.17471f, transform.position.z), Quaternion.Euler(90f, 0f, -90f));
            }
            else if (randomNum == 1)
            {
                Instantiate(randomDrop[randomNum], new Vector3(transform.position.x, 9.8f, transform.position.z), Quaternion.Euler(-90f, 0f, 0f));
            }
            else if (randomNum == 2 || randomNum == 3)
            {
                Instantiate(randomDrop[randomNum], new Vector3(transform.position.x, 10.17471f, transform.position.z), Quaternion.identity);
            }
        }
        gameManager.killedEnemies++;
        gameManager.enemyCount--;
    }
}
