using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    public float detectionRadius = 5f; // 索敵範囲（何m以内にプレイヤーがいたら追尾を始めるか）
    public float speed = 4f;           // プレイヤーへの追尾速度
    public float verticalSpeed = 2f;   // 索敵外のときに落ちる速度（重力っぽい）

    private Transform player;          // プレイヤーのTransform（位置）
    private bool isChasing = false;    // 現在追尾中かどうか

    public float rotationSpeed = 360f; // 1秒で1回転（360度）


    void Start()
    {
        // プレイヤーオブジェクトを探す（タグで "Player" を検索）
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

     void Update()
        {
            // ✅ 1. 常に回転（見た目だけ）
            transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

            if (player == null) return;

            float distance = Vector2.Distance(transform.position, player.position);

            if (distance <= detectionRadius)
            {
                isChasing = true;
            }

            if (isChasing)
            {
                // ✅ 2. プレイヤーへの方向を計算（ワールド座標）
                Vector2 direction = (player.position - transform.position).normalized;

                // ✅ 3. ワールド座標で移動（回転の影響を受けない）
                transform.Translate(direction * speed * Time.deltaTime, Space.World);
            }
            else
            {
                // ✅ 4. 落下もワールド座標で
                Vector2 move = Vector2.down * verticalSpeed * Time.deltaTime;
                transform.Translate(move, Space.World);
            }
        }
    

    // シーンビューで索敵範囲を可視化（ゲーム中には見えない）
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
