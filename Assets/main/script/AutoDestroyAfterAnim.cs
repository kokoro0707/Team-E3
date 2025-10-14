using UnityEngine;

public class AutoDestroyAfterAnim : MonoBehaviour
{
    public float lifeTime = 1f; // ƒAƒjƒ‚Ì’·‚³‚É‡‚í‚¹‚Ä’²®

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}

