using UnityEngine;

public class SplitSpawner : MonoBehaviour
{
    public GameObject splitEnemyPrefab;
    public float interval = 3f;
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            SpawnFromAbove();
            timer = 0f;
        }
    }

    void SpawnFromAbove()
    {
        if (splitEnemyPrefab == null) return;

        float camX = Camera.main.transform.position.x;
        float camY = Camera.main.transform.position.y;
        float orthoSize = Camera.main.orthographicSize;
        float aspect = Camera.main.aspect;

        float camHalfWidth = orthoSize * aspect;
        float camHalfHeight = orthoSize;

        float leftX = camX - camHalfWidth;
        float rightX = camX + camHalfWidth;
        float topY = camY + camHalfHeight;

        float spawnX = Random.Range(leftX, rightX);
        float spawnY = topY + 1f;

        Vector3 spawnPos = new Vector3(spawnX, spawnY, 0f);
        Instantiate(splitEnemyPrefab, spawnPos, Quaternion.identity);
    }
}