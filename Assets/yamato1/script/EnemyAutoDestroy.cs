using UnityEngine;

public class EnemyAutoDestroy : MonoBehaviour
{
    public float destroyY = -10f; // 画面下の境界（カメラに合わせて調整）

    void Update()
    {
        if (transform.position.y < destroyY)
        {
            Debug.Log("敵を消します：" + gameObject.name);

            enemyziguzagu zig = GetComponent<enemyziguzagu>();
            if (zig != null) zig.MarkForDestruction();

            ZigZagSpawner.Unregister(gameObject);
            Destroy(gameObject);
        }
    }
}
