using System;
using UnityEngine;
using TMPro;

public class SkillPointManager : MonoBehaviour
{
    public static SkillPointManager instance{ get; private set; }

    [SerializeField] private int skillPoints = 0;
    [SerializeField] private int killCount = 0;
    [SerializeField] private int totalKillCount = 0; //　合計キル数
    [SerializeField] private int killsPoint = 11; // 11体で1ポイント

    [SerializeField] private TextMeshProUGUI skillPointText;

    [SerializeField] private AudioSource seSource;
    [SerializeField] private AudioClip drumClip;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
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
        totalKillCount += amont;

        if(killCount >= killsPoint) // キルカウントがキルポイントを上回ったらスキルポイント増加
        {
            killCount = 0;
            Debug.Log("uu");
            AddSkillPoint(1);
        }
    }

    // スキルポイントを増やす
    private void AddSkillPoint(int amont)
    {
        if (seSource != null && drumClip != null)
        {
            seSource.clip = drumClip;
            seSource.Play();
        }
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

    // 合計キル数を取得
    public int GetTotalKillCount()
    {
        return totalKillCount;
    }

    public void ResetKillCount()
    {
        killCount = 0;
        totalKillCount = 0;
    }
}
