using UnityEngine;
using UnityEngine.UIElements;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerClr : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float maxAngularSpeed = 360f;//��b�Ԃɉ���]��
    public float angularAccel = 720f;//�؂�ւ��X�s�[�h  
    public float jumpForce = 7f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;

    [Header("攻撃設定")]
    public GameObject slashPrehab;
    public Transform attackSpawn;

    private Rigidbody2D rb;
    private float targetAngularVel = 0f;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    bool CheckGrounded()
    {
        Bounds bounds = GetComponent<Collider2D>().bounds;
        float extraHeight = 0.1f;

        RaycastHit2D hit = Physics2D.BoxCast(bounds.center, bounds.size, 0f, Vector2.down, extraHeight, groundLayer);

        return hit.collider != null;
    }


    void Update()
    {
        float inputX = Input.GetAxisRaw("Horizontal"); // -1 or 0 or +1


        Vector2 v = rb.linearVelocity;
        v.x = inputX * moveSpeed;
        rb.linearVelocity = v;


        targetAngularVel = -inputX * maxAngularSpeed;


        float currentAV = rb.angularVelocity;


        float newAV = Mathf.MoveTowards(
            currentAV,
            targetAngularVel,
            angularAccel * Time.deltaTime
        );

        rb.angularVelocity = newAV;

        isGrounded = CheckGrounded();

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        if(Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
    }
    void Attack()
    {
        if (slashPrehab != null)
        {
            // 斬撃生成（プレイヤーの上に出す）
            Vector3 spawnPos = transform.position + Vector3.up * 1.0f; // プレイヤーの真上1.0の位置（調整可）
            GameObject slash = Instantiate(slashPrehab, spawnPos, Quaternion.identity);

            // プレイヤーを親にして、位置だけ固定で追従
            slash.transform.SetParent(transform);

            // 親の回転の影響を受けないようにする
            slash.transform.rotation = Quaternion.identity;
        }
    }
    void OnDrawGizmosSelected()
    {
           if (groundCheck != null)
           {
               Gizmos.color = Color.red;
               Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
           }
        }
    }