using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Enemy enemy;
    public Wave[] waves;

    [System.Serializable]
    public class Wave{
        public int enemyCount;
        public float timeBetweenSpawns;
    }

    Wave currentWave;
    int currentWaveNumber;
    int enemiesRemainingToSpawn;
    int enemiesRemainingToLive;
    float nextSpawnTime;

    void Start(){
        NextWave();
    }

    void Update()
    {
        if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
        {
            enemiesRemainingToSpawn--;
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

            Enemy spawnEnemy = Instantiate(enemy, Vector3.zero, Quaternion.identity) as Enemy;
            spawnEnemy.OnDeath += OnEnemyDeath; // subscribe this function: OnEnemyDeath. If the enemy is dead, OnDeath call this function.
        }
    }

    void OnEnemyDeath(){
        enemiesRemainingToLive--;
        if(enemiesRemainingToLive == 0){
            NextWave();
        }
    }

    void NextWave(){
        currentWaveNumber++;
        if(currentWaveNumber - 1 < waves.Length){
            currentWave = waves[currentWaveNumber - 1];
            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingToLive = enemiesRemainingToSpawn;
        }
    }
}
