using UnityEngine;

public class Enemy : MonoBehaviour
{
    public AudioSource EnemySE;
    public AudioClip Down;


    [Header("敵ステータス")]
    public int maxHP = 1;
    private int currentHP;
    public ParticleSystem effect;

    // ✅ 一度だけカウントするためのフラグ
    private bool isCounted = false;

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
        if (Down != null) EnemySE.PlayOneShot(Down);
        

        // ✅ すでにカウント済みならスキップ
        if (isCounted) return;
        isCounted = true;

        if (SkillPointManager.instance != null)
        {
            SkillPointManager.instance.AddKillCount(1);
        }

        var manager = FindObjectOfType<StageManager01>();
        if (manager != null)
        {
            manager.OnEnemyDestroyed();
        }

        SpecialGauge.instance.AddKill();


        if (effect != null)
            Instantiate(effect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
