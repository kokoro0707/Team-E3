using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageManager01 : MonoBehaviour
{
    [Header("UI設定")]
    //public Button startButton;
    public Text phaseText;          // フェイズ表示用テキスト
    public Text enemyCountText;     // 敵数表示用テキスト（数字のみ）

    [Header("ゲーム全体オブジェクト")]
    //public GameObject gameObjectsGroup;

    [Header("フェイズごとのスポナー設定")]
    public List<GameObject> phaseSpawners = new List<GameObject>();

    [Header("フェイズごとの敵数設定")]
    public List<int> enemiesPerPhase = new List<int>();

    private int currentPhaseIndex = -1;
    private int remainingEnemies;
    private bool isPlaying = false;

    void Start()
    {
        phaseText.gameObject.SetActive(false);
        enemyCountText.gameObject.SetActive(false);

        foreach (var spawner in phaseSpawners)
        {
            if (spawner != null)
                spawner.SetActive(false);
        }

        StartGame();
    }

    public void StartGame()
    {
       //startButton.gameObject.SetActive(false);
        phaseText.gameObject.SetActive(true);
        enemyCountText.gameObject.SetActive(true);

        isPlaying = true;
        currentPhaseIndex = -1;
        NextPhase();
    }

    void NextPhase()
    {
        currentPhaseIndex++;

        if (currentPhaseIndex >= enemiesPerPhase.Count)
        {
            GameClear();
            return;
        }

        foreach (var spawner in phaseSpawners)
        {
            if (spawner != null)
                spawner.SetActive(false);
        }


        remainingEnemies = enemiesPerPhase[currentPhaseIndex];

        if (phaseSpawners[currentPhaseIndex] != null)
            phaseSpawners[currentPhaseIndex].SetActive(true);

        UpdateUI();
    }

    private bool nextPhaseCalled = false;

    public void OnEnemyDestroyed()
    {
        if (remainingEnemies <= 0) return;

        remainingEnemies--;
        UpdateUI();
        StartCoroutine(AnimateEnemyCountText());

        if(remainingEnemies<=0)
        {
            remainingEnemies = 0;
            UpdateUI();
            StartCoroutine(AnimateEnemyCountText());

            Invoke(nameof(NextPhase), 2f);
        }
    }


    public void OnPlayerDead()
    {
        if (!isPlaying) return;

        isPlaying = false;

        foreach (var spawner in phaseSpawners)
        {
            if (spawner != null)
                spawner.SetActive(false);
        }

        Debug.Log("Game Over!");
        // TODO: ゲームオーバー画面追加予定
    }

    void GameClear()
    {
        isPlaying = false;

        foreach (var spawner in phaseSpawners)
        {
            if (spawner != null)
                spawner.SetActive(false);
        }
        Debug.Log("Game Clear!");
        // TODO: クリア画面追加予定
        if(GameClearManager.instance!=null)
        {
            GameClearManager.instance.StartCoroutine(GameClearManager.instance.StartGameClear());
        }
    }

    void UpdateUI()
    {
        phaseText.text = "フェイズ " + (currentPhaseIndex + 1);
        enemyCountText.text = remainingEnemies.ToString();
    }

    //void DestroyAllEnemies()
    //{
    //    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
    //    foreach (var enemy in enemies)
    //    {
    //        Destroy(enemy);
    //    }
    //}

    IEnumerator AnimateEnemyCountText()
    {
        float duration = 0.2f;
        Vector3 originalScale = enemyCountText.transform.localScale;
        Vector3 targetScale = originalScale * 1.3f;

        float timer = 0f;

        // 拡大アニメーション
        while (timer < duration)
        {
            enemyCountText.transform.localScale = Vector3.Lerp(originalScale, targetScale, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        enemyCountText.transform.localScale = targetScale;

        // 縮小アニメーション
        timer = 0f;
        while (timer < duration)
        {
            enemyCountText.transform.localScale = Vector3.Lerp(targetScale, originalScale, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        enemyCountText.transform.localScale = originalScale;
    }

    public void AddEnemies(int count)
    {
        remainingEnemies += count;
        UpdateUI();
    }
}
