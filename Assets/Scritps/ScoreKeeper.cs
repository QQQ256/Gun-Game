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
        EventCenter.GetInstance().AddEventListener("OnEnemyDeath", OnEnemyKilled);
        EventCenter.GetInstance().AddEventListener("OnPlayerDeath", OnPlayerDeath);
    }

    private void OnDisable() {
        EventCenter.GetInstance().RemoveEventListener("OnPlayerDeath", OnPlayerDeath);
        EventCenter.GetInstance().RemoveEventListener("OnEnemyDeath", OnEnemyKilled);
    }
    
    void OnEnemyKilled(){
        score += 5;
    }

    // 玩家死后重新开始游戏，上面的事件会被订阅两次
    // 所以这里检测玩家死亡后应该取消订阅OnEnemyKilled()
    void OnPlayerDeath(){
        // Enemy.OnDeathStatic -= OnEnemyKilled;
        EventCenter.GetInstance().RemoveEventListener("OnEnemyDeath", OnEnemyKilled);
    }
}
