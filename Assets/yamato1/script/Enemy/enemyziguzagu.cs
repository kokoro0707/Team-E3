using UnityEngine;

public class enemyziguzagu : MonoBehaviour
{
    public float verticalSpeed = 1f;
    public float horizontalAmplitude = 10f;
    public float horizontalFrequency = 2f;

    private float startX;
    private bool isDestroyed = false;

    void Start()
    {
        startX = transform.position.x;
    }

    void Update()
    {
        if (isDestroyed) return;

        float x = startX + Mathf.Sin(Time.time * horizontalFrequency) * horizontalAmplitude;
        float y = transform.position.y - verticalSpeed * Time.deltaTime;
        transform.position = new Vector2(x, y);
    }

    public void MarkForDestruction()
    {
        isDestroyed = true;
    }
}
