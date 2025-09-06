using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UIElements;

public class SkillTreeActive : MonoBehaviour
{
    [SerializeField] private RectTransform SkillTreePanel;
    [SerializeField] private float slideDistance = 800f; // 下方向への移動量
    [SerializeField] private float slideSpeed = 5f;
    [SerializeField] private float slideDelay = 0.3f; // 現状使ってない関数

    private bool isActive = false;
    private Vector2 visiblePosition;
    private Vector2 hiddenPositin;
    private Vector2 targetPosition;

    private void Start()
    {
        // 現在位置を表示位置とする
        visiblePosition = SkillTreePanel.anchoredPosition;

        // 非表示位置を下にずらして固定
        hiddenPositin = new Vector2(visiblePosition.x, visiblePosition.y - slideDistance);

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

        // スライドの処理にフェードやエフェクトを入れたい場合使用する、現状使わない
        //if (!isActive)
        //{
        //    StopAllCoroutines();
        //    StartCoroutine(DisableAfterSlide());
        //}
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

    private IEnumerator DisableAfterSlide()
    {
        yield return new WaitForSecondsRealtime(slideDelay);

        SkillTreePanel.gameObject.SetActive(false);
    }
}
