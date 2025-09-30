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
    [SerializeField] SkillLine nextLine; // Ÿ‚ÌƒXƒLƒ‹‚Ö‚Â‚È‚ª‚éü
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
        if (SkillManager.instance.HasSkill(skilltype)) return; // ‚·‚Å‚Éæ“¾Ï‚İ‚È‚ç–³‹

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
        nextLine?.ResetLine();
    }

    private void LearnSkill()
    {
        if (SkillManager.instance.HasSkill(skilltype)) return;

        SkillManager.instance.LearnSkill(this.skilltype);
        Debug.Log($"{ skilltype} K“¾Š®—¹");
        ChangeLearnedBlock(Color.blue);

        // ƒ‰ƒCƒ“‚ğ“h‚èØ‚Á‚½ó‘Ô‚ÉŒÅ’è
        nextLine?.SetComplete();

        //// ’·‰Ÿ‚µ‚ğ‰ğœ
        isHolding = false;
    }

    //public void OnClick()
    //{
    //    // K“¾Ï‚İ‚È‚ç‰½‚à‚µ‚È‚¢
    //    if (SkillManager.instance.HasSkill(this.skilltype))
    //    {
    //        Debug.Log("K“¾Ï‚İ");
    //        return;
    //    }

    //    // K“¾‰Â”\H
    //    if (SkillManager.instance.CanLearnSkill(cost, skilltype))
    //    {
    //        // K“¾‰Â”\‚È‚çK“¾‚·‚é
    //        SkillManager.instance.LearnSkill(this.skilltype);
    //        Debug.Log("K“¾");
    //        ChangeLearnedBlock(Color.blue);
    //    }
    //    else
    //    {
    //        // K“¾•s‰Â”\‚È‚çƒƒO‚ğo‚·
    //        Debug.Log("K“¾NG");
    //    }
    //}

    // K“¾‚µ‚½ê‡ hidepanelŠO‚·
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
