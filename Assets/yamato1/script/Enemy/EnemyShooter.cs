using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public float detectionRadius = 5f;

    [Header("追尾の速度")]
    public float minShootSpeed = 2f;        // 最低速度
    public float maxShootSpeed = 8f;        // 最大速度
    public float shootAcceleration = 3f;    // 加速度

    [Header("ジャンプ（上昇）の設定")]
    public float jumpInitialSpeed = 15f;    // ジャンプ初速
    public float gravity = 20f;             // 重力相当

    [Header("挙動タイミング")]
    public float cooldownTime = 1.5f;       // クールタイム（突進間隔）
    public float stopDuration = 2f;         // 停止時間

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

                // 弧を描く演出として少し下方向に力を加える
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
        // 同じ敵同士では反応しない
        if (collision.gameObject.CompareTag("Enemy")) return;

        // ジャンプ中の衝突は無視（地面にぶつかっても止まらない）
        if (currentState == State.JumpingUp) return;

        // 攻撃中にプレイヤーなどに衝突したら一時停止
        currentState = State.Stopped;
        rb.linearVelocity = Vector2.zero;
        stopTimer = stopDuration;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
