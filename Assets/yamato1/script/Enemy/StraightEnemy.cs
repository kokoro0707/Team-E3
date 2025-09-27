using UnityEngine;

public class StraightEnemy : MonoBehaviour
{
    public Vector2 direction = Vector2.left;
    public float speed = 3f;

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }
}
