using UnityEngine;

public class Slash : MonoBehaviour
{
    [Header("基本設定")]
    public float lifeTime = 0.3f;
    public int damage = 1;

    [Header("追従設定")]
    public Transform target;
    public Vector3 offset = new Vector3(0, 1.0f, 0);

    [Header("スキル強化")]
    public bool isPoweredUp = false;
    public float rangeMultiplier = 1.0f;

    void Start()
    {
        // 🔹 サイズをスキルレベルに応じて拡大
        transform.localScale *= rangeMultiplier;

        // 🔹 当たり判定の拡大
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            if (col is CircleCollider2D circle)
                circle.radius *= rangeMultiplier;
            else if (col is BoxCollider2D box)
                box.size *= rangeMultiplier;
        }

        // 🔹 強化エフェクトの色変更
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            if (isPoweredUp)
                sr.color = new Color(1f, 0.5f, 0.2f, 1f); // 強化時 → オレンジ
            else
                sr.color = Color.white;
        }

        Destroy(gameObject, lifeTime);
    }

    void LateUpdate()
    {
        // プレイヤーを追従
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 🔹 敵にダメージ
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
