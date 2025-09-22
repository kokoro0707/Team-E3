using UnityEngine;

public class ZigZagSpawner : MonoBehaviour
{
    public GameObject zigZagPrefab;
    public float interval = 3f;
    private float timer;

    internal static void Unregister(GameObject gameObject)
    {
        throw new System.NotImplementedException();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            float x = Random.Range(-10f, 10f);
            Vector3 pos = new Vector3(x, 10f, 0f);
            Instantiate(zigZagPrefab, pos, Quaternion.identity);
            timer = 0f;
        }
    }
}