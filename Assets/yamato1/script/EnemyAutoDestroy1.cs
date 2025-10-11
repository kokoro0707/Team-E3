using UnityEngine;

public class EnemyAutoDetroy1 : MonoBehaviour
{
    public float destroyX = 16f; // 画面外のX範囲（カメラサイズに合わせて調整）

    void Update()
    {
        if (Mathf.Abs(transform.position.x) > destroyX)
        {
           
            Destroy(gameObject);
        }
    }
}
