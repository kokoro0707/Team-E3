using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [System.Serializable]
    public class Phase
    {
        public string phaseName;       // 「フェイズ1」「フェイズ2」などの表示名
        public EnemySpawner spawner;   // このフェイズで使うスポナー
        public int enemyCount;         // スポーンする敵の数
    }

    public Phase[] phases;              // フェイズの配列

    public Text enemyCountText;         // 左上の残り敵数表示
    public GameObject phaseTransitionPanel; // 中央にフェイズ名表示用パネル
    public Text phaseTransitionText;    // パネル内のテキスト
    public GameObject gameClearPanel;   // ゲームクリア表示パネル

    private int currentPhaseIndex = -1;
    private int enemiesRemaining = 0;

    void Start()
    {
        StartNextPhase();
    }

    void StartNextPhase()
    {
        currentPhaseIndex++;
        if (currentPhaseIndex >= phases.Length)
        {
            GameClear();
            return;
        }

        StartCoroutine(PhaseTransition(phases[currentPhaseIndex]));
    }

    IEnumerator PhaseTransition(Phase phase)
    {
        // フェイズ開始演出表示
        phaseTransitionPanel.SetActive(true);
        phaseTransitionText.text = phase.phaseName;
        enemyCountText.gameObject.SetActive(false);

        yield return new WaitForSeconds(2f);

        // 演出終了
        phaseTransitionPanel.SetActive(false);
        enemyCountText.gameObject.SetActive(true);

        // 敵の数をセットし、UI更新
        enemiesRemaining = phase.enemyCount;
        UpdateEnemyCountUI();

        // スポナーを有効化し敵をスポーン
        phase.spawner.gameObject.SetActive(true);
        phase.spawner.SpawnEnemies(enemiesRemaining);
    }

    // 敵を倒したときに呼ぶ
    public void OnEnemyDefeated()
    {
        enemiesRemaining--;
        UpdateEnemyCountUI();

        if (enemiesRemaining <= 0)
        {
            // 現フェイズのスポナーを非表示にする
            phases[currentPhaseIndex].spawner.gameObject.SetActive(false);

            // 次のフェイズへ
            StartNextPhase();
        }
    }

    void UpdateEnemyCountUI()
    {
        enemyCountText.text = $"残り敵数: {enemiesRemaining}";
    }

    void GameClear()
    {
        enemyCountText.gameObject.SetActive(false);
        phaseTransitionPanel.SetActive(false);
        gameClearPanel.SetActive(true);
    }
}
