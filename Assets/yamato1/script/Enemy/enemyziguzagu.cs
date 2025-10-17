using UnityEngine;

public class enemyziguzagu : MonoBehaviour
{
    public float verticalSpeed = 1f;
    public float horizontalAmplitude = 10f;
    public float horizontalFrequency = 2f;

    private float startX;
    private bool isDestroyed = false;

    void Start()
    {
        startX = transform.position.x;
    }

    void Update()
    {
        // 回転処理
        float rotationSpeed = 180f; // 1秒で180度回転
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        if (isDestroyed) return;

        // ジグザグ移動
        float x = startX + Mathf.Sin(Time.time * horizontalFrequency) * horizontalAmplitude;
        float y = transform.position.y - verticalSpeed * Time.deltaTime;
        transform.position = new Vector2(x, y);
    }

    // 外部から破壊予約
    public void MarkForDestruction()
    {
        isDestroyed = true;
    }

    // 実際に破壊する処理
    public void DestroyEnemy()
    {
        if (isDestroyed) return;

        isDestroyed = true;

        // 敵をスポーナーから解除
        ZigZagSpawner.Unregister(gameObject);

        // Killカウント追加（← ここが今回追加したい処理）
        if (SkillPointManager.instance != null)
        {
            SkillPointManager.instance.AddKillCount(1);
        }

        // ステージマネージャーに通知
        StageManager01 manager = FindObjectOfType<StageManager01>();
        if (manager != null)
        {
            manager.OnEnemyDestroyed();
        }

        // 自分自身を削除
        Destroy(gameObject);
    }
}
