using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public virtual void OnHit()
    {
        FindObjectOfType<GameManager>()?.OnEnemyDefeated();
        Destroy(gameObject);
    }
}
