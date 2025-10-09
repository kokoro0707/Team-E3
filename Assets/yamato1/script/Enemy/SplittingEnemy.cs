using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SplittingEnemy : MonoBehaviour
{
    public GameObject splitPrefab;   // 分裂後の敵プレハブ
    public int splitCount = 0;       // 0なら分裂可能、1なら分裂済み

    [Header("移動設定（分裂前）")]
    public float moveSpeed = 3f;     // 分裂前の移動速度
    private Rigidbody2D rb;

    [Header("分裂時の初速度")]
    public float splitUpSpeed = 4f;    // 分裂時の上方向の速度（半分にしたいなら調整）
    public float splitSideSpeed = 2f;  // 分裂時の左右方向の速度（半分にしたいなら調整）

    private Vector2 moveDirection;

    private Vector2 minBounds;
    private Vector2 maxBounds;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // カメラ端を取得
        Camera cam = Camera.main;
        minBounds = cam.ViewportToWorldPoint(Vector2.zero);
        maxBounds = cam.ViewportToWorldPoint(Vector2.one);

        if (splitCount == 0)
        {
            // 最初は斜め下方向（ランダムに左か右）
            float horizontal = Random.value < 0.5f ? -1f : 1f;
            moveDirection = new Vector2(horizontal, -1f).normalized;
            rb.linearVelocity = moveDirection * moveSpeed;

            // 重力はオフ（一定速度で動く）
            rb.gravityScale = 0f;
        }
        else
        {
            // 分裂後は重力有効、velocityは分裂時にセットされる想定
            rb.gravityScale = 0.5f;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            // X成分だけ反転してバウンドさせる
            Vector2 vel = rb.linearVelocity;
            vel.x = -vel.x;
            rb.linearVelocity = vel;
        }
    }

    void Update()
    {
        Vector3 pos = transform.position;

        // 壁（画面左右）にあたったら反射（X方向速度を反転）
        if (pos.x <= minBounds.x && rb.linearVelocity.x < 0f)
        {
            rb.linearVelocity = new Vector2(-rb.linearVelocity.x, rb.linearVelocity.y);
            transform.position = new Vector3(minBounds.x, pos.y, pos.z);
        }
        else if (pos.x >= maxBounds.x && rb.linearVelocity.x > 0f)
        {
            rb.linearVelocity = new Vector2(-rb.linearVelocity.x, rb.linearVelocity.y);
            transform.position = new Vector3(maxBounds.x, pos.y, pos.z);
        }

        // 画面外に下に落ちたら削除
        if (pos.y < minBounds.y - 2f)
        {
            Destroy(gameObject);
        }
    }

    // 攻撃された時に呼ばれる
    public void OnHit()
    {
        if (splitCount == 0)
        {
            Split();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 分裂処理
    void Split()
    {
        Vector3 pos = transform.position;

        GameObject left = Instantiate(splitPrefab, pos, Quaternion.identity);
        GameObject right = Instantiate(splitPrefab, pos, Quaternion.identity);

        SetSplitVelocity(left, -1);
        SetSplitVelocity(right, 1);

        Destroy(gameObject);
    }

    void SetSplitVelocity(GameObject obj, int direction)
    {
        SplittingEnemy enemy = obj.GetComponent<SplittingEnemy>();
        Rigidbody2D enemyRb = obj.GetComponent<Rigidbody2D>();
        if (enemy != null && enemyRb != null)
        {
            enemy.splitCount = 1;
            enemyRb.gravityScale = 0.03f;

            // 上方向速度を1.5倍にアップ！
            float newUpSpeed = splitUpSpeed * 1.5f;

            enemyRb.linearVelocity = new Vector2(direction * splitSideSpeed, newUpSpeed);
        }
    }

}
