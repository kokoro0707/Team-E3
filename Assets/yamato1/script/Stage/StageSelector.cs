using UnityEngine;

public class StageSelector : MonoBehaviour
{
    public GameObject stageSelectUI; // ステージ選択UI
    public GameObject gameplayRoot;  // ゲームプレイ用の親オブジェクト
    private int stageIndex;

    [System.Obsolete]
    public void SelectStage(int stageindex)
    {
        stageSelectUI.SetActive(false);      // ステージ選択UIを非表示
        gameplayRoot.SetActive(true);        // ゲームプレイを開始

        FindObjectOfType<StageManager>().StartStage(stageIndex);
        // ステージ開始
    }
}