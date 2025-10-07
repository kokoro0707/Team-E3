using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    public float detectionRadius = 5f; // 索敵範囲（何m以内にプレイヤーがいたら追尾を始めるか）
    public float speed = 4f;           // プレイヤーへの追尾速度
    public float verticalSpeed = 2f;   // 索敵外のときに落ちる速度（重力っぽい）

    private Transform player;          // プレイヤーのTransform（位置）
    private bool isChasing = false;    // 現在追尾中かどうか

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
        // プレイヤーが見つからなければ何もしない
        if (player == null) return;

        // プレイヤーとの距離を測る
        float distance = Vector2.Distance(transform.position, player.position);

        // 一定距離以内なら追尾を開始
        if (distance <= detectionRadius)
        {
            isChasing = true;
        }

        // 追尾しているとき
        if (isChasing)
        {
            // プレイヤー方向を計算（正規化で方向だけ取り出す）
            Vector2 direction = (player.position - transform.position).normalized;

            // その方向へ移動
            transform.Translate(direction * speed * Time.deltaTime);
        }
        else
        {
            // まだ追尾してないときは下に落ちる
            Vector2 move = Vector2.down * verticalSpeed * Time.deltaTime;
            transform.Translate(move);
        }
    }

    // シーンビューで索敵範囲を可視化（ゲーム中には見えない）
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
