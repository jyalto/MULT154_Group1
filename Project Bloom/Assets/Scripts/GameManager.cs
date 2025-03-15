using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject enemy;
    public int enemyCount = 0;
    public int wave = 1;
    public int killedEnemies = 0;
    public bool shotgunDrop = false;
    public bool rpgDrop = false;

    public Transform[] spawnPoints;

    private Coroutine myCoroutine = null;

    // Update is called once per frame
    void Update()
    {
        if (wave == 1)
        {
            if (enemyCount < 5 && killedEnemies < 15 && myCoroutine == null)
            {
                myCoroutine = StartCoroutine(WaveOneSpawnEnemy());
            }
            if (killedEnemies == 15)
            {
                wave = 2;
            }
        }
        else if (wave == 2)
        {
            if (enemyCount < 7 && killedEnemies < 35 && myCoroutine == null)
            {
                myCoroutine = StartCoroutine(WaveTwoSpawnEnemy());
            }
            if (killedEnemies == 35)
            {
                wave = 3;
            }
        }
        else if (wave == 3)
        {
            if (enemyCount < 10 && killedEnemies < 75 && myCoroutine == null)
            {
                myCoroutine = StartCoroutine(WaveThreeSpawnEnemy());
            }
            if (killedEnemies == 75)
            {
                wave = 4;
            }
        }
        else if (wave == 4)
        {
            if (enemyCount < 15 && killedEnemies < 200 && myCoroutine == null)
            {
                myCoroutine = StartCoroutine(WaveFourSpawnEnemy());
            }
            if (killedEnemies == 200)
            {
                wave = 5;
            }
        }
        //print("Current wave: " + wave);
    }

    private IEnumerator WaveOneSpawnEnemy()
    {
        while (enemyCount < 5)
        {
            int randomPoint = Random.Range(0, spawnPoints.Length);
            Instantiate(enemy, spawnPoints[randomPoint].position, Quaternion.identity);
            yield return new WaitForSeconds(3f);
        }

        myCoroutine = null;
    }

    private IEnumerator WaveTwoSpawnEnemy()
    {
        while (enemyCount < 7)
        {
            int randomPoint = Random.Range(0, spawnPoints.Length);
            Instantiate(enemy, spawnPoints[randomPoint].position, Quaternion.identity);
            yield return new WaitForSeconds(2.75f);
        }

        myCoroutine = null;
    }

    private IEnumerator WaveThreeSpawnEnemy()
    {
        while (enemyCount < 10)
        {
            int randomPoint = Random.Range(0, spawnPoints.Length);
            Instantiate(enemy, spawnPoints[randomPoint].position, Quaternion.identity);
            yield return new WaitForSeconds(2.5f);
        }

        myCoroutine = null;
    }

    private IEnumerator WaveFourSpawnEnemy()
    {
        while (enemyCount < 15)
        {
            int randomPoint = Random.Range(0, spawnPoints.Length);
            Instantiate(enemy, spawnPoints[randomPoint].position, Quaternion.identity);
            yield return new WaitForSeconds(2f);
        }

        myCoroutine = null;
    }
}
