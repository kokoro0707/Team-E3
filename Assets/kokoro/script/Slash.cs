using UnityEngine;

public class Slash : MonoBehaviour
{
    public float lifeTime = 0.3f;
    public int damage = 1;
    public Transform target;          // 追従先（プレイヤー）
    public Vector3 offset = new Vector3(0, 1.0f, 0); // プレイヤーの上あたりに出す

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void LateUpdate()
    {
        // プレイヤーを追従（親子関係なし）
        if (target != null)
        {
            transform.position = target.position + offset;
        }
    }

}
