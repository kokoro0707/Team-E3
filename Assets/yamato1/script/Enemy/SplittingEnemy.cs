using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SplittingEnemy : MonoBehaviour
{
    public GameObject splitPrefab;     // 分裂後に生成するプレハブ
    public float splitForce = 5f;      // 分裂時の弾く力
    public int splitCount = 0;         // 分裂回数（0なら分裂可能）

    public float moveSpeed = 3f;       // 初期移動速度

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // ランダムな方向に初期速度を設定（下方向に落ちる）
        float vx = Random.Range(-1f, 1f);
        float vy = Random.Range(-0.5f, -0.1f); // 下向き

        Vector2 initialVelocity = new Vector2(vx, vy).normalized * moveSpeed;
        rb.linearVelocity = initialVelocity;
    }

    void Update()
    {
        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0f, 0f));
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1f, 1f));
        Vector2 pos = transform.position;

        // 左右端に当たったら反転
        if (pos.x < min.x || pos.x > max.x)
        {
            rb.linearVelocity = new Vector2(-rb.linearVelocity.x, rb.linearVelocity.y);
        }

        // 下に落ちすぎたら削除（画面外処理）
        if (pos.y < min.y - 2f)
        {
            Destroy(gameObject);
        }
    }

    // 攻撃を受けたときに呼び出す
    public void OnHit()
    {
        if (splitCount == 0)
        {
            Split(); // 初回ヒットで分裂
        }
        else
        {
            Destroy(gameObject); // 2回目以降は消滅
            FindObjectOfType<StageManager>().OnEnemyDestroyed();
        }
    }

    // 分裂処理
    void Split()
    {
        Vector3 pos = transform.position;

        GameObject left = Instantiate(splitPrefab, pos, Quaternion.identity);
        GameObject right = Instantiate(splitPrefab, pos, Quaternion.identity);

        SplittingEnemy e1 = left.GetComponent<SplittingEnemy>();
        SplittingEnemy e2 = right.GetComponent<SplittingEnemy>();

        if (e1 != null)
        {
            e1.splitCount = 1;

            Rigidbody2D rb1 = left.GetComponent<Rigidbody2D>();
            rb1.linearVelocity = Vector2.zero;
            rb1.AddForce(new Vector2(-1f, 1f).normalized * splitForce, ForceMode2D.Impulse);
        }

        if (e2 != null)
        {
            e2.splitCount = 1;

            Rigidbody2D rb2 = right.GetComponent<Rigidbody2D>();
            rb2.linearVelocity = Vector2.zero;
            rb2.AddForce(new Vector2(1f, 1f).normalized * splitForce, ForceMode2D.Impulse);
        }

        Destroy(gameObject);
    }
}
