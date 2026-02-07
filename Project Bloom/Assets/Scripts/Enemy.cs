using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent agent;
    private GameManager gameManager;
    private GameObject player;
    private PlayerController playerController;
    private BuildingManager buildingManager;
    public Transform target;
    public AudioSource[] audioSources;
    public GameObject model;

    public GameObject[] randomDrop;
    public GameObject[] rareDrop;
    public GameObject shotgunItem;
    public GameObject rpgItem;
    public GameObject flamethrowerItem;

    public bool bigEnemyActive = false;
    public bool attacking = false;

    public float health = 10;
    public float playerLockOnRange = 10;

    private bool shotgunAdded = false;
    private bool rpgAdded = false;
    private bool flamethrowerAdded = false;
    private bool takingFireDamage = false;
    private bool playerTakingDamage = false;

    private float updateRate = 0.2f;
    private float nextUpdate = 0f;

    private Coroutine fireDamageCoroutine = null;
    private Coroutine playerDamageCoroutine = null;

    private List<GameObject> rareDropList;

    public GameObject bloodEffect;

    EnemySound enemySound;
    DamagedSound damagedSound;

    void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        buildingManager = player.GetComponent<BuildingManager>();
        //audioSources = GetComponents<AudioSource>();

        enemySound = GetComponent<EnemySound>();

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

    void Start()
    {
        if (!bigEnemyActive)
        {
            if (gameManager.wave == 2)
            {
                agent.speed = 14;
                //health = 12;
            }
            else if (gameManager.wave == 3)
            {
                agent.speed = 16;
                //health = 14;
            }
            else if (gameManager.wave == 4)
            {
                agent.speed = 18;
                //health = 16;
            }
        }
        else
        {
            if (gameManager.wave == 2)
            {
                agent.speed = 10;
            }
            else if (gameManager.wave == 3)
            {
                agent.speed = 12;
            }
            else if (gameManager.wave == 4)
            {
                agent.speed = 14;
            }
        }
    }

    void Update()
    {
        if (health > 0)
        {
            if (!attacking)
            {
                agent.isStopped = false;
                model.GetComponent<Animator>().SetBool("Attack", false);
                model.GetComponent<Animator>().SetBool("Walk", true);
                if (Time.time >= nextUpdate)
                {
                    UpdateChase();
                    nextUpdate = Time.time + updateRate;
                }
            }
            else
            {
                agent.isStopped = true;
                model.GetComponent<Animator>().SetBool("Attack", true);
                model.GetComponent<Animator>().SetBool("Walk", false);
            }
        }
        else
        {
            enemySound.PlayDeathSound();
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

        if (playerController.flameActive)
        {
            if (fireDamageCoroutine == null && takingFireDamage)
            {
                fireDamageCoroutine = StartCoroutine(FireDamage());
            }
        }
        else
        {
            takingFireDamage = false;
            if (fireDamageCoroutine != null)
            {
                StopCoroutine(fireDamageCoroutine);
                fireDamageCoroutine = null;
            }
        }

        if (playerDamageCoroutine == null && playerTakingDamage)
        {
            playerDamageCoroutine = StartCoroutine(DamagePlayer());
        }

        // UpdateAnims();
    }

    private void UpdateAnims()
    {
        /*
        if (gameObject.GetComponent<Rigidbody>().velocity.magnitude > 1f)
        {
            model.GetComponent<Animator>().SetBool("walking", true);
        }
        */
    }

    private void UpdateChase()
    {
        target = player.transform; // Default to player transform

        if (!attacking)
        {
            foreach (GameObject building in buildingManager.placedBuildings)
            {
                if (building.GetComponent<LureDevice>() != null) // Lure targets
                {
                    Vector3 currentPosition = gameObject.transform.position;
                    float newTargetDistance = Vector3.Distance(currentPosition, building.transform.position);
                    float currentTargetDistance = Vector3.Distance(currentPosition, target.position);

                    // print("Lure device found at a distance of " + newTargetDistance + "! Current target distance is " + currentTargetDistance + ".");

                    /*if (newTargetDistance < currentTargetDistance && Vector3.Distance(currentPosition, player.transform.position) > playerLockOnRange && building.GetComponent<TrapBehavior>().luring)
                    {
                        //print("New target acquired!");
                        target = building.transform;
                    }*/

                    if (building.GetComponent<TrapBehavior>().luring)
                    {
                        target = building.transform;
                    }
                }
            }

            agent.SetDestination(target.position);
        }
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

                Vector3 hitPos = other.ClosestPoint(transform.position);

                hitPos -= other.transform.forward * 4f;

                Instantiate(
                    bloodEffect,
                    hitPos,
                    Quaternion.identity
                );
            }
        }

        if (other.CompareTag("Explosion"))
        {
            if (bigEnemyActive)
            {
                health -= 25;
                if (health > 0)
                {
                    Vector3 hitPos = other.ClosestPoint(transform.position);

                    hitPos -= other.transform.forward * 4f;

                    Instantiate(
                        bloodEffect,
                        hitPos,
                        Quaternion.identity
                    );
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }

        if (other.CompareTag("Flamethrower") && takingFireDamage == false && playerController.flameActive)
        {
            fireDamageCoroutine = StartCoroutine(FireDamage());
            takingFireDamage = true;
        }

        if (other.CompareTag("Player") && playerTakingDamage == false)
        {
            damagedSound = other.GetComponentInChildren<DamagedSound>();
            attacking = true;
            playerDamageCoroutine = StartCoroutine(DamagePlayer());
            playerTakingDamage = true;
        }

        AnimatorStateInfo stateInfo = playerController.playerAnim.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("Attack") && other.CompareTag("Bat"))
        {
            health -= 3.5f;
            Vector3 hitPos = other.ClosestPoint(transform.position);

            hitPos -= other.transform.forward * -0.5f;

            Instantiate(
                bloodEffect,
                hitPos,
                Quaternion.identity
            );
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

        if (other.CompareTag("Player"))
        {
            attacking = false;
            playerTakingDamage = false;
            if (playerDamageCoroutine != null)
            {
                StopCoroutine(playerDamageCoroutine);
                playerDamageCoroutine = null;
            }
        }
    }

    private IEnumerator DamagePlayer()
    {
        yield return new WaitForSeconds(1.85f);
        if (bigEnemyActive)
        {
            playerController.health -= 5;
            if (playerController.health > 0)
            {
                damagedSound.PlayRandomSound();
            }
        }
        else
        {
            if (gameManager.wave == 1)
            {
                playerController.health -= 2;
                if (playerController.health > 0)
                {
                    damagedSound.PlayRandomSound();
                }
            }
            if (gameManager.wave == 2)
            {
                playerController.health -= 3;
                if (playerController.health > 0)
                {
                    damagedSound.PlayRandomSound();
                }
            }
            if (gameManager.wave == 3 || gameManager.wave == 4)
            {
                playerController.health -= 4;
                if (playerController.health > 0)
                {
                    damagedSound.PlayRandomSound();
                }
            }
        }
        yield return new WaitForSeconds(1f);
        playerDamageCoroutine = null;
    }

    private IEnumerator FireDamage()
    {
        health -= 1.35f;
        yield return new WaitForSeconds(0.2f);  
        fireDamageCoroutine = null;
    }

    private void OnDestroy()
    {
        if (playerController != null)
        {
            if (!bigEnemyActive)
            {
                int randomRate = Random.Range(0, 3);
                if (randomRate == 2)
                {
                    int randomNum = Random.Range(0, randomDrop.Length);
                    if (randomNum == 0)
                    {
                        //gameManager.spawnedItems.Add(Instantiate(randomDrop[randomNum], new Vector3(transform.position.x, 10.17471f, transform.position.z), Quaternion.Euler(90f, 0f, -90f)));
                        gameManager.spawnedItems.Add(Instantiate(randomDrop[randomNum], new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), randomDrop[randomNum].transform.rotation));
                    }
                    else if (randomNum == 1)
                    {
                        //gameManager.spawnedItems.Add(Instantiate(randomDrop[randomNum], new Vector3(transform.position.x, 9.8f, transform.position.z), Quaternion.Euler(-90f, 0f, 0f)));
                        gameManager.spawnedItems.Add(Instantiate(randomDrop[randomNum], new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), randomDrop[randomNum].transform.rotation));
                    }
                    else if (randomNum == 2 || randomNum == 3)
                    {
                        int randomChance = Random.Range(0, 2);

                        if (randomChance == 1 && rareDrop.Length > 0)
                        {
                            int randomNum2 = Random.Range(0, rareDrop.Length);
                            //gameManager.spawnedItems.Add(Instantiate(rareDrop[randomNum2], new Vector3(transform.position.x, 10.17471f, transform.position.z), Quaternion.identity));
                            gameManager.spawnedItems.Add(Instantiate(rareDrop[randomNum2], new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), rareDrop[randomNum2].transform.rotation));
                        }

                        else
                        {
                            //gameManager.spawnedItems.Add(Instantiate(randomDrop[randomNum], new Vector3(transform.position.x, 10.17471f, transform.position.z), Quaternion.identity));
                            gameManager.spawnedItems.Add(Instantiate(randomDrop[randomNum], new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), randomDrop[randomNum].transform.rotation));
                        }
                    }
                    else
                    {
                        //gameManager.spawnedItems.Add(Instantiate(randomDrop[randomNum], new Vector3(transform.position.x, 10.17471f, transform.position.z), Quaternion.Euler(90f, 0f, -90f)));
                        gameManager.spawnedItems.Add(Instantiate(randomDrop[randomNum], new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), randomDrop[randomNum].transform.rotation));
                    }
                }
            }

            else
            {
                if (rareDrop.Length > 0)
                {
                    int randomNum2 = Random.Range(0, rareDrop.Length);
                    //gameManager.spawnedItems.Add(Instantiate(rareDrop[randomNum2], new Vector3(transform.position.x, 10.17471f, transform.position.z), Quaternion.identity));
                    gameManager.spawnedItems.Add(Instantiate(rareDrop[randomNum2], new Vector3(transform.position.x, transform.position.y - 3f, transform.position.z), rareDrop[randomNum2].transform.rotation));
                }

                else
                {
                    int randomNum = Random.Range(2, randomDrop.Length);
                    //gameManager.spawnedItems.Add(Instantiate(randomDrop[randomNum], new Vector3(transform.position.x, 10.17471f, transform.position.z), Quaternion.identity));
                    gameManager.spawnedItems.Add(Instantiate(randomDrop[randomNum], new Vector3(transform.position.x, transform.position.y - 3f, transform.position.z), randomDrop[randomNum].transform.rotation));
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
}