using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public enum SkillType
{
    ATTACK,
    ATTACKRANGE,
    Shield,
    Shield2,
    Shield3,
    ShieldRepair,
    ShieldRepairSpeed,
}
public class SkillManager : MonoBehaviour
{
    [SerializeField] GameObject skillBlockPanel;
    int skillPoint;

    List<SkillType> skillList = new List<SkillType>();
    SkillBlock[] skillBlocks;

    public static SkillManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        skillBlocks = skillBlockPanel.GetComponentsInChildren<SkillBlock>();
    }
    void Start()
    {
    }

    // スキルを持っているか
    public bool HasSkill(SkillType skilltype)
    {
        return skillList.Contains(skilltype);
    }

    // スキルの習得条件
    public bool CanLearnSkill(int cost, SkillType skilltype)
    {
        if(skillPoint < cost)
        {
            return false;
        }

        // ここに追加
        if(skilltype == SkillType.Shield2)
        {
            return HasSkill(SkillType.Shield);
        }
        return true;
    }

    // スキル習得
    public void LearnSkill(SkillType skillType)
    {
        skillList.Add(skillType);
        CehckActiveBlocks();
    }

    void CehckActiveBlocks()
    {
        foreach (SkillBlock skillBlock in skillBlocks)
        {
            skillBlock.CheckActiveBlock();
        }
    }
}
