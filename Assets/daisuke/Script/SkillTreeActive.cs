using UnityEngine;

public class SkillTreeActive : MonoBehaviour
{
    [SerializeField] private RectTransform SkillTreePanel;
    [SerializeField] private float slideDistance = 800f; // 下方向への移動量
    [SerializeField] private float slideSpeed = 5f;

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

        targetPosition = isActive ? visiblePosition : hiddenPositin;
    }

    private void Update()
    {
        // スライドアニメーション
        SkillTreePanel.anchoredPosition = Vector2.Lerp(SkillTreePanel.anchoredPosition, targetPosition, Time.unscaledDeltaTime * slideSpeed);

        if (Input.GetKeyDown(KeyCode.M))
        {
            ActiveSkillTree();
        }
    }
}
