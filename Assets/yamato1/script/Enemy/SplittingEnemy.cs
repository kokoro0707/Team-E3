using System.Collections;
using UnityEngine;

public class SplittingEnemy : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float speed = 3f;
    public float jumpHeight = 2f;
    public float jumpOffsetY = 3f;
    public float arcDuration = 0.5f;
    public float splitOffsetX = 1.5f;

    private Vector3 direction;
    private bool hasSplit = false;
    private bool isSplittingEnemy = false;  // 分裂後の敵かどうか判定

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.gravityScale = 0f;

        // 分裂後の敵は移動方向を固定（左右に動かすなど）
        if (!isSplittingEnemy)
        {
            float angleDeg = Random.Range(30f, 60f);
            if (Random.value < 0.5f) angleDeg = 180f - angleDeg;

            float angleRad = angleDeg * Mathf.Deg2Rad;
            direction = new Vector3(Mathf.Cos(angleRad), -Mathf.Sin(angleRad), 0f).normalized;
        }
        else
        {
            direction = Vector3.right;  // 分裂後は右向きに移動開始
        }
    }

    void Update()
    {
        // 常に回転（分裂前・後ともに回転）
        float rotationSpeed = 360f;
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        // 分裂後はJumpThenFallコルーチンで動くのでUpdateでの移動はしない
        if (isSplittingEnemy) return;

        // 分裂前の移動処理
        transform.position += direction * speed * Time.deltaTime;

        float centerY = Camera.main.transform.position.y;
        if (!hasSplit && Mathf.Abs(transform.position.y - centerY) < 0.5f)
        {
            hasSplit = true;
            StartCoroutine(SplitJump());
        }

        // 画面端で移動方向反転
        float camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
        float leftEdge = Camera.main.transform.position.x - camHalfWidth;
        float rightEdge = Camera.main.transform.position.x + camHalfWidth;

        if (transform.position.x < leftEdge || transform.position.x > rightEdge)
        {
            direction.x *= -1f;
        }
    }

    IEnumerator SplitJump()
    {
        // 分裂元の敵を消す（これ重要！）
        Destroy(gameObject);

        // 分裂後の敵を2体生成
        GameObject leftClone = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        GameObject rightClone = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

        // Layerやタグが「Enemy」になっているかチェック（念のためセット）
        leftClone.layer = LayerMask.NameToLayer("Enemy");
        rightClone.layer = LayerMask.NameToLayer("Enemy");
        leftClone.tag = "Enemy";
        rightClone.tag = "Enemy";

        SplittingEnemy leftScript = leftClone.GetComponent<SplittingEnemy>();
        SplittingEnemy rightScript = rightClone.GetComponent<SplittingEnemy>();

        if (leftScript != null)
        {
            leftScript.isSplittingEnemy = true;  // 分裂後の敵であることをセット
            Vector3 leftOffset = new Vector3(-splitOffsetX, jumpOffsetY, 0);
            leftScript.StartCoroutine(leftScript.JumpThenFall(leftClone, leftOffset));
        }

        if (rightScript != null)
        {
            rightScript.isSplittingEnemy = true;
            Vector3 rightOffset = new Vector3(splitOffsetX, jumpOffsetY, 0);
            rightScript.StartCoroutine(rightScript.JumpThenFall(rightClone, rightOffset));
        }

        // ステージ管理に敵数増加を通知
        var stageManager = FindObjectOfType<StageManager01>();
        if (stageManager != null)
        {
            stageManager.AddEnemies(2);
        }

        yield return null;
    }

    public IEnumerator JumpThenFall(GameObject target, Vector3 offset)
    {
        Vector3 start = target.transform.position;
        Vector3 end = start + offset;
        Vector3 mid = (start + end) / 2 + new Vector3(0, jumpHeight, 0);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / arcDuration;
            Vector3 p1 = Vector3.Lerp(start, mid, t);
            Vector3 p2 = Vector3.Lerp(mid, end, t);
            target.transform.position = Vector3.Lerp(p1, p2, t);
            yield return null;
        }

        Vector3 fallDirection = new Vector3(offset.x, -Mathf.Abs(offset.y), 0).normalized;
        float fallSpeed = speed;

        while (true)
        {
            target.transform.position += fallDirection * fallSpeed * Time.deltaTime;

            float camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
            float leftEdge = Camera.main.transform.position.x - camHalfWidth;
            float rightEdge = Camera.main.transform.position.x + camHalfWidth;

            if (target.transform.position.x < leftEdge || target.transform.position.x > rightEdge)
            {
                fallDirection.x *= -1f;
            }

            yield return null;
        }
    }

    // 敵が攻撃された時に呼ばれる
    public void OnHit()
    {
        Destroy(gameObject);
        FindObjectOfType<StageManager01>()?.OnEnemyDestroyed();
    }
}
