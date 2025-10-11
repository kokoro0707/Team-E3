using UnityEngine;
using System.Collections;

public class StraightEnemy : MonoBehaviour
{
    public Vector2 direction = Vector2.left;
    public float speed = 3f;
    public float spawnScaleTime = 0.5f;

    void Start()
    {
        // 初期サイズを 0 にして拡大開始
        transform.localScale = Vector3.zero;
        StartCoroutine(ScaleIn());
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
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
}
