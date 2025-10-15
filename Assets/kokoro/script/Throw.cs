using UnityEngine;

public class ThrowSlash : MonoBehaviour
{
    public float speed = 12f;       // 飛ぶスピード
    public float lifeTime = 2f;     // 消えるまでの時間
    public int damage = 1;          // ダメージ量

    private Vector2 direction;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    void Update()
    {
        // Rigidbody を使わずにまっすぐ移動
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy e = other.GetComponent<Enemy>();
            if (e != null)
            {
                e.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
        else if (!other.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}


