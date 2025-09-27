using UnityEngine;

public class SplittingEnemy : MonoBehaviour
{
    public GameObject splitPrefab;
    public float splitSpeed = 4f;
    public int splitCount = 0; // 初期は0

    private Vector2 velocity;

    void Start()
    {
        float vx = Random.Range(-2f, 2f);
        float vy = Random.Range(-3f, -1f);
        velocity = new Vector2(vx, vy);
    }

    void Update()
    {
        transform.Translate(velocity * Time.deltaTime);

        Vector2 min = Camera.main.ViewportToWorldPoint(new Vector2(0f, 0f));
        Vector2 max = Camera.main.ViewportToWorldPoint(new Vector2(1f, 1f));
        Vector3 pos = transform.position;

        if (pos.x < min.x || pos.x > max.x) velocity.x *= -1;
        if (pos.y < min.y || pos.y > max.y) velocity.y *= -1;
    }

    public void SetVelocity(Vector2 v)
    {
        velocity = v;
    }


    public void OnHit()
    {
        if (splitCount == 0)
        {
            Split();
        }
        else
        {
            Destroy(gameObject);
            FindObjectOfType<StageManager>().OnEnemyDestroyed();
        }
    }

    void Split()
    {
        Vector3 pos = transform.position;

        GameObject left = Instantiate(splitPrefab, pos, Quaternion.identity);
        GameObject right = Instantiate(splitPrefab, pos, Quaternion.identity);

        SplittingEnemy e1 = left.GetComponent<SplittingEnemy>();
        SplittingEnemy e2 = right.GetComponent<SplittingEnemy>();

        if (e1 != null)
        {
            e1.splitCount = 1;
            float vy = Random.Range(splitSpeed, splitSpeed + 2f);
            e1.SetVelocity(new Vector2(-splitSpeed, vy)); // 左上
        }

        if (e2 != null)
        {
            e2.splitCount = 1;
            float vy = Random.Range(splitSpeed, splitSpeed + 2f);
            e2.SetVelocity(new Vector2(splitSpeed, vy)); // 右上
        }


        Destroy(gameObject);
    }
}
