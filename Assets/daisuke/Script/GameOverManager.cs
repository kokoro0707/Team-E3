using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using Unity.Mathematics;
using UnityEditor;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class GameOverManager : MonoBehaviour
{
    public static GameOverManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform player;

    private Vector3 lastplayerPositon;

    [Header("Fade/ClonePlaer")]
    [SerializeField] private Image fadeimage;
    [SerializeField] private GameObject clonePlayerobject;

    [Header("Clear UI")]
    [SerializeField] private Image spotLightImage;
    [SerializeField] private Image gameOverText;

    [Header("Score UI")]
    [SerializeField] private GameObject OverCanvas;
    [SerializeField] private Transform scoreParet;
    [SerializeField] private Sprite[] numberSprite;

    [Header("Buttons")]
    [SerializeField] private GameObject retryButton;
    [SerializeField] private GameObject titleButton;

    [Header("数字同士の幅")]
    [SerializeField] private float numberSpacing;

    [Header("数値設定")]
    [SerializeField] private float zoomspeed = 2f;
    [SerializeField] private float zoomDistance = 1.5f;
    [SerializeField] private float fadespeed = 1f;
    [SerializeField] private float rotateSpeed = 180f;
    [SerializeField] private float rightOffset = 5.3f;

    [Header("爆発エフェクト")]
    [SerializeField] private ParticleSystem explodeEffect;

    [Header("オーディオ関連")]
    [SerializeField] private AudioSource seSource;
    [SerializeField] private AudioClip drumClip;
    [SerializeField] private AudioClip explsosionClip;
    [SerializeField] private AudioClip scoreClip;
    [SerializeField] private AudioClip retryClip;
    [SerializeField] private AudioClip titleClip;

    private bool isClearing = false;

    private void Start()
    {
        if(OverCanvas  != null)
        {
            OverCanvas.SetActive(false);
        }

        if (mainCamera == null)
            mainCamera = Camera.main;

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player != null)
            lastplayerPositon = player.position; // 初期座標を記録

        // フェード初期化
        if (fadeimage != null)
        {
            var c = fadeimage.color;
            c.a = 0f;
            fadeimage.color = c;
        }

        // UI初期化
        if (spotLightImage != null) spotLightImage.gameObject.SetActive(false);
        if (gameOverText != null) gameOverText.gameObject.SetActive(false);

        // リトライ・タイトルへ戻るボタン初期化
        if (retryButton != null) retryButton.SetActive(false);
        if (titleButton != null) titleButton.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) // デバック用キー　Oキー
        {
            StartCoroutine(StartGameOver());
        }
    }

    public void SetPlayerDeathPosition(Vector3 pos)
    {
        lastplayerPositon = pos;
    }

    public IEnumerator StartGameOver()
    {
        if (OverCanvas != null)
        {
            OverCanvas.SetActive(true);
        }

        isClearing = true;
        Time.timeScale = 0f; // ゲーム停止

        // ==== ズーム処理 ====

        Vector3 startPos = mainCamera.transform.position;
        Vector3 targetPos = lastplayerPositon + new Vector3(0, 1.5f, -zoomDistance);

        float t = 0f;
        while (t < 1f)
        {
            t += Time.unscaledDeltaTime * fadespeed;
            mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.3f);

        // ==== オーディオ再生 ====
        if (seSource != null && drumClip != null)
        {
            seSource.clip = drumClip;
            seSource.loop = true;
            seSource.Play();
        }

        // ==== フェード処理 ====
        float f = 0f;
        while (f < 1f)
        {
            f += Time.unscaledDeltaTime * fadespeed;

            if (fadeimage != null)
            {
                var c = fadeimage.color;
                c.a = Mathf.Clamp01(f);
                fadeimage.color = c;
            }

            yield return null;
        }
     

        // ==== 新プレイヤー生成 ====
        GameObject newPlayer = null;

        // 生成基準座標（プレイヤー生存時 or 最後の座標）
        Vector3 basePos = player != null ? player.position : lastplayerPositon;

        // スポーン位置（右方向に少しずらす）
        Vector3 offset = new Vector3(rightOffset, 0f, 0f); // ←必要なら調整（右方向に出したい距離）
        Vector3 spawnPos = basePos + offset;

        // 回転（固定）
        Quaternion spawnRot = Quaternion.Euler(20f, 40f, 0f);

        // 生成処理
        if (clonePlayerobject != null)
        {
            newPlayer = Instantiate(clonePlayerobject, spawnPos, spawnRot);
            newPlayer.layer = LayerMask.NameToLayer("ClearPlayer");
        }


        // ==== 回転演出 ====
        if (newPlayer != null)
        {

            // 永続回転
            StartCoroutine(RotateForever(newPlayer));
            yield return new WaitForSecondsRealtime(1f);

            //  ==== オーディオ停止 ====
            if (seSource != null && seSource.isPlaying) seSource.Stop();

            // ==== クリアテキストとスポットライト表示 ====
            if (gameOverText != null)gameOverText.gameObject.SetActive(true);
            if (spotLightImage != null)spotLightImage.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(0.5f);

            //// ==== 回転終了後に爆発
            if (explodeEffect != null)
            {
                if (seSource != null && explsosionClip != null)
                {
                    seSource.PlayOneShot(explsosionClip);
                }
                Time.timeScale = 1f;
                // 爆発のプレイヤーの位置に生成＆再生
                Vector3 explosionPos = newPlayer.transform.position - mainCamera.transform.forward * 0.5f;
                Quaternion explosionRot = Quaternion.identity;
                ParticleSystem explosion = Instantiate(explodeEffect, explosionPos, explosionRot);
                var renderer = explosion.GetComponent<ParticleSystemRenderer>();
                if(renderer != null)
                {
                    renderer.sortingLayerName = "Default";
                    renderer.sortingOrder = 500;
                }

                Instantiate(explodeEffect, explosionPos, explosionRot);

                Destroy(newPlayer);

                yield return new WaitForSecondsRealtime(1f);
            }

        }

        // ==== スコア表示 ====
        int score = 0;
        if(SkillPointManager.instance != null)
        {
            score = SkillPointManager.instance.GetTotalKillCount();
        }
        if (seSource != null && scoreClip != null)
        {
            seSource.PlayOneShot(scoreClip);
        }
        ShowScore(score);
        yield return new WaitForSecondsRealtime(1f);

        // ==== リトライ・タイトルボタン表示
        if (retryButton != null) retryButton.gameObject.SetActive(true);
        if (seSource != null && retryClip != null)
        {
            seSource.PlayOneShot(retryClip);
        }
        yield return new WaitForSecondsRealtime(1f);


        if (titleButton != null) titleButton.gameObject.SetActive(true);
        if (seSource != null && titleClip != null)
        {
            seSource.PlayOneShot(titleClip);
        }

        Time.timeScale = 0f;
        isClearing = false;
    }

    private IEnumerator RotateForever(GameObject target)
    {
        while (target != null)
        {
            target.transform.Rotate(Vector3.forward, rotateSpeed * Time.unscaledDeltaTime);
            yield return null;
        }
    }

    private void ShowScore(int score)
    {
        if (scoreParet == null || numberSprite == null || numberSprite.Length < 10) return;

        // 一旦クリア
        foreach (Transform child in scoreParet)
            Destroy(child.gameObject);

        string scoreText = score.ToString();
        float startX = -((scoreText.Length - 1) * numberSpacing) / 2f;

        for (int i = 0; i < scoreText.Length; i++)
        {
            int num = int.Parse(scoreText[i].ToString());

            GameObject obj = new GameObject("Num_" + num);
            obj.transform.SetParent(scoreParet, false);

            RectTransform rect = obj.AddComponent<RectTransform>();
            Image image = obj.AddComponent<Image>();
            image.sprite = numberSprite[num];
            image.SetNativeSize();

            rect.anchoredPosition = new Vector2(startX + i * numberSpacing, 0);
        }
    }

    public void OnRetryButton()
    {
        if (SkillPointManager.instance != null)
        {
            SkillPointManager.instance.ResetKillCount();
        }

        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void OnTitleButton()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("Title");
        Debug.Log("タイトルに戻る");
    }


}
