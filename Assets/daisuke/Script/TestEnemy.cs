using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    // デバック用で作ったやつ
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            //SkillPointManager.instance.AddKillCount(1);

            GameOverManager.Instance.SetPlayerDeathPosition(transform.position);
            GameOverManager.Instance.StartCoroutine(GameOverManager.Instance.StartGameOver());
            Destroy(gameObject);
        }
    }
}
