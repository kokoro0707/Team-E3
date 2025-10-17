using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public float detectionRadius = 5f;

    [Header("追尾の速度")]
    public float minShootSpeed = 2f;
    public float maxShootSpeed = 8f;
    public float shootAcceleration = 3f;

    [Header("ジャンプ（上昇）の設定")]
    public float jumpInitialSpeed = 15f;
    public float gravity = 20f;

    [Header("挙動タイミング")]
    public float cooldownTime = 1.5f;
    public float stopDuration = 2f;

    private Transform player;
    private Rigidbody2D rb;

    private enum State
    {
        Idle,
        JumpingUp,
        Attacking,
        Stopped
    }

    private State currentState = State.Idle;

    private float cooldownTimer = 0f;
    private float stopTimer = 0f;
    private float currentShootSpeed;
    private float verticalVelocity;
    private bool isDestroyed = false; // ← 追加

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        rb = GetComponent<Rigidbody2D>();

        cooldownTimer = cooldownTime;
        currentShootSpeed = minShootSpeed;
        verticalVelocity = 0f;
    }

    void Update()
    {
        if (isDestroyed) return;

        if (player == null || rb == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Idle:
                rb.linearVelocity = Vector2.zero;
                currentShootSpeed = minShootSpeed;

                if (distanceToPlayer <= detectionRadius)
                {
                    currentState = State.JumpingUp;
                    verticalVelocity = jumpInitialSpeed;
                }
                break;

            case State.JumpingUp:
                verticalVelocity -= gravity * Time.deltaTime;
                rb.linearVelocity = new Vector2(0, verticalVelocity);

                if (verticalVelocity <= 0f)
                {
                    currentState = State.Attacking;
                    cooldownTimer = 0f;
                    currentShootSpeed = minShootSpeed;
                }
                break;

            case State.Attacking:
                cooldownTimer -= Time.deltaTime;

                if (distanceToPlayer > detectionRadius)
                {
                    currentState = State.Idle;
                    rb.linearVelocity = Vector2.zero;
                    break;
                }

                currentShootSpeed += shootAcceleration * Time.deltaTime;
                currentShootSpeed = Mathf.Min(currentShootSpeed, maxShootSpeed);

                if (cooldownTimer <= 0f)
                {
                    Vector2 direction = (player.position - transform.position).normalized;

                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

                    rb.linearVelocity = direction * currentShootSpeed;

                    cooldownTimer = cooldownTime;
                }

                rb.linearVelocity += Vector2.down * gravity * 0.05f * Time.deltaTime;
                break;

            case State.Stopped:
                rb.linearVelocity = Vector2.zero;
                stopTimer -= Time.deltaTime;

                if (stopTimer <= 0f)
                {
                    currentState = State.Idle;
                }
                break;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy")) return;
        if (currentState == State.JumpingUp) return;

        currentState = State.Stopped;
        rb.linearVelocity = Vector2.zero;
        stopTimer = stopDuration;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    // ✅ 追加：敵が倒されたときに呼ぶ処理
    public void DestroyEnemy()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        // Killカウント加算
        if (SkillPointManager.instance != null)
        {
            SkillPointManager.instance.AddKillCount(1);
        }

        // 必要なら他の処理もここに（爆発、ドロップ、通知など）

        Destroy(gameObject);
    }
}
