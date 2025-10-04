using UnityEngine;

public class EnemyShooter : MonoBehaviour
{
    public float detectionRadius = 5f;

    [Header("追尾の速度")]
    public float minShootSpeed = 2f;        // 追尾の最低速度（はじめ遅い）
    public float maxShootSpeed = 8f;        // 追尾の最大速度
    public float shootAcceleration = 3f;    // 追尾の加速度（速度が上がる速さ）

    [Header("ジャンプ（上昇）の設定")]
    public float jumpInitialSpeed = 15f;    // ジャンプ開始の初速（速い）
    public float gravity = 20f;              // ジャンプの減速（重力相当）

    public float cooldownTime = 1.5f;       // 突進のクールタイム
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

    private float currentShootSpeed;     // 今の追尾速度（徐々に増える）

    private float verticalVelocity;      // ジャンプ中の縦速度

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
                currentShootSpeed = minShootSpeed;   // 速度はリセット

                if (distanceToPlayer <= detectionRadius)
                {
                    currentState = State.JumpingUp;
                    verticalVelocity = jumpInitialSpeed; // ジャンプ初速セット
                }
                break;

            case State.JumpingUp:
                // ジャンプの上昇速度を時間経過で重力で減速
                verticalVelocity -= gravity * Time.deltaTime;

                // 縦速度を適用して移動
                rb.linearVelocity = new Vector2(0, verticalVelocity);

                // 上昇が終わったら（速度が0以下になったら）攻撃開始へ
                if (verticalVelocity <= 0f)
                {
                    currentState = State.Attacking;
                    cooldownTimer = 0f;  // 即突進開始OK
                    currentShootSpeed = minShootSpeed;  // 追尾速度リセット
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

                // 速度を加速（maxShootSpeedまで）
                currentShootSpeed += shootAcceleration * Time.deltaTime;
                currentShootSpeed = Mathf.Min(currentShootSpeed, maxShootSpeed);

                if (cooldownTimer <= 0f)
                {
                    Vector2 direction = (player.position - transform.position).normalized;

                    // 回転（頂点をプレイヤー方向に向ける）
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.Euler(0, 0, angle - 90f);

                    // 加速した速度で突進
                    rb.linearVelocity = direction * currentShootSpeed;

                    cooldownTimer = cooldownTime;
                }

                // 弧の演出で少し下に加速
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        // 衝突した相手がEnemyタグなら停止処理をしない
        if (collision.gameObject.CompareTag("Enemy")) return;

        // ジャンプ中の衝突は無視（地面に当たって止まらないように）
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
}
