using UnityEngine;

public class BouncingEnemy : MonoBehaviour
{
    public Vector2 velocity = new Vector2(2f, -3f);
    public float spawnScaleTime = 0.5f;
    public float rotationSpeed = 360f;

    private Vector2 minBounds;
    private Vector2 maxBounds;
    private Vector2 currentVelocity;
    private bool isActive = false;
    private float scaleTimer = 0f;
    private bool isDestroyed = false; // ← 追加（多重破壊防止）

    void Start()
    {
        Camera cam = Camera.main;
        minBounds = cam.ViewportToWorldPoint(new Vector2(0f, 0f));
        maxBounds = cam.ViewportToWorldPoint(new Vector2(1f, 1f));

        transform.localScale = Vector3.zero;

        if (Mathf.Abs(velocity.y) < 0.1f)
        {
            velocity.y = -3f;
        }

        currentVelocity = Vector2.zero;
    }

    void Update()
    {
        if (isDestroyed) return;

        // 回転アニメーション
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        if (!isActive)
        {
            scaleTimer += Time.deltaTime;
            float t = Mathf.Clamp01(scaleTimer / spawnScaleTime);
            float targetScale = 0.5f;  // 1/2のサイズにするため
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * targetScale, t);

            if (t >= 0.5f)
            {
                isActive = true;
                currentVelocity = velocity;
            }

            return;
        }

        // 移動
        transform.Translate(currentVelocity * Time.deltaTime, Space.World);

        Vector3 pos = transform.position;

        // 画面端で跳ね返り
        if (pos.x < minBounds.x || pos.x > maxBounds.x)
        {
            currentVelocity.x *= -1;
        }

        if (pos.y < minBounds.y || pos.y > maxBounds.y)
        {
            currentVelocity.y *= -1;
        }
    }

    // ✅ 追加：敵を破壊する処理
    public void DestroyEnemy()
    {
        if (isDestroyed) return;

        isDestroyed = true;

        // Killカウントを追加
        if (SkillPointManager.instance != null)
        {
            SkillPointManager.instance.AddKillCount(1);
        }

        // ここで他の通知やエフェクトも入れられる

        Destroy(gameObject);
    }
}
