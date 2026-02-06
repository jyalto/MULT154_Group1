using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private SoundManager soundManager;

    public GameObject enemy;
    public GameObject bigEnemy;
    public GameObject barricade;
    public int enemyCount = 0;
    public int bigEnemyCount = 0;
    public int wave = 1;
    public int killedEnemies = 0;
    public bool shotgunDrop = false;
    public bool rpgDrop = false;
    public bool flamethrowerDrop = false;

    private bool delay = true;

    private int randomBigSpawn = 0;

    public List<GameObject> spawnedItems = new List<GameObject>();
    public Transform[] spawnPoints;
    public Transform[] bigspawnPoints;

    public TMP_Text waveText;
    public TMP_Text ammoText;
    public Image bulletImage;
    public Image skullImage;

    public GameObject alertRadio;
    public AudioClip wave1;
    public AudioClip wave2;
    public AudioClip wave3;
    public AudioClip wave4;


    private Coroutine myCoroutine = null;

    void Start()
    {
        GameObject soundManagerObject = GameObject.Find("Sound Manager");

        soundManager = soundManagerObject.GetComponent<SoundManager>();

        alertRadio.GetComponent<AudioSource>().PlayOneShot(wave1, 4);

        enemyCount = 0;
        bigEnemyCount = 0;
        wave = 1;
        killedEnemies = 0;
        shotgunDrop = false;
        rpgDrop = false;
        delay = true;
        flamethrowerDrop = false;
        randomBigSpawn = 0;
        myCoroutine = null;

        foreach (GameObject item in spawnedItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        spawnedItems.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit(); 
        }

        if (!delay)
        {
            barricade.SetActive(false);
            if (wave == 1)
            {
                if (enemyCount < 10 && killedEnemies < 35 && myCoroutine == null)
                {
                    myCoroutine = StartCoroutine(WaveOneSpawnEnemy());

                }
                if (killedEnemies == 35)
                {
                    wave = 2;
                    alertRadio.GetComponent<AudioSource>().PlayOneShot(wave2, 4);
                }
            }
            else if (wave == 2)
            {
                if (enemyCount < 20 && killedEnemies < 75 && myCoroutine == null)
                {
                    myCoroutine = StartCoroutine(WaveTwoSpawnEnemy());
                }
                if (killedEnemies == 75)
                {
                    wave = 3;
                    alertRadio.GetComponent<AudioSource>().PlayOneShot(wave3, 4);
                }
            }
            else if (wave == 3)
            {
                if (enemyCount < 30 && killedEnemies < 150 && myCoroutine == null)
                {
                    myCoroutine = StartCoroutine(WaveThreeSpawnEnemy());
                }
                if (killedEnemies == 150)
                {
                    wave = 4;
                    alertRadio.GetComponent<AudioSource>().PlayOneShot(wave4, 4);
                }
            }
            else if (wave == 4)
            {
                soundManager.Wave4Start();
                if (myCoroutine == null)
                {
                    myCoroutine = StartCoroutine(WaveFourSpawnEnemy());
                }
                /*if (killedEnemies == 200)
                {
                    wave = 5;
                }*/
            }
        }

        else
        {
            barricade.SetActive(true);
            if (myCoroutine == null)
            {
                myCoroutine = StartCoroutine(StartDelay());
            }
        }
        

        if (wave != 4)
        {
            waveText.SetText("Wave: " + wave);
            skullImage.gameObject.SetActive(false);
        }
        else
        {
            soundManager.Disturbed();
            waveText.SetText("Wave: ");
            skullImage.gameObject.SetActive(true);
        }
    }

    private IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(60f);
        delay = false;
        myCoroutine = null;
    }

    private IEnumerator WaveOneSpawnEnemy()
    {
        while (enemyCount < 10)
        {
            if (bigEnemyCount == 0)
            {
                randomBigSpawn = Random.Range(0, 15);

                if (randomBigSpawn == 1)
                {
                    int randomPoint = Random.Range(0, bigspawnPoints.Length);
                    Instantiate(bigEnemy, bigspawnPoints[randomPoint].position, Quaternion.identity);
                }

                else
                {
                    int randomPoint = Random.Range(0, spawnPoints.Length);
                    Instantiate(enemy, spawnPoints[randomPoint].position, Quaternion.identity);
                }

            }

            else
            {
                int randomPoint = Random.Range(0, spawnPoints.Length);
                Instantiate(enemy, spawnPoints[randomPoint].position, Quaternion.identity);
            }

            yield return new WaitForSeconds(3f);
        }

        myCoroutine = null;
    }

    private IEnumerator WaveTwoSpawnEnemy()
    {
        while (enemyCount < 20)
        {
            if (bigEnemyCount == 0)
            {
                randomBigSpawn = Random.Range(0, 10);

                if (randomBigSpawn == 1)
                {
                    int randomPoint = Random.Range(0, bigspawnPoints.Length);
                    Instantiate(bigEnemy, bigspawnPoints[randomPoint].position, Quaternion.identity);
                }

                else
                {
                    int randomPoint = Random.Range(0, spawnPoints.Length);
                    Instantiate(enemy, spawnPoints[randomPoint].position, Quaternion.identity);
                }

            }

            else
            {
                int randomPoint = Random.Range(0, spawnPoints.Length);
                Instantiate(enemy, spawnPoints[randomPoint].position, Quaternion.identity);
            }

            yield return new WaitForSeconds(2.75f);
        }

        myCoroutine = null;
    }

    private IEnumerator WaveThreeSpawnEnemy()
    {
        while (enemyCount < 30)
        {
            if (bigEnemyCount == 0)
            {
                randomBigSpawn = Random.Range(0, 5);

                if (randomBigSpawn == 1)
                {
                    int randomPoint = Random.Range(0, bigspawnPoints.Length);
                    Instantiate(bigEnemy, bigspawnPoints[randomPoint].position, Quaternion.identity);
                }

                else
                {
                    int randomPoint = Random.Range(0, spawnPoints.Length);
                    Instantiate(enemy, spawnPoints[randomPoint].position, Quaternion.identity);
                }

            }

            else
            {
                int randomPoint = Random.Range(0, spawnPoints.Length);
                Instantiate(enemy, spawnPoints[randomPoint].position, Quaternion.identity);
            }

            yield return new WaitForSeconds(2.5f);
        }

        myCoroutine = null;
    }

    private IEnumerator WaveFourSpawnEnemy()
    {
        while (enemyCount < 200)
        {
            if (bigEnemyCount < 15)
            {
                randomBigSpawn = Random.Range(0, 5);

                if (randomBigSpawn == 1)
                {
                    int randomPoint = Random.Range(0, bigspawnPoints.Length);
                    Instantiate(bigEnemy, bigspawnPoints[randomPoint].position, Quaternion.identity);
                }

                else
                {
                    int randomPoint = Random.Range(0, spawnPoints.Length);
                    Instantiate(enemy, spawnPoints[randomPoint].position, Quaternion.identity);
                }

            }

            else
            {
                int randomPoint = Random.Range(0, spawnPoints.Length);
                Instantiate(enemy, spawnPoints[randomPoint].position, Quaternion.identity);
            }

            yield return new WaitForSeconds(0.5f);
        }

        myCoroutine = null;
    }

    public IEnumerator ReloadScene()
    {
        soundManager.DeathCry();
        yield return new WaitForSeconds(2f);
        Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene("MainMenu");
    }

}
