using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillBlock : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] SkillType skilltype;
    [SerializeField] int cost = 1;
    [SerializeField] new string name;
    [SerializeField] string info;
    [SerializeField] GameObject hidePanel;
    [SerializeField] SkillLine nextLine; // 次のスキルへつながる線
    [SerializeField] float holdTime = 1.5f;

    private bool isHolding = false;
    private float holdCounter = 0;

    private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        CheckActiveBlock();
    }

    private void Update()
    {
        if (!isHolding) return;
        if(!SkillManager.instance.CanLearnSkill(cost,skilltype)) return;

        holdCounter += Time.unscaledDeltaTime;
        float progress = Mathf.Clamp01(holdCounter / holdTime);

        nextLine?.SetFillProgress(progress);
        if (progress >= 1f)
        {
            LearnSkill();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
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
            nextLine?.ResetLine();
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
        ChangeLearnedBlock(Color.blue);

        // ラインを塗り切った状態に固定
        if(nextLine != null) nextLine.SetComplete();

        //// 長押しを解除
        isHolding = false;
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
