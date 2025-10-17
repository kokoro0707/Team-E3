using System;
using System.Collections;
using NUnit.Framework.Internal;
using UnityEngine;
using UnityEngine.Audio;
using static Unity.VisualScripting.Metadata;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerClr : MonoBehaviour
{
    [Header("サウンド")]
    public AudioSource AudioSource;
    public AudioClip AtaackSE;
    public AudioClip ChildSE;
    public AudioClip spesialSE;
    public AudioClip janpSE;
    public AudioClip dieSE;

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
    private bool canThrowAttack = false;
    private bool canThrowAttack2 = false;
    private bool canThrowAttack3 = false;
    private bool hasPowerUpSkill = false;
    public GameObject breakEffectPrefab;
    [Header("シールド")]
    public  GameObject targetSprite;
    [SerializeField] private Sprite Nomal;
    [SerializeField] private Sprite Shield;
    [SerializeField] private Sprite Shield2;
    [SerializeField] private Sprite Shield3;
    public ParticleSystem Shieldeffect;

    private bool hasShieldRepairSkill = false; // 永続スキル取得フラグ
    public float shieldRepairInterval = 5f;   // 回復間隔（秒）
    public float shieldRepairInterval2 = 3f;   // 回復間隔（秒）
    public int shieldRepairAmount = 1;        // 回復量
    private Coroutine shieldRepairCoroutine;

    [Header("攻撃設定")]
    public GameObject slashPrehab;
    public GameObject slashPrehab2;
    public Transform attackSpawn;
    private int attackRangeLevel = 0;
    private float currentRangeMultiplier = 1f;

    [Header("必殺技")]
    public GameObject specialSlashPrefab;
    public float specialDuration = 0.8f; // 表示時間
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
            if(hp==3)
            {
                var spriteRenderer = targetSprite.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = Shield2;
            }
            else
            {
                var spriteRenderer = targetSprite.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = Shield;
            }
        }
        if(skillType==SkillType.Shield3)
        {
            hp += 1;
            if (hp==4)
            {
                var spriteRenderer = targetSprite.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = Shield3;
            }
            else if(hp==3)
            {
                var spriteRenderer = targetSprite.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = Shield2;
            }
            else
            {
                var spriteRenderer = targetSprite.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = Shield;
            }
            
        }
        if(skillType==SkillType.ATTACKRANGE)
        {
            attackRangeLevel = Mathf.Max(attackRangeLevel, 1);
            currentRangeMultiplier = 1.5f;
            hasPowerUpSkill = true;
        }
        if(skillType==SkillType.ATTACKRANGE2)
        {
            attackRangeLevel = Mathf.Max(attackRangeLevel, 2);
            currentRangeMultiplier = 3.0f;
            hasPowerUpSkill = true;
        }
        if(skillType==SkillType.ThrowAttack)
        {
            canThrowAttack = true;
        }
        if(skillType==SkillType.ThrowAttack2)
        {
            canThrowAttack = false;
            canThrowAttack2 = true;
        }
        if (skillType == SkillType.ThrowAttack3)
        {
            canThrowAttack2 = false;
            canThrowAttack3 = true;
        }
        if(skillType==SkillType.ShieldRepair)
        {
            if (!hasShieldRepairSkill)
            {
                hasShieldRepairSkill = true;
                // 永続回復Coroutine開始
                shieldRepairCoroutine = StartCoroutine(ShieldRepairOverTime());
            }
        }
        if (skillType == SkillType.ShieldRepairSpeed)
        {
            if (!hasShieldRepairSkill)
            {
                hasShieldRepairSkill = true;
                // 永続回復Coroutine開始
                shieldRepairCoroutine = StartCoroutine(ShieldRepairOverTime2());
            }
        }
    }
    //シールド関連
    private IEnumerator ShieldRepairOverTime()
    {
        while (hasShieldRepairSkill)
        {
            yield return new WaitForSeconds(shieldRepairInterval);
            RecoverShield();
        }
    }
    private IEnumerator ShieldRepairOverTime2()
    {
        while (hasShieldRepairSkill)
        {
            yield return new WaitForSeconds(shieldRepairInterval2);
            RecoverShield();
        }
    }
    private void RecoverShield()
    {
        int maxHp = 4; // 最大HP
        if (hp < maxHp)
        {
            if (ChildSE != null) AudioSource.PlayOneShot(ChildSE);
            hp = Mathf.Min(hp + shieldRepairAmount, maxHp);
            UpdateShieldSprite();
            PlayShieldEffect();
        }
    }
    private void UpdateShieldSprite()
    {
        var spriteRenderer = targetSprite.GetComponent<SpriteRenderer>();
        if (hp == 4)
            spriteRenderer.sprite = Shield3;
        else if (hp == 3)
            spriteRenderer.sprite = Shield2;
        else if (hp == 2)
            spriteRenderer.sprite = Shield;
        else
            spriteRenderer.sprite = Nomal;
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
            if (janpSE != null) AudioSource.PlayOneShot(janpSE);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        if (Input.GetButtonDown("Fire2"))
        {
            if (AtaackSE != null) AudioSource.PlayOneShot(AtaackSE);
            Attack();
            if(canThrowAttack)
            {
                ThrowAttack();
            }
            if(canThrowAttack2)
            {
                ThrowAttack2();
            }
            if(canThrowAttack3)
            {
                ThrowAttack3();
            }
        }
        if (Input.GetButtonDown("Fire1"))
        {
            if(SpecialGauge.instance.IsFull())
            {
                if (spesialSE != null) AudioSource.PlayOneShot(spesialSE);
                StartCoroutine(DoSpecialAttack());
                SpecialGauge.instance.ResetGauge();
            }
            else
            {
                Debug.Log("弾ってない");
            }
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
            Vector3 spawnPos = transform.position + Vector3.up * 1.0f; // プレイヤーの真上1.0の位置（調整可）
            GameObject slash = Instantiate(slashPrehab, spawnPos, Quaternion.identity);

            // 追従設定
            Slash slashScript = slash.GetComponent<Slash>();
            slashScript.transform.SetParent(transform);//slashScript.target = transform;                // プレイヤーを追従対象に設定
            slashScript.offset = new Vector3(0, 1.0f, 0);  // 高さ調整

            slashScript.rangeMultiplier = currentRangeMultiplier;
            slashScript.isPoweredUp = hasPowerUpSkill;

            // スキル取得時に範囲とサイズUP
            if (hasPowerUpSkill)
            {
                slashScript.rangeMultiplier = 1.5f;
                slashScript.isPoweredUp = true;
            }
            else
            {
                slashScript.rangeMultiplier = 1.0f;
                slashScript.isPoweredUp = false;
            }
        }
    }
    //斬撃
    void ThrowAttack()
    {
        if (slashPrehab2 == null) return;

        // 発射位置（プレイヤーの少し上）
        Vector3 spawnPos = transform.position + new Vector3(0, 1.0f, 0f);

        // 斬撃生成
        GameObject slash = Instantiate(slashPrehab2, spawnPos, Quaternion.identity);

        // 方向を「上」に設定
        ThrowSlash slashScript = slash.GetComponent<ThrowSlash>();
        slashScript.SetDirection(Vector2.up); // ← 真上に飛ぶ！

        // プレイヤーの子にしない（独立して飛ばす）
    }

    private void ThrowAttack2()
    {
        if (slashPrehab2 == null) return;

        //発射位置
        Vector3 spawnPos = transform.position + new Vector3(0, 1.0f, 0);

        Vector2[] directions = new Vector2[]
        {
        Vector2.up,                       // 上
        new Vector2(1, 1).normalized,     // 斜め右上
        new Vector2(-1, 1).normalized     // 斜め左上
        };

        foreach (Vector2 dir in directions)
        {
            GameObject slash = Instantiate(slashPrehab2, spawnPos, Quaternion.identity);
            ThrowSlash slashScript = slash.GetComponent<ThrowSlash>();
            slashScript.SetDirection(dir);
        }
    }
    //五方向
    private void ThrowAttack3()
    {
        if (slashPrehab2 == null) return;

        Vector3 spawnPos = transform.position + new Vector3(0, 1.0f, 0);

        // 5方向（上・斜め左右上・斜め左右）
        Vector2[] directions = new Vector2[]
        {
        Vector2.up,                       // 上
        new Vector2(1, 1).normalized,     // 右上
        new Vector2(-1, 1).normalized,    // 左上
        new Vector2(1, 0.3f).normalized,  // 右少し上
        new Vector2(-1, 0.3f).normalized  // 左少し上
        };

        foreach (Vector2 dir in directions)
        {
            GameObject slash = Instantiate(slashPrehab2, spawnPos, Quaternion.identity);
            ThrowSlash slashScript = slash.GetComponent<ThrowSlash>();
            slashScript.SetDirection(dir);
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
        if (hp == 3)
        {
            spriteRenderer.sprite = Shield2;
            PlayShieldEffect();
        }
        else if (hp == 2)
        {
            spriteRenderer.sprite = Shield;
            PlayShieldEffect();
        }
        else if(hp==1)
        {
            spriteRenderer.sprite = Nomal;
            PlayShieldEffect();
        }
        else  
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
        // エフェクト再生
        if (breakEffectPrefab != null)
        {
            if (dieSE != null) AudioSource.PlayOneShot(dieSE);
            GameObject effect = Instantiate(breakEffectPrefab, transform.position, Quaternion.identity);
            Destroy(effect, 2f); // エフェクトを自動削除
        }

        if (BGMManager.instance != null)
        {
            //BGMManager.instance.FadeOut(1f); // フェードアウト停止（ゆっくり消える）
            BGMManager.instance.StopBGM();
        }
        // プレイヤー削除
        Destroy(gameObject);
         GameOverManager.Instance.SetPlayerDeathPosition(transform.position);
        GameOverManager.Instance.StartCoroutine(GameOverManager.Instance.StartGameOver());

    }


}