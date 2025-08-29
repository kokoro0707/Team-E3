using UnityEngine;

public class Slash : MonoBehaviour
{
    public float lifeTime = 0.3f;
    public int damage = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject,lifeTime);
    }

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            Debug.Log("“G‚Éƒqƒbƒg");
        }
    }
}
