using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("敵ステータス")]
    public int maxHP = 1;
    private int currentHP;

    void Start()
    {
        currentHP = maxHP;
    }

    // ===== 通常攻撃などの当たり判定 =====
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Saw")) // 斬撃や攻撃エフェクトに "Saw" タグをつける
        {
            Debug.Log("itata");
            TakeDamage(1); // 通常攻撃のダメージを 1 とする
        }
    }

    // ===== ダメージ処理 =====
    public void TakeDamage(int damage)
    {
        currentHP -= damage;

        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // ここで死亡エフェクトやスコア加算もできる
        Destroy(gameObject);
    }
}
