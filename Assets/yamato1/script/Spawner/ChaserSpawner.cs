using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
public class ChaserSpawner : MonoBehaviour
{
    public GameObject chaserPrefab;
    public float interval = 4f;
    private float timer = 0f;

    private List<GameObject> activeEnemies = new List<GameObject>();
    public static ChaserSpawner Instance;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            SpawnChaser();
            timer = 0f;
        }

        activeEnemies.RemoveAll(e => e == null);
    }

    void SpawnChaser()
    {
        if (chaserPrefab == null) return;

        float x = Random.Range(-10f, 10f);
        float y = Random.Range(8f, 12f); // âÊñ è„ïîÇ©ÇÁèoåª
        Vector3 pos = new Vector3(x, y, 0f);

        GameObject enemy = Instantiate(chaserPrefab, pos, Quaternion.identity);
        activeEnemies.Add(enemy);
    }

    public static void Unregister(GameObject enemy)
    {
        if (Instance != null && Instance.activeEnemies.Contains(enemy))
        {
            Instance.activeEnemies.Remove(enemy);
        }
    }
}
