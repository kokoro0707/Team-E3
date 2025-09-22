using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public float detectionRadius = 5f;
    public float shootSpeed = 5f;
    public float verticalSpeed = 2f;
    public float cooldownTime = 2f;

    private Transform player;
    private bool hasShot = false;
    private Vector2 shootDirection;
    private float cooldownTimer = 0f;
    private Rigidbody2D rb;
    private bool isStuck = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null || rb == null || isStuck) return;

        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                hasShot = false;
                rb.linearVelocity = Vector2.zero;
            }
            return;
        }

        if (!hasShot)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance <= detectionRadius)
            {
                shootDirection = (player.position - transform.position).normalized;
                rb.linearVelocity = shootDirection * shootSpeed;
                hasShot = true;
            }
        }

        if (hasShot)
        {
            rb.linearVelocity += Vector2.down * verticalSpeed * Time.deltaTime;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasShot && collision.gameObject.CompareTag("Wall"))
        {
            // 壁にぶつかったらくっつく
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic; // 物理挙動を止める
            isStuck = true;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
