using UnityEngine;
using System.Collections;

public class StraightEnemy : MonoBehaviour
{
    public Vector2 direction = Vector2.left;
    public float speed = 3f;
    public float spawnScaleTime = 0.5f;
    public bool canMove = false; // 拡大中は動かない
    private bool isDestroyed = false; // ✅ 多重破壊防止

    void Start()
    {
        transform.localScale = Vector3.zero;
        StartCoroutine(ScaleIn());
    }

    void Update()
    {
        if (canMove)
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
    }

    IEnumerator ScaleIn()
    {
        Vector3 targetScale = Vector3.one;
        float elapsed = 0f;

        while (elapsed < spawnScaleTime)
        {
            float t = elapsed / spawnScaleTime;
            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
    }
    public void OnHit()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        // ✅ Killカウント加算
        if (SkillPointManager.instance != null)
        {
            SkillPointManager.instance.AddKillCount(1);
        }

        // ✅ EnemyCount 減らす（撃破時のみ）
        FindObjectOfType<StageManager01>()?.OnEnemyDestroyed();

        Destroy(gameObject);
    }
}
