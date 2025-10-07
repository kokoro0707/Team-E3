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

    // 攻撃（Saw）に当たったらダメージ
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Saw"))
        {
            Debug.Log("敵に攻撃がヒット");

            TakeDamage(1);
        }
    }

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
        Destroy(gameObject);
    }
}