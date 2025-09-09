using UnityEngine;
using UnityEngine.UI;

public class SkillBlock : MonoBehaviour
{
    [SerializeField] SkillType skilltype;
    [SerializeField] int cost;
    [SerializeField] new string name;
    [SerializeField] string info;
    [SerializeField] GameObject hidePanel;

    void Start()
    {
        CheckActiveBlock();
    }

    public void OnClick()
    {
        // K“¾Ï‚İ‚È‚ç‰½‚à‚µ‚È‚¢
        if(SkillManager.instance.HasSkill(this.skilltype))
        {
            Debug.Log("K“¾Ï‚İ");
            return;
        }

        // K“¾‰Â”\H
        if (SkillManager.instance.CanLearnSkill(cost, skilltype))
        {
            // K“¾‰Â”\‚È‚çK“¾‚·‚é
            SkillManager.instance.LearnSkill(this.skilltype);
            Debug.Log("K“¾");
            ChangeLearnedBlock(Color.blue);
        }
        else
        {
            // K“¾•s‰Â”\‚È‚çƒƒO‚ğo‚·
            Debug.Log("K“¾NG");
        }
    }

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
        Image image = GetComponent<Image>();
        image.color = color;
    }
}
