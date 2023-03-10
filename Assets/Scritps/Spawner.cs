using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public bool devMode;
    public Enemy enemy;
    public Wave[] waves;
    public event System.Action<int> OnNewWave; // <int>存储现在是第几波

    [System.Serializable]
    public class Wave{
        public bool infinite;
        public int enemyCount;
        public int hitsToKillPlayer;
        public float timeBetweenSpawns;
        public float moveSpeed;
        public float enemyHealth;
        public Color skinColor;
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
        map = FindObjectOfType<MapGenerator>();
        playerEntity = FindObjectOfType<Player>();
        playerTransform = playerEntity.transform;

        nextCampCheckTime = Time.time + timeBetweenToCheckPlayerPositionTime;
        playerOldPosition = playerTransform.position;

        playerEntity.OnDeath += OnPlayerDeath; // 不使用player中的dead，而是subscribe一个函数
        MonoManager.GetInstance().AddUpdateEventListener(SpawnUpdate);
        NextWave();
    }
    
    void OnDisable(){
        MonoManager.GetInstance().RemoveUpdateEventListener(SpawnUpdate);
    }

    private void SpawnUpdate(){
        if(!isDisabled){
            if(Time.time > nextCampCheckTime){
                nextCampCheckTime = Time.time + timeBetweenToCheckPlayerPositionTime;

                // 玩家一段时间内移动的距离小于campThresholdDistance，则
                isCamping = Vector3.Distance(playerOldPosition, playerTransform.position) < campThresholdDistance;
                // 设定上一波玩家的最后位置
                playerOldPosition = playerTransform.position;
                
            }
            
            if ((enemiesRemainingToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime)
            {
                enemiesRemainingToSpawn--;
                nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;
                
                StartCoroutine(SpawnEnemy());
            }
        }

        if(devMode){
            if(Input.GetKeyDown(KeyCode.Return)){
                StopCoroutine(SpawnEnemy());
                foreach(Enemy enemy in FindObjectsOfType<Enemy>()){
                    GameObject.Destroy(enemy.gameObject);
                }
                NextWave();
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
        Color initalColor = Color.white;
        Color flashColor = Color.red;

        while(spawnTimer < spawnDelayTime){
            tileMaterial.color = Color.Lerp(initalColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1)); // PingPong -> 0 -> 1 -> 0
            spawnTimer += Time.deltaTime;
            yield return null;
        }

        GameObject spawnEnemy = PoolManager.GetInstance().GetObjectFromPool("Prefabs/Enemy");
        spawnEnemy.transform.position = randomTilePos.position + Vector3.up;
        spawnEnemy.transform.rotation = Quaternion.identity;
        spawnEnemy.GetComponent<Enemy>().OnDeath += OnEnemyDeath;
        spawnEnemy.GetComponent<Enemy>().SetCharacteristics(currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColor);
        // Enemy spawnEnemy = Instantiate(enemy, randomTilePos.position + Vector3.up, Quaternion.identity) as Enemy;
        // spawnEnemy.GetComponent<Enemy>().OnDeath += OnEnemyDeath; // subscribe this function: OnEnemyDeath. If the enemy is dead, OnDeath call this function.
        // spawnEnemy.GetComponent<Enemy>().SetCharacteristics(currentWave.moveSpeed, currentWave.hitsToKillPlayer, currentWave.enemyHealth, currentWave.skinColor);
    }

    void OnPlayerDeath(){
        isDisabled = true;
    }

    void OnEnemyDeath(){
        // PoolManager.GetInstance().PushObjectToPool(this.gameObject.name, this.gameObject);
        enemiesRemainingToLive--;
        if(enemiesRemainingToLive == 0){
            NextWave();
        }
    }


    void NextWave(){
        if(currentWaveNumber > 0){
            AudioManager.instance.PlaySound2D("Level Complete");
        }

        currentWaveNumber++;
        if(currentWaveNumber - 1 < waves.Length){
            currentWave = waves[currentWaveNumber - 1];
            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingToLive = enemiesRemainingToSpawn;

            if(OnNewWave != null){
                OnNewWave(currentWaveNumber);
            }

            // EventCenter.GetInstance().EventTrigger("OnNewWave");

            ResetPlayerPosition();
        }
    }

    [ContextMenu("hahha")]
    void ResetPlayerPosition(){
        playerTransform.position = map.PositionToCoord(Vector3.zero).position + Vector3.up * 3;
    }

}
