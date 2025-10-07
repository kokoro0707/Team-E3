using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    // デバック用で作ったやつ
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            Destroy(gameObject);
            SkillPointManager.instance.AddKillCount(1);
        }
    }
}
