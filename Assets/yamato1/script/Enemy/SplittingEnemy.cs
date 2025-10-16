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
    private bool isSplittingEnemy = false;

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.gravityScale = 0f;

        // 分裂後の敵は移動方向を固定
        if (!isSplittingEnemy)
        {
            float angleDeg = Random.Range(30f, 60f);
            if (Random.value < 0.5f) angleDeg = 180f - angleDeg;

            float angleRad = angleDeg * Mathf.Deg2Rad;
            direction = new Vector3(Mathf.Cos(angleRad), -Mathf.Sin(angleRad), 0f).normalized;
        }
        else
        {
            direction = Vector3.right;
        }
    }

    void Update()
    {
        // ✅ 分裂前後問わず常に回転
        float rotationSpeed = 360f;
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        if (isSplittingEnemy) return;

        // 分裂前の移動処理
        transform.position += direction * speed * Time.deltaTime;

        float centerY = Camera.main.transform.position.y;
        if (!hasSplit && Mathf.Abs(transform.position.y - centerY) < 0.5f)
        {
            hasSplit = true;
            StartCoroutine(SplitJump());
        }

        // 画面端で反射
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
        Destroy(gameObject);

        GameObject leftClone = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        GameObject rightClone = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

        // ✅ レイヤー・タグ・有効化を明示的に設定
        SetupClone(leftClone, new Vector3(-splitOffsetX, jumpOffsetY, 0));
        SetupClone(rightClone, new Vector3(splitOffsetX, jumpOffsetY, 0));

       // FindObjectOfType<StageManager01>()?.AddEnemies(2);

        yield return null;
    }

    void SetupClone(GameObject clone, Vector3 offset)
    {
        clone.layer = LayerMask.NameToLayer("Enemy");
        clone.tag = "Enemy";

        // ✅ Colliderとスクリプトを強制的に有効化
        var collider = clone.GetComponent<Collider2D>();
        if (collider != null) collider.enabled = true;

        var script = clone.GetComponent<SplittingEnemy>();
        if (script != null)
        {
            script.isSplittingEnemy = true;
            script.enabled = true;
            script.StartCoroutine(script.JumpThenFall(clone, offset));
        }
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

        while(true)
{
            target.transform.position += fallDirection * fallSpeed * Time.deltaTime;

            // 画面端で反射
            float camHalfWidth = Camera.main.orthographicSize * Camera.main.aspect;
            float leftEdge = Camera.main.transform.position.x - camHalfWidth;
            float rightEdge = Camera.main.transform.position.x + camHalfWidth;

            if (target.transform.position.x < leftEdge || target.transform.position.x > rightEdge)
            {
                fallDirection.x *= -1f;
            }

            // ✅ Y座標が -10 以下なら消す
            if (target.transform.position.y < -10f)
            {
                Destroy(target);
                yield break; // コルーチン終了
            }

            yield return null;
        }
    }

    public void OnHit()
    {
        Destroy(gameObject);
        FindObjectOfType<StageManager01>()?.OnEnemyDestroyed();
    }
}