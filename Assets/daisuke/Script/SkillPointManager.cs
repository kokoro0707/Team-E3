using System;
using UnityEngine;
using TMPro;

public class SkillPointManager : MonoBehaviour
{
    public static SkillPointManager instance{ get; private set; }

    [SerializeField] private int skillPoints = 0;
    [SerializeField] private int killCount = 0;
    [SerializeField] private int killsPoint = 2; // 2体で1ポイント

    [SerializeField] private TextMeshProUGUI skillPointText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
            skillPoints = 3; // デバック用初期値
    }

    private void Start()
    {
        UpdateSkillPointUI();
    }

    private void UpdateSkillPointUI()
    {
        if(skillPointText != null)
        {
            skillPointText.text = $"Skill Point: {skillPoints}";
        }
    }

    public int GetSkillPoint() => skillPoints;

    // キルカウント増加
    public void AddKillCount(int amont)
    {
        killCount += amont;

        if(killCount >= killsPoint) // キルカウントがキルポイントを上回ったらスキルポイント増加
        {
            killCount = 0;
            AddSkillPoint(1);
        }
    }

    // スキルポイントを増やす
    private void AddSkillPoint(int amont)
    {
        skillPoints += amont;
        UpdateSkillPointUI();
    }

    // スキルポイント使用
    public bool UseSkillPoint(int cost)
    {
        if(skillPoints >= cost)
        {
            skillPoints -= cost;
            UpdateSkillPointUI();
            return true;
        }
        else
        {
            Debug.Log("スキルポイントが足りません");
            return false;
        }

    }
}
