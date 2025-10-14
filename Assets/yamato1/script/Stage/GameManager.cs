using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int currentPhase = 0;
    public int maxPhase = 3;
    public int enemiesRemaining = 0;

    public Text enemyCountText;
    public GameObject startButton;
    public GameObject gameClearPanel;

    public GameObject phaseTransitionPanel;
    public Text phaseText;
    public Animator phaseAnimator;

    public EnemySpawner[] phaseSpawners;

    public void StartGame()
    {
        startButton.SetActive(false);
        currentPhase = 1;
        StartPhase(currentPhase);
    }

    void StartPhase(int phase)
    {
        foreach (var spawner in phaseSpawners)
            spawner.gameObject.SetActive(false);

        phaseSpawners[phase - 1].gameObject.SetActive(true);
        phaseSpawners[phase - 1].SpawnEnemies();
    }

    public void SetEnemyCount(int count)
    {
        enemiesRemaining = count;
        UpdateEnemyCountUI();
    }

    public void OnEnemyDefeated()
    {
        enemiesRemaining--;
        UpdateEnemyCountUI();

        if (enemiesRemaining <= 0)
        {
            if (currentPhase < maxPhase)
            {
                currentPhase++;
                StartCoroutine(TransitionToNextPhase(currentPhase));
            }
            else
            {
                GameClear();
            }
        }
    }

    IEnumerator TransitionToNextPhase(int nextPhase)
    {
        phaseText.text = "フェイズ " + nextPhase + " 開始！";
        phaseTransitionPanel.SetActive(true);
        phaseAnimator.SetTrigger("Play");

        yield return new WaitForSeconds(2f);

        phaseTransitionPanel.SetActive(false);
        StartPhase(nextPhase);
    }

    void UpdateEnemyCountUI()
    {
        enemyCountText.text = "残り敵数: " + enemiesRemaining;
    }

    void GameClear()
    {
        gameClearPanel.SetActive(true);
    }
}
