using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public enum SkillType
{
    ThrowAttack,
    ThrowAttack2,
    ThrowAttack3,
    ATTACKRANGE,
    ATTACKRANGE2,
    Shield,
    Shield2,
    Shield3,
    ShieldRepair,
    ShieldRepairSpeed,
}
public class SkillManager : MonoBehaviour
{
    [SerializeField] GameObject skillBlockPanel;

    List<SkillType> skillList = new List<SkillType>();
    public SkillBlock[] skillBlocks;

    public static SkillManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }

        skillBlocks = skillBlockPanel.GetComponentsInChildren<SkillBlock>();
    }
    void Start()
    {
        CheckActiveBlocks();
    }

    // スキルを持っているか
    public bool HasSkill(SkillType skilltype)
    {
        return skillList.Contains(skilltype);
    }

    // スキルの習得条件
    public bool CanLearnSkill(int cost, SkillType skilltype)
    {
        if(disableSkills.Contains(skilltype)) return false;

        if(SkillPointManager.instance.GetSkillPoint() < cost)
        {
            return false;
        }

        if (HasSkill(skilltype)) return true;

        // ここにスキル取得条件追加
        switch (skilltype)
        {
            case SkillType.Shield2:                  // 取得したいスキル
                return HasSkill(SkillType.Shield);   // 取得するために必要なスキル
            case SkillType.Shield3:
                return HasSkill(SkillType.Shield2);
            case SkillType.ShieldRepair:
                return HasSkill(SkillType.Shield);
            case SkillType.ShieldRepairSpeed:
                return HasSkill(SkillType.ShieldRepair);
            case SkillType.ThrowAttack2:
                return HasSkill(SkillType.ThrowAttack);
                case SkillType.ThrowAttack3:
                return HasSkill(SkillType.ThrowAttack2);
            case SkillType.ATTACKRANGE:
                return HasSkill(SkillType.ThrowAttack);
            case SkillType.ATTACKRANGE2:
                return HasSkill(SkillType.ATTACKRANGE);
            default:
                return true;
        }
    }

    // 習得不可スキルの設定
    private List<SkillType> disableSkills = new List<SkillType>();

    public bool IsDisabled(SkillType skill)
    {
        return disableSkills.Contains(skill);
    }

    public void DisableSkill(SkillType skill)
    {
        if(!disableSkills.Contains(skill))
            disableSkills.Add(skill);
    }

    public delegate void SkillLearnHandler(SkillType skillType);
    public event SkillLearnHandler OnSkillLearned;

    // スキル習得
    public void LearnSkill(SkillType skillType)
    {
        int cost = 1;

        //  すでに習得済ならスキップ
        if (HasSkill(skillType)) return;

        // スキルポイントが足りなければスキップ
        if (SkillPointManager.instance.GetSkillPoint() < cost) return; 

        // 習得条件を満たしていないならスキップ
        if (!CanLearnSkill(cost, skillType)) return;

        // スキルポイントを消費
        if (!SkillPointManager.instance.UseSkillPoint(cost)) return;

        // 習得処理
         skillList.Add(skillType);
         OnSkillLearned?.Invoke(skillType);

        // スキルブロックにアニメーションを伝達
        foreach (SkillBlock block in skillBlocks)
        {
            if (skillBlocks != null && block.SkillType == skillType)
            {
                block.AnimateConnectedLines();
                break;
            }
        }
            CheckActiveBlocks();
    }

    // スキル習得済パネル確認
    public void CheckActiveBlocks()
    {
        Debug.Log("checkblock実行");
        if (skillBlocks == null) return;
        foreach (SkillBlock skillBlock in skillBlocks)
        {
            if((skillBlock == null)) continue;
            Debug.Log($"[check]{skillBlock.name}: HasSkill = {HasSkill(skillBlock.SkillType)}");

            if (HasSkill(skillBlock.SkillType))
            {
                Debug.Log($"→ Skip {skillBlock.name}(already learned)");
                continue;
            }
            skillBlock.CheckActiveBlock();
        }
    }
}
