using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public Image fadeImage;
    public GameObject gameOverUI;

    public RectTransform newWaveBanner;
    public RectTransform playerHealthBar;
    public TextMeshProUGUI newWaveTitle;
    public TextMeshProUGUI newWaveEnemyCount;
    public TextMeshProUGUI scoreUI;
    public TextMeshProUGUI gameOverScoreUIText;
    
    Spawner spawner;
    Player player;

    private void Awake() {
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;
    }

    void Start()
    {
        player = FindObjectOfType<Player>();
        // FindObjectOfType<Player>().OnDeath += OnGameOver;
        EventCenter.GetInstance().AddEventListener("OnPlayerDeath", OnGameOver);
    }

    private void OnDisable() {
        EventCenter.GetInstance().RemoveEventListener("OnPlayerDeath", OnGameOver);
    }

    void Update(){
        scoreUI.text = ScoreKeeper.score.ToString("D6");
        float healthPercent = 0;
        if(player != null){
            healthPercent = player.health / player.startHealth;
        }
        playerHealthBar.localScale = new Vector3(healthPercent, 1, 1);
    }
    void OnNewWave(int waveNumber){
        string[] numbers = {"One", "Two", "Three", "Four", "Five"};
        newWaveTitle.text = "- Wave " + numbers[waveNumber - 1] + " -";
        string enemyCountString = spawner.waves[waveNumber - 1].infinite ? "Infinite" : spawner.waves[waveNumber - 1].enemyCount + "";
        newWaveEnemyCount.text = "Enemies: " + enemyCountString;

        // 推荐使用字符串的方式开启和停止coroutine（前提是没有参数传递）
        StopCoroutine("AnimateNewWaveBanner");
        StartCoroutine("AnimateNewWaveBanner");
    }

    void OnGameOver(){
        StartCoroutine(Fade(Color.clear, Color.black, 1));

        scoreUI.gameObject.SetActive(false);
        playerHealthBar.transform.parent.gameObject.SetActive(false);
        gameOverUI.SetActive(true);

        gameOverScoreUIText.text = scoreUI.text;

        Cursor.visible = true;
    }

    IEnumerator Fade(Color from, Color to, float time){
        float speed = 1 / time;
        float percent = 0;

        while(percent < 1){
            percent += Time.deltaTime * speed;
            fadeImage.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    IEnumerator AnimateNewWaveBanner(){
        float delayTime = 2f;
        float speed = 3f;
        float percent = 0;
        int direction = 1;

        /*
        Time.time 表示从游戏开始到当前的流逝时间（以秒为单位）。

        Time.deltaTime 表示上一帧到这一帧的时间（以秒为单位）。
        */
        float endDelayTime = Time.time + 1 / speed + delayTime;

        while(percent >= 0){
            percent += Time.deltaTime * speed * direction;

            if(percent >= 1){
                percent = 1;
                if(Time.time > endDelayTime){ // 超过了等待时间
                    direction = -1;
                }
            }

            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(-170, 30, percent);
            /*
            表示暂停当前协程，直到下一帧结束时再恢复执行。它可以被用来实现帧间平滑过渡，例如动画效果等。
            */
            yield return null;
        }

    }

    // UI Input
    public void StartNewGame(){
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        PoolManager.GetInstance().Clear();
        EventCenter.GetInstance().Clear();
    }

    public void ReturnToMenu(){
        SceneManager.LoadScene("Menu");
    }
}
