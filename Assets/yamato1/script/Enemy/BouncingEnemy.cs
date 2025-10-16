using UnityEngine;

public class BouncingEnemy : MonoBehaviour
{
    public Vector2 velocity = new Vector2(2f, -3f);
    public float spawnScaleTime = 0.5f;
    public float rotationSpeed = 360f; // ← 回転速度追加

    private Vector2 minBounds;
    private Vector2 maxBounds;
    private Vector2 currentVelocity;
    private bool isActive = false;
    private float scaleTimer = 0f;

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
        // ✅ 常に回転（見た目だけ）
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        if (!isActive)
        {
            scaleTimer += Time.deltaTime;
            float t = Mathf.Clamp01(scaleTimer / spawnScaleTime);
            transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);

            if (t >= 0.5f)
            {
                isActive = true;
                currentVelocity = velocity;
            }

            return;
        }

        // 移動処理（回転とは無関係）
        transform.Translate(currentVelocity * Time.deltaTime, Space.World); // ← 念のためWorld指定

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