using UnityEngine;

public class BouncingEnemy : MonoBehaviour
{
    public Vector2 velocity = new Vector2(2f, -3f); // ← Inspectorで設定 or スポーナーから渡す

    private Vector2 minBounds;
    private Vector2 maxBounds;

    void Start()
    {
        Camera cam = Camera.main;
        minBounds = cam.ViewportToWorldPoint(new Vector2(0f, 0f));
        maxBounds = cam.ViewportToWorldPoint(new Vector2(1f, 1f));

        // 念のため、Y方向がゼロなら強制的に下向きにする
        if (Mathf.Abs(velocity.y) < 0.1f)
        {
            velocity.y = -3f;
        }
    }

    void Update()
    {
        transform.Translate(velocity * Time.deltaTime);

        Vector3 pos = transform.position;

        if (pos.x < minBounds.x || pos.x > maxBounds.x)
            velocity.x *= -1;

        if (pos.y < minBounds.y || pos.y > maxBounds.y)
            velocity.y *= -1;
    }
}