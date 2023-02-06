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
    float timeBetweenToCheckPlayerPositionTime = 2f; // 若玩家一段时间不动，则生成敌人在玩家附近
    float campThresholdDistance = 1.5f;
    float nextCampCheckTime;
    Vector3 playerOldPosition;
    bool isCamping;
    bool isDisabled;

    MapGenerator map;
    LivingEntity playerEntity;
    Transform playerTransform;

    void Start(){
        NextWave();
        map = FindObjectOfType<MapGenerator>();
        playerEntity = FindObjectOfType<Player>();
        playerEntity.OnDeath += OnPlayerDeath; // 不使用player中的dead，而是subscribe一个函数
        playerTransform = playerEntity.transform;

        nextCampCheckTime = Time.time + timeBetweenToCheckPlayerPositionTime;
        playerOldPosition = playerTransform.position;
    }

    void Update()
    {
        if(!isDisabled){
            if(Time.time > nextCampCheckTime){
                nextCampCheckTime = Time.time + timeBetweenToCheckPlayerPositionTime;

                // 玩家一段时间内移动的距离小于campThresholdDistance，则
                isCamping = Vector3.Distance(playerOldPosition, playerTransform.position) < campThresholdDistance;
                // 设定上一波玩家的最后位置
                playerOldPosition = playerTransform.position;
            }

            if (enemiesRemainingToSpawn > 0 && Time.time > nextSpawnTime)
            {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;
                
                StartCoroutine(SpawnEnemy());
            }
        }
    }

    IEnumerator SpawnEnemy(){
        float spawnDelayTime = 1f; // 变换颜色多久后生成敌人
        float tileFlashSpeed = 4f; // 变换颜色的速度
        float spawnTimer = 0f;

        // 获取tile的位置，变换颜色，生成敌人
        Transform randomTilePos = map.GetRandomOpenTile();

        // 玩家一段时间内移动距离太短，则将敌人生成在玩家所在位置
        if(isCamping){
            randomTilePos = map.PositionToCoord(playerTransform.position);
        }

        Material tileMaterial = randomTilePos.GetComponent<Renderer>().material;
        Color initalColor = tileMaterial.color;
        Color flashColor = Color.red;

        while(spawnTimer < spawnDelayTime){
            tileMaterial.color = Color.Lerp(initalColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1)); // PingPong -> 0 -> 1 -> 0
            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Enemy spawnEnemy = Instantiate(enemy, randomTilePos.position + Vector3.up, Quaternion.identity) as Enemy;
        spawnEnemy.OnDeath += OnEnemyDeath; // subscribe this function: OnEnemyDeath. If the enemy is dead, OnDeath call this function.
    }

    void OnPlayerDeath(){
        isDisabled = true;
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
