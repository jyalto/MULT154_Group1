using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameManager gameManager;
    public PlayerController playerObject;
    public Transform player;

    public GameObject[] randomDrop;
    public GameObject[] rareDrop;
    public GameObject shotgunItem;
    public GameObject rpgItem;
    public GameObject flamethrowerItem;

    public bool bigEnemyActive = false;

    public float health = 10;

    private bool shotgunAdded = false;
    private bool rpgAdded = false;
    private bool flamethrowerAdded = false;
    private bool takingFireDamage = false;

    private float updateRate = 0.2f;
    private float nextUpdate = 0f;

    private Coroutine fireDamageCoroutine = null;

    private List<GameObject> rareDropList;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        playerObject = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        gameManager.enemyCount++;

        rareDropList = new List<GameObject>(rareDrop);

        if (gameManager.shotgunDrop)
        {
            rareDropList.Add(shotgunItem);
            shotgunAdded = true;
        }
        if (gameManager.rpgDrop)
        {
            rareDropList.Add(rpgItem);
            rpgAdded = true;
        }

        rareDrop = rareDropList.ToArray();

        if (transform.localScale.x > 4)
        {
            health = 50;
            bigEnemyActive = true;
            gameManager.bigEnemyCount++;
        }
    }

    void Update()
    {
        if (health > 0)
        {
            if (Time.time >= nextUpdate)
            {
                Chase();
                nextUpdate = Time.time + updateRate;
            }
        }
        else
        {
            Destroy(gameObject);
        }
        //print(health);

        if (gameManager.shotgunDrop && !shotgunAdded)
        {
            rareDropList.Add(shotgunItem);
            rareDrop = rareDropList.ToArray();
            shotgunAdded = true;
        }

        if (gameManager.rpgDrop && !rpgAdded)
        {
            rareDropList.Add(rpgItem);
            rareDrop = rareDropList.ToArray();
            rpgAdded = true;
        }

        if (gameManager.flamethrowerDrop && !flamethrowerAdded)
        {
            rareDropList.Add(flamethrowerItem);
            rareDrop = rareDropList.ToArray();
            flamethrowerAdded = true;
        }
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

        if (other.CompareTag("Explosion"))
        {
            if (bigEnemyActive)
            {
                health -= 25;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        if (other.CompareTag("Flamethrower") && takingFireDamage == false && playerObject.flameActive)
        {
            fireDamageCoroutine = StartCoroutine(FireDamage());
            takingFireDamage = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Flamethrower"))
        {
            takingFireDamage = false;
            if (fireDamageCoroutine != null)
            {
                StopCoroutine(fireDamageCoroutine);
                fireDamageCoroutine = null;
            }
        }
    }

    private IEnumerator FireDamage()
    {
        while (takingFireDamage)
        {
            health -= 1;
            yield return new WaitForSeconds(0.2f);  
        }
        fireDamageCoroutine = null;
    }

    private void OnDestroy()
    {
        if (!bigEnemyActive)
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
                    int randomChance = Random.Range(0, 2);

                    if (randomChance == 1 && rareDrop.Length > 0)
                    {
                        int randomNum2 = Random.Range(0, rareDrop.Length);
                        Instantiate(rareDrop[randomNum2], new Vector3(transform.position.x, 10.17471f, transform.position.z), Quaternion.identity);
                    }

                    else
                    {
                        Instantiate(randomDrop[randomNum], new Vector3(transform.position.x, 10.17471f, transform.position.z), Quaternion.identity);
                    }
                }
            }
        }

        else
        {
            if (rareDrop.Length > 0)
            {
                int randomNum2 = Random.Range(0, rareDrop.Length);
                Instantiate(rareDrop[randomNum2], new Vector3(transform.position.x, 10.17471f, transform.position.z), Quaternion.identity);
            }

            else
            {
                int randomNum = Random.Range(2, randomDrop.Length);
                Instantiate(randomDrop[randomNum], new Vector3(transform.position.x, 10.17471f, transform.position.z), Quaternion.identity);
            }
        }
        
        gameManager.killedEnemies++;

        if (bigEnemyActive)
        {
            gameManager.bigEnemyCount--;
            gameManager.enemyCount--;
        }
        else
        {
            gameManager.enemyCount--;
        }
    }
}
