using System;
using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerClr : MonoBehaviour
{
    [Header("プレイヤー設定")]
    public float hp = 1;
    public float moveSpeed = 5f;
    public float maxAngularSpeed = 360f;//��b�Ԃɉ���]��
    public float angularAccel = 720f;//�؂�ւ��X�s�[�h  
    public float jumpForce = 7f;
    public LayerMask groundLayer;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    private bool Invncble=false;
    [Header("シールド")]
    public  GameObject targetSprite;
    [SerializeField] private Sprite Nomal;
    [SerializeField] private Sprite Shield;
    [SerializeField] private Sprite Shield2;
    public ParticleSystem Shieldeffect;

    [Header("攻撃設定")]
    public GameObject slashPrehab;
    public Transform attackSpawn;

    [Header("必殺技")]
    public GameObject specialSlashPrefab;
    public float specialDuration = 1.0f; // 表示時間
    public ScreenFade screenFade;  // インスペクタで黒パネルのスクリプトを設定する

    private Rigidbody2D rb;
    private Collider2D Col;
    private float targetAngularVel = 0f;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Col = GetComponent<Collider2D>();
        SkillManager.instance.OnSkillLearned += ApplySkill;
        hp += 1;
    }

    private void ApplySkill(SkillType skillType)
    {
        if (skillType == SkillType.Shield)
        {
            hp += 1;
            var spriteRenderer = targetSprite.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = Shield;
        }
        if (skillType == SkillType.Shield2)
        {
            hp += 1;
            var spriteRenderer = targetSprite.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = Shield2;
        }
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

        if (Input.GetButtonDown("Fire1"))
        {
            Attack();
        }
        if (Input.GetButtonDown("Fire2"))
        {
            StartCoroutine(DoSpecialAttack());
        }
    }
    IEnumerator DoSpecialAttack()
    {
        // 1. 画面暗転
        yield return StartCoroutine(screenFade.FadeIn());

        // 2. 必殺技オブジェクトをカメラ中央に表示
        GameObject slash = Instantiate(
            specialSlashPrefab,
            Camera.main.transform.position + new Vector3(0, 0, 10f), // カメラの前面
            Quaternion.identity
        );
        //ダメージ
        DamageAllEnemiesOnScreen();

        // 3. 黒背景のままエフェクトを見せる
        yield return new WaitForSeconds(specialDuration);

        Destroy(slash);

        // 4. フェードアウトで元の画面に戻る
        yield return StartCoroutine(screenFade.FadeOut());
    }
    void DamageAllEnemiesOnScreen(int damage = 999)
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        Camera cam = Camera.main;

        float height = cam.orthographicSize * 2f;
        float width = height * cam.aspect;
        Rect screenRect = new Rect(
            cam.transform.position.x - width / 2f,
            cam.transform.position.y - height / 2f,
            width,
            height
        );

        foreach (var e in enemies)
        {
            if (e == null) continue;
            Vector2 pos = e.transform.position;
            if (screenRect.Contains(pos))
            {
                e.TakeDamage(damage); // ここで Enemy の TakeDamage が呼ばれる
            }
        }
    }
    void Attack()
    {
        if (slashPrehab != null)
        {
            // 斬撃生成（プレイヤーの上に出す）
            Vector3 spawnPos = transform.position + Vector3.up * 1.5f; // プレイヤーの真上1.0の位置（調整可）
            GameObject slash = Instantiate(slashPrehab, spawnPos, Quaternion.identity);

            // 追従設定
            Slash slashScript = slash.GetComponent<Slash>();
            slashScript.target = transform;                // プレイヤーを追従対象に設定
            slashScript.offset = new Vector3(0, 1.0f, 0);  // 高さ調整
        }
    }
    //エフェクト追従
    private void PlayShieldEffect()
    {
        if (Shieldeffect != null)
        {
            // エフェクトを生成
             ParticleSystem efect= Instantiate(Shieldeffect, transform.position, Quaternion.identity);

            // プレイヤーの子にして追従させる
            efect.transform.SetParent(transform);

            // 位置をリセットしてプレイヤーの中心に固定
            efect.transform.localPosition = Vector3.zero;
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
    // ===== プレイヤーダメージ判定 =====
    void OnTriggerEnter2D(Collider2D other)
    {
        if (Invncble) return;

        if (other.CompareTag("Saw")) return;

        if (other.CompareTag("Enemy")) 
        {
            Debug.Log("atat");
            TakeDamage(1); // 通常攻撃のダメージを 1 とする
        }
    }

    // ===== ダメージ処理 =====
    public void TakeDamage(int damage)
    {
        if (Invncble) return; // ← これを追加！

        hp -= damage;

        var spriteRenderer = targetSprite.GetComponent<SpriteRenderer>();
        if (hp == 2)
        {
            spriteRenderer.sprite = Shield;
            PlayShieldEffect();
        }
        else if (hp == 1)
        {
            spriteRenderer.sprite = Nomal;
            PlayShieldEffect();
        }
        else if (hp <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(InvncbleCoroutine());
    }
    private IEnumerator InvncbleCoroutine()
    {
        Debug.Log("無敵");
        Invncble = true;
        Col.enabled = false;

        SpriteRenderer sr = targetSprite.GetComponent<SpriteRenderer>();
        Color originalColor = sr.color;

        float duration = 1.5f; // 無敵時間
        float blinkInterval = 0.1f; // 点滅間隔
        float timer = 0f;

        while (timer < duration)
        {
            // 表示ON/OFFを交互に切り替え
            sr.enabled = !sr.enabled;

            yield return new WaitForSeconds(blinkInterval);
            timer += blinkInterval;
        }

        // 最終的に表示を戻す
        sr.enabled = true;
        sr.color = originalColor;

        Col.enabled = true;
        Invncble = false;
    }

    void Die()
    {
        // ここで死亡エフェクトやスコア加算もできる
        Destroy(gameObject);
    }

}