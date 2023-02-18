using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreKeeper : MonoBehaviour
{
    public static int score {get; private set;}

    float lastEnemyKillTime;
    float streakExpireTime = 2f;
    int streakCount;
    void Start()
    {
        Enemy.OnDeathStatic += OnEnemyKilled;
        FindObjectOfType<Player>().OnDeath += OnPlayerDeath;
    }

    void OnEnemyKilled(){
        // if(Time.time < lastEnemyKillTime + streakExpireTime){
        //     streakCount++;
        // }
        // else{
        //     streakCount = 0;
        // }

        // lastEnemyKillTime = Time.time;

        // score += 5 + (int)Mathf.Pow(2, streakCount);
        score += 5;
    }

    // 玩家死后重新开始游戏，上面的事件会被订阅两次
    // 所以这里检测玩家死亡后应该取消订阅OnEnemyKilled()
    void OnPlayerDeath(){
        Enemy.OnDeathStatic -= OnEnemyKilled;
    }
}
