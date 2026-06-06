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


    [Header("フェイズ用画像")]
    public Image phaseImage;               // フェイズ表示用Image
    public List<Sprite> phaseSprites;     // フェイズごとのスプライトリスト

    [Header("ゲーム全体オブジェクト")]
    //public GameObject gameObjectsGroup;

    [Header("フェイズごとのスポナー設定")]
    public List<GameObject> phaseSpawners = new List<GameObject>();

    [Header("フェイズごとの敵数設定")]
    public List<int> enemiesPerPhase = new List<int>();

    private int currentPhaseIndex = -1;
    private int remainingEnemies;
    private bool isPlaying = false;

    Coroutine enemyCountAnimCoroutine;
    float targetScaleMultiplier = 1f;

    void Start()
    {
        //phaseText.gameObject.SetActive(false);
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

        if (BGMManager.instance != null)
            BGMManager.instance.PlayBGM();
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

        // ---- アニメーション処理 ----
        // 拡大のトリガーを重ねても自然に反応
        targetScaleMultiplier = 1.3f; // もう少し大きくしてもOK

        if (enemyCountAnimCoroutine == null)
            enemyCountAnimCoroutine = StartCoroutine(AnimateEnemyCountText());
        // ----------------------------

        if (remainingEnemies <= 0)
        {
            remainingEnemies = 0;
            UpdateUI();
            targetScaleMultiplier = 1.3f;
            if (enemyCountAnimCoroutine == null)
                enemyCountAnimCoroutine = StartCoroutine(AnimateEnemyCountText());

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

        if(BGMManager.instance!=null)
            BGMManager.instance.FadeOut(1f) ;
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
        if (BGMManager.instance != null)
            BGMManager.instance.StopBGM();

        if (GameClearManager.instance!=null)
        {
            GameClearManager.instance.StartCoroutine(GameClearManager.instance.StartGameClear());
        }
    }

    void UpdateUI()
    {
        // テキスト表示は不要なら非表示にするか、そのままでもOK
        // phaseText.text = "フェイズ " + (currentPhaseIndex + 1);
        enemyCountText.text = remainingEnemies.ToString();

        if (phaseSprites != null && currentPhaseIndex < phaseSprites.Count)
        {
            phaseImage.sprite = phaseSprites[currentPhaseIndex];
            phaseImage.gameObject.SetActive(true);
        }
        else
        {
            phaseImage.gameObject.SetActive(false);
        }
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
        float duration = 0.15f;
        Vector3 baseScale = Vector3.one;
        float currentTime = 0f;

        while (true)
        {
            // スムーズにtargetScaleMultiplierへ補間
            Vector3 targetScale = baseScale * targetScaleMultiplier;
            enemyCountText.transform.localScale = Vector3.Lerp(
                enemyCountText.transform.localScale,
                targetScale,
                Time.deltaTime * 10f // ← スムーズさ調整
            );

            currentTime += Time.deltaTime;

            // 徐々に倍率を1fに戻す
            targetScaleMultiplier = Mathf.Lerp(targetScaleMultiplier, 1f, Time.deltaTime * 2f);

            // 一定時間スケールが元に戻ったら終了
            if (Mathf.Abs(targetScaleMultiplier - 1f) < 0.01f &&
                Vector3.Distance(enemyCountText.transform.localScale, baseScale) < 0.01f)
            {
                enemyCountText.transform.localScale = baseScale;
                break;
            }

            yield return null;
        }

        enemyCountAnimCoroutine = null;
    }

    public void AddEnemies(int count)
    {
        remainingEnemies += count;
        UpdateUI();
    }
}
