using UnityEngine;

// プレイヤーに対して1.5秒ごとに突進を繰り返す敵の挙動スクリプト
public class EnemyShooter : MonoBehaviour
{
    // プレイヤーを検知する範囲（この距離内で追尾開始）
    public float detectionRadius = 5f;

    // 突進の速度
    public float shootSpeed = 5f;

    // カーブ演出用：下方向へ加える速度（重力っぽい）
    public float verticalSpeed = 2f;

    // 突進のクールタイム（秒）
    public float cooldownTime = 1.5f;

    // プレイヤーのTransform（位置情報）
    private Transform player;

    // Rigidbody2D（物理挙動制御）
    private Rigidbody2D rb;

    // クールダウンのタイマー（現在のカウント）
    private float cooldownTimer = 0f;

    // 初期化処理（ゲーム開始時に1回だけ実行）
    void Start()
    {
        // "Player" タグのついたオブジェクトを探す
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        // この敵のRigidbody2Dを取得
        rb = GetComponent<Rigidbody2D>();

        // 最初の突進までのクールタイムを設定
        cooldownTimer = cooldownTime;
    }

    // 毎フレーム実行される処理
    void Update()
    {
        // プレイヤーまたはRigidbodyが存在しない場合は処理をしない
        if (player == null || rb == null) return;

        // プレイヤーとの距離を計算
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // プレイヤーが検知範囲内にいるときのみ追尾
        if (distanceToPlayer <= detectionRadius)
        {
            // クールタイマーを減らす
            cooldownTimer -= Time.deltaTime;

            // クールタイムが終了したら突進
            if (cooldownTimer <= 0f)
            {
                // プレイヤーの方向を計算（ベクトルを正規化）
                Vector2 direction = (player.position - transform.position).normalized;

                // その方向へ突進速度を設定
                rb.velocity = direction * shootSpeed;

                // クールタイムをリセット
                cooldownTimer = cooldownTime;
            }

            // 突進中に少しずつ下方向に速度を加える（カーブ演出）
            rb.velocity += Vector2.down * verticalSpeed * Time.deltaTime;
        }
        else
        {
            // プレイヤーが範囲外に出たら、停止＆クールタイムをリセット
            rb.velocity = Vector2.zero;
            cooldownTimer = cooldownTime;
        }
    }

    // Unityエディタ上で検知範囲を可視化するための処理
    void OnDrawGizmosSelected()
    {
        // 線だけの円で表示
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
