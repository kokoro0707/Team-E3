using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    // デバック用で作ったやつ
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            // こっちは敵死亡時に呼ぶ
            SkillPointManager.instance.AddKillCount(1);

            // こっちはプレイヤー死亡時に呼ぶ
            GameOverManager.Instance.SetPlayerDeathPosition(transform.position);
            GameOverManager.Instance.StartCoroutine(GameOverManager.Instance.StartGameOver());
            Destroy(gameObject);
        }
    }
}
