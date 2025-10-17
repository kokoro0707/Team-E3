using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    public float detectionRadius = 5f;
    public float speed = 4f;
    public float verticalSpeed = 2f;
    public float rotationSpeed = 360f;

    private Transform player;
    private bool isChasing = false;
    private bool isDestroyed = false; // ← 追加：破壊済みかどうか

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        if (isDestroyed) return; // すでに破壊されたら何もしない

        // 常に回転
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRadius)
        {
            isChasing = true;
        }

        if (isChasing)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
        else
        {
            Vector2 move = Vector2.down * verticalSpeed * Time.deltaTime;
            transform.Translate(move, Space.World);
        }
    }

    // ✅ 追加：敵が倒された時の処理
    public void DestroyEnemy()
    {
        if (isDestroyed) return;

        isDestroyed = true;

        // Killカウント追加
        if (SkillPointManager.instance != null)
        {
            SkillPointManager.instance.AddKillCount(1);
        }

        // 他に通知やエフェクトが必要ならここに追加

        Destroy(gameObject);
    }

    // 索敵範囲の可視化
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
