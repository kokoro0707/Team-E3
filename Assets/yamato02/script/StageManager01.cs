using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageManager01 : MonoBehaviour
{
    [Header("UI設定")]
    public Button startButton;
    public Text phaseText;
    public Text enemyCountText;

    [Header("ゲーム全体オブジェクト")]
    public GameObject gameObjectsGroup;

    [Header("フェイズごとのスポナー設定")]
    public List<GameObject> phaseSpawners = new List<GameObject>();

    [Header("フェイズごとの敵数設定")]
    public List<int> enemiesPerPhase = new List<int>();

    private int currentPhaseIndex = -1;
    private int remainingEnemies;
    private bool isPlaying = false;

    void Start()
    {
        // UI非表示
        phaseText.gameObject.SetActive(false);
        enemyCountText.gameObject.SetActive(false);

        // ゲーム全体非表示
        if (gameObjectsGroup != null)
            gameObjectsGroup.SetActive(false);

        // 全スポナー無効化
        foreach (var spawner in phaseSpawners)
        {
            if (spawner != null)
                spawner.SetActive(false);
        }

        // スタートボタンイベント登録
        startButton.onClick.AddListener(StartGame);
    }

    // 🔹 ゲーム開始処理
    public void StartGame()
    {
        startButton.gameObject.SetActive(false);
        phaseText.gameObject.SetActive(true);
        enemyCountText.gameObject.SetActive(true);

        if (gameObjectsGroup != null)
            gameObjectsGroup.SetActive(true);

        isPlaying = true;
        currentPhaseIndex = -1;
        NextPhase();
    }

    // 🔹 フェイズ切替処理
    void NextPhase()
    {
        currentPhaseIndex++;

        // ゲームクリアチェック
        if (currentPhaseIndex >= enemiesPerPhase.Count)
        {
            GameClear();
            return;
        }

        // 全スポナー無効化
        foreach (var spawner in phaseSpawners)
        {
            if (spawner != null)
                spawner.SetActive(false);
        }

        // 残った敵を全削除（前フェイズの敵）
        DestroyAllEnemies();

        // 敵数設定
        remainingEnemies = enemiesPerPhase[currentPhaseIndex];

        // スポナー有効化
        if (phaseSpawners[currentPhaseIndex] != null)
            phaseSpawners[currentPhaseIndex].SetActive(true);

        UpdateUI();
    }

    // 🔹 敵が倒されたときに呼ばれる
    public void OnEnemyDestroyed()
    {
        if (remainingEnemies <= 0) return;  // すでに0以下なら処理しない（連続呼び出し対策）

        remainingEnemies--;
        UpdateUI();

        if (remainingEnemies <= 0)
        {
            remainingEnemies = 0;  // 負の値にならないように固定
            UpdateUI();            // 念のためUIも更新

            DestroyAllEnemies();   // 残ってる敵を即削除

            Invoke(nameof(NextPhase), 2f); // 2秒待って次フェイズへ
        }
    }

    // 🔹 プレイヤーがやられたとき
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

    // 🔹 ゲームクリア
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
    }

    // 🔹 UI更新
    void UpdateUI()
    {
        phaseText.text = "フェイズ " + (currentPhaseIndex + 1);
        enemyCountText.text = "残りの敵: " + remainingEnemies;
    }

    // 🔹 敵をすべて削除
    void DestroyAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (var enemy in enemies)
        {
            Destroy(enemy);
        }
    }
}
