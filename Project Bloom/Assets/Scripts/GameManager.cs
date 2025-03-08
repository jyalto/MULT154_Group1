using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject enemy;
    public int enemyCount = 0;

    public Transform[] spawnPoints;

    // Update is called once per frame
    void Update()
    {
        if (enemyCount < 1)
        {
            SpawnEnemy();
        }
    }

    private void SpawnEnemy()
    {
        //GameObject newEnemy = Instantiate(enemy, new Vector3(Random.Range(-47.6f, 47.6f), 0, Random.Range(-48.3f, 47.5f)), Quaternion.identity);
        int randomPoint = Random.Range(0, spawnPoints.Length);

        GameObject newEnemy = Instantiate(enemy, spawnPoints[randomPoint].position, Quaternion.identity);
    }
}