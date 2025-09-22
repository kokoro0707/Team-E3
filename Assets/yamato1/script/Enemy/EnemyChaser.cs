using UnityEngine;

public class EnemyChaser : MonoBehaviour
{
    public float detectionRadius = 5f; //索敵範囲
    public float speed = 2f; //追尾速度
    public float verticalSpeed = 2f;//自由落下

    private Transform player;
    private bool isChasing = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject playerobj = GameObject.FindGameObjectWithTag("Player");
        if(playerobj != null)
        {
            player = playerobj.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= detectionRadius)
        {
            isChasing = true;
        }
        //下に徐々に落ちる
        Vector2 move = Vector2.down * verticalSpeed;
      
        if (isChasing)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            transform.Translate(direction * speed * Time.deltaTime);
        }
        transform.Translate(move * Time.deltaTime);
    }

    void OnDrawGizmosSelected()
    {
        // 索敵範囲をシーンビューで可視化（ゲーム中には表示されません）
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
