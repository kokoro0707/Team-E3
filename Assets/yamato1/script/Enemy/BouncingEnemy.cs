using UnityEngine;

public class BouncingEnemy : MonoBehaviour
{
    public Vector2 velocity = new Vector2(2f, -3f); // Inspectorで設定 or スポーナーから渡す
    public float spawnScaleTime = 0.5f; // 拡大にかける時間（秒）

    private Vector2 minBounds;
    private Vector2 maxBounds;
    private Vector2 currentVelocity;
    private bool isActive = false; // バウンド開始したかどうか
    private float scaleTimer = 0f;

    void Start()
    {
        // カメラのワールド座標範囲を取得
        Camera cam = Camera.main;
        minBounds = cam.ViewportToWorldPoint(new Vector2(0f, 0f));
        maxBounds = cam.ViewportToWorldPoint(new Vector2(1f, 1f));

        // 最初は見た目を0にする
        transform.localScale = Vector3.zero;

        // Y方向が0に近いなら強制的に下向きに
        if (Mathf.Abs(velocity.y) < 0.1f)
        {
            velocity.y = -3f;
        }

        // 拡大アニメーションが終わったら使うように待機
        currentVelocity = Vector2.zero;
    }

    void Update()
    {
        // 拡大アニメーション中
        if (!isActive)
        {
            scaleTimer += Time.deltaTime;
            float t = Mathf.Clamp01(scaleTimer / spawnScaleTime);
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);

            if (t >= 1f)
            {
                isActive = true;
                currentVelocity = velocity; // 拡大終了 → バウンド開始
            }

            return; // 拡大中は移動しない
        }

        // 移動処理
        transform.Translate(currentVelocity * Time.deltaTime);
        Vector3 pos = transform.position;

        if (pos.x < minBounds.x || pos.x > maxBounds.x)
        {
            currentVelocity.x *= -1;
        }

        if (pos.y < minBounds.y || pos.y > maxBounds.y)
        {
            currentVelocity.y *= -1;
        }
    }
}
