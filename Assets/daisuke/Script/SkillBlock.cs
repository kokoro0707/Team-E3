using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillBlock : MonoBehaviour, IPointerEnterHandler,IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] SkillType skilltype;
    [SerializeField] int cost = 1;
    [SerializeField] new string name;
    [SerializeField] string info;
    [SerializeField] GameObject hidePanel;
    [Header("次へとつながるラインを入れる")]
    [SerializeField] SkillLine[] nextLine; // 次のスキルへつながる線
    [SerializeField] float holdTime = 1.5f;

    private bool isHolding = false;
    private float holdCounter = 0;

    private Image image;

    [SerializeField] private SkillContent content;

    public SkillType SkillType => skilltype;

    void Start()
    {
        image = GetComponent<Image>();
        CheckActiveBlock();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("enter");
        if (content != null)
            content.ShowContent(info);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (content != null)
            content.HideContent();
    }

    private void Update()
    {
        if (!isHolding) return;
        if(!SkillManager.instance.CanLearnSkill(cost,skilltype)) return;

        holdCounter += Time.unscaledDeltaTime;
        float progress = Mathf.Clamp01(holdCounter / holdTime);

        foreach (var line in nextLine)
        {
            line?.SetFillProgress(progress);
        }

        if (progress >= 1f)
        {
            LearnSkill();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(isHolding) return;
        Debug.Log("onPointerDown");
        if (SkillManager.instance.HasSkill(skilltype)) return; // すでに取得済みなら無視

        if (!SkillManager.instance.CanLearnSkill(cost, skilltype))
        {
            Debug.Log("CanLearnSkill == false");
            return;
        }
        Debug.Log("CanLearnSkill == true");
        isHolding = true;
        holdCounter = 0;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("onPointerUp");
        isHolding = false;
        holdCounter = 0f;
        if(!SkillManager.instance.HasSkill(skilltype))
        {
            foreach (var line in nextLine)
            {
                line?.ResetLine();
            }
        }
    }

    private void LearnSkill()
    {
        // すでに取得済みならスキップ
        if (SkillManager.instance.HasSkill(skilltype)) return;

        // スキルポイント・前提スキルがなければスキップ
        if (!SkillManager.instance.CanLearnSkill(cost, skilltype)) return;

        // スキル習得
        SkillManager.instance.LearnSkill(skilltype);
        Debug.Log($"{ skilltype} 習得完了");
        ChangeLearnedBlock(Color.gray);

        // ラインを塗り切った状態に固定
        foreach (var line in nextLine)
        {
            line?.SetComplete();
        }

        //// 長押しを解除
        isHolding = false;
    }

    public void AnimateConnectedLines()
    {
        foreach(var line in nextLine)
        {
            line?.AnimaterFill();
        }
    }

    // 習得した場合 hidepanel外す
    public void CheckActiveBlock()
    {
        // すでに習得済なら、パネルは常に表示
        if(SkillManager.instance.HasSkill(skilltype))
        {
            hidePanel.SetActive(false);
            return;
        }

        // 習得済でない場合は、習得条件をチェック
        if (SkillManager.instance.CanLearnSkill(cost, skilltype))
        {
            hidePanel.SetActive(false);
        }
        else
        {
            hidePanel.SetActive(true);
        }
    }

    void ChangeLearnedBlock(Color color)
    {
        image.color = color;
    }
}
