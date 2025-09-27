using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillBlock : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] SkillType skilltype;
    [SerializeField] int cost;
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
        if(isHolding && SkillManager.instance.CanLearnSkill(cost,skilltype))
        {
            holdCounter += Time.deltaTime;
            float progress = Mathf.Clamp01(holdCounter / holdTime);

            // 線を更新
            if (nextLine != null)

            nextLine.SetFillProgres(progress);
            if(progress >= 1f)
            {
                // 長押し完了でスキル取得
                LearnSkill();
                isHolding = false;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(SkillManager.instance.HasSkill(skilltype)) return; // すでに取得済みなら無視

        if(!SkillManager.instance.CanLearnSkill(cost,skilltype)) return; // 習得不可なら無視

        isHolding = true;
        holdCounter = 0;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHolding = false;
        holdCounter = 0f;

        // 線をリセット
        if (nextLine != null)
            nextLine.SetFillProgres(0f);
    }

    private void LearnSkill()
    {
        if (SkillManager.instance.HasSkill(skilltype)) return;

        SkillManager.instance.LearnSkill(this.skilltype);
        Debug.Log("${ skilltype} 習得完了");
        ChangeLearnedBlock(Color.blue);
    }

    //public void OnClick()
    //{
    //    // 習得済みなら何もしない
    //    if(SkillManager.instance.HasSkill(this.skilltype))
    //    {
    //        Debug.Log("習得済み");
    //        return;
    //    }

    //    // 習得可能？
    //    if (SkillManager.instance.CanLearnSkill(cost, skilltype))
    //    {
    //        // 習得可能なら習得する
    //        SkillManager.instance.LearnSkill(this.skilltype);
    //        Debug.Log("習得");
    //        ChangeLearnedBlock(Color.blue);
    //    }
    //    else
    //    {
    //        // 習得不可能ならログを出す
    //        Debug.Log("習得NG");
    //    }
    //}

    // 習得した場合 hidepanel外す
    public void CheckActiveBlock()
    {
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
