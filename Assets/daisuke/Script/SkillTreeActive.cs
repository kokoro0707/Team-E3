using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UIElements;

public class SkillTreeActive : MonoBehaviour
{
    [SerializeField] private RectTransform SkillTreePanel;
    [SerializeField] private float slideSpeed = 5f;

    private bool isActive = false;
    private Vector2 visiblePosition;
    private Vector2 hiddenPositin;
    private Vector2 targetPosition;

    private void Start()
    {
        // 現在位置を表示位置とする
        visiblePosition = SkillTreePanel.anchoredPosition;

        // 非表示位置を下にずらして固定(パネルの高さぶん下げる)
        hiddenPositin = new Vector2(visiblePosition.x, visiblePosition.y - SkillTreePanel.rect.height);

        // 初期状態は非表示
        SkillTreePanel.anchoredPosition = hiddenPositin;
        targetPosition = hiddenPositin;
    }
    public void ActiveSkillTree()
    {
        isActive = !isActive;

        if(isActive)
        {
            SkillTreePanel.gameObject.SetActive(true);
            Time.timeScale = 0f; // ゲームの一時停止
        }
        else
        {
            Time.timeScale = 1f; // ゲーム再開
        }

        targetPosition = isActive ? visiblePosition : hiddenPositin;
    }

    private void Update()
    {
        // スライドアニメーション
        SkillTreePanel.anchoredPosition = Vector2.Lerp(SkillTreePanel.anchoredPosition, targetPosition, Time.unscaledDeltaTime * slideSpeed);

        // キーボード入力時にも表示
        if (Input.GetKeyDown(KeyCode.M))
        {
            ActiveSkillTree();
        }
    }
}
