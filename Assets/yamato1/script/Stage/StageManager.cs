using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;

public class StageManager : MonoBehaviour
{
    public List<StageData> stages; // ← これだけでOK

    public Text enemyCountText;
    public int currentStage = 0;
    private int remainingEnemies;

    void Start()
    {
        StartStage(currentStage);
    }

    public void StartStage(int stageIndex)
    {
        currentStage = stageIndex;
        remainingEnemies = stages[stageIndex].enemyCount;
        enemyCountText.text = "残りの敵: " + remainingEnemies;
        stages[stageIndex].ActivateSpawners();
    }

    public void OnEnemyDestroyed()
    {
        remainingEnemies--;
        enemyCountText.text = "残りの敵: " + remainingEnemies;

        if (remainingEnemies <= 0)
        {
            AdvanceToNextStage();
        }
    }

    void AdvanceToNextStage()
    {
        if (currentStage + 1 < stages.Count)
        {
            stages[currentStage].DeactivateSpawners();
            currentStage++;
            StartStage(currentStage);
        }
        else
        {
            Debug.Log("全ステージクリア！");
        }
    }

    internal void Start(int stageindex)
    {
        throw new NotImplementedException();
    }

    internal void StartStage(object stageIndex)
    {
        throw new NotImplementedException();
    }
}