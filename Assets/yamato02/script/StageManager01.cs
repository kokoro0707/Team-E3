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
    public GameObject gameObjectsGroup; // 床・壁・プレイヤー・敵などをまとめたオブジェクト

    [Header("フェイズごとのスポナー設定")]
    public List<GameObject> phaseSpawners = new List<GameObject>();

    [Header("フェイズごとの敵数設定")]
    public List<int> enemiesPerPhase = new List<int>();

    private int currentPhaseIndex = -1;
    private int remainingEnemies;
    private bool isPlaying = false;

    void Start()
    {
        // 最初はUIを整理
        phaseText.gameObject.SetActive(false);
        enemyCountText.gameObject.SetActive(false);

        // ゲーム全体を非表示
        if (gameObjectsGroup != null)
            gameObjectsGroup.SetActive(false);

        // 全てのスポナーを無効化しておく
        foreach (var spawner in phaseSpawners)
        {
            if (spawner != null)
                spawner.SetActive(false);
        }

        // スタートボタンを設定
        startButton.onClick.AddListener(StartGame);
    }

    // 🔹 ゲームスタート処理
    public void StartGame()
    {
        // ボタンを非表示にしてUIを有効化
        startButton.gameObject.SetActive(false);
        phaseText.gameObject.SetActive(true);
        enemyCountText.gameObject.SetActive(true);

        // ゲームオブジェクト群を表示
        if (gameObjectsGroup != null)
            gameObjectsGroup.SetActive(true);

        // ゲーム開始
        isPlaying = true;
        currentPhaseIndex = -1;
        NextPhase();
    }

    // 🔹 フェイズ切り替え処理
    void NextPhase()
    {
        currentPhaseIndex++;

        // フェイズが全部終わったらゲームクリア
        if (currentPhaseIndex >= enemiesPerPhase.Count)
        {
            GameClear();
            return;
        }

        // 前フェイズのスポナーを止める
        foreach (var spawner in phaseSpawners)
        {
            if (spawner != null)
                spawner.SetActive(false);
        }

        // 今のフェイズのスポナーをONにする
        if (phaseSpawners[currentPhaseIndex] != null)
            phaseSpawners[currentPhaseIndex].SetActive(true);

        // 敵の数を設定
        remainingEnemies = enemiesPerPhase[currentPhaseIndex];
        UpdateUI();
    }

    // 🔹 敵が倒されたときに呼ぶ
    public void OnEnemyDestroyed()
    {
        remainingEnemies--;
        UpdateUI();

        if (remainingEnemies <= 0)
        {
            Invoke(nameof(NextPhase), 1f); // 少し待って次フェイズへ
        }
    }

    // 🔹 プレイヤーが倒されたときに呼ぶ
    public void OnPlayerDead()
    {
        if (!isPlaying) return;

        isPlaying = false;

        // 全てのスポナーを停止
        foreach (var spawner in phaseSpawners)
        {
            if (spawner != null)
                spawner.SetActive(false);
        }

        Debug.Log("Game Over!");
        // あとでここに「ゲームオーバー画面を表示」処理を追加
    }

    // 🔹 ゲームクリア時
    void GameClear()
    {
        isPlaying = false;

        // スポナー全停止
        foreach (var spawner in phaseSpawners)
        {
            if (spawner != null)
                spawner.SetActive(false);
        }

        Debug.Log("Game Clear!");
        // あとでここに「ゲームクリア画面を表示」処理を追加
    }

    // 🔹 UI更新
    void UpdateUI()
    {
        phaseText.text = "フェイズ " + (currentPhaseIndex + 1);
        enemyCountText.text = "残りの敵: " + remainingEnemies;
    }
}
