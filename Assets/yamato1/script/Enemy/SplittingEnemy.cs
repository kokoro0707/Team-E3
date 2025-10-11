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

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) rb.gravityScale = 0f;

        float angleDeg = Random.Range(30f, 60f);
        if (Random.value < 0.5f) angleDeg = 180f - angleDeg;

        float angleRad = angleDeg * Mathf.Deg2Rad;
        direction = new Vector3(Mathf.Cos(angleRad), -Mathf.Sin(angleRad), 0f).normalized;
    }

    void Update()
    {
        if (hasSplit) return;

        transform.position += direction * speed * Time.deltaTime;

        float centerY = Camera.main.transform.position.y;
        if (!hasSplit && Mathf.Abs(transform.position.y - centerY) < 0.5f)
        {
            hasSplit = true;
            StartCoroutine(SplitJump());
        }

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
        GameObject leftClone = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        GameObject rightClone = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

        SplittingEnemy leftScript = leftClone.GetComponent<SplittingEnemy>();
        SplittingEnemy rightScript = rightClone.GetComponent<SplittingEnemy>();

        if (leftScript != null)
        {
            leftScript.hasSplit = true;
            Vector3 leftOffset = new Vector3(-splitOffsetX, jumpOffsetY, 0);
            leftScript.StartCoroutine(leftScript.JumpThenFall(leftClone, leftOffset));
        }

        if (rightScript != null)
        {
            rightScript.hasSplit = true;
            Vector3 rightOffset = new Vector3(splitOffsetX, jumpOffsetY, 0);
            rightScript.StartCoroutine(rightScript.JumpThenFall(rightClone, rightOffset));
        }

        Destroy(gameObject);
        yield return null;
    }

    public IEnumerator JumpThenFall(GameObject target, Vector3 offset)
    {
        Vector3 start = target.transform.position;
        Vector3 end = start + offset;
        Vector3 mid = (start + end) / 2 + new Vector3(0, jumpHeight, 0); // 弧の頂点を上に

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / arcDuration;
            Vector3 p1 = Vector3.Lerp(start, mid, t);
            Vector3 p2 = Vector3.Lerp(mid, end, t);
            target.transform.position = Vector3.Lerp(p1, p2, t);
            yield return null;
        }

        // ジャンプ完了 → その方向に落下開始
        Vector3 fallDirection = new Vector3(offset.x, -Mathf.Abs(offset.y), 0).normalized;
        float fallSpeed = speed;

        while (true)
        {
            target.transform.position += fallDirection * fallSpeed * Time.deltaTime;

            // 画面端で反射（任意）
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


    public void OnHit()
    {
        Destroy(gameObject);
        FindObjectOfType<StageManager>()?.OnEnemyDestroyed();
    }
}