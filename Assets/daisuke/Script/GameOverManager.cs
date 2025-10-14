using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using Unity.Mathematics;
using UnityEditor;
using Unity.VisualScripting;

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
    [SerializeField] private float rightOffset;

    [SerializeField] private ParticleSystem explodeEffect;

    private bool isClearing = false;

    private void Start()
    {
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

        // ==== SE部分 ====

      

        // ==== 新プレイヤー生成 ====
        GameObject newPlayer = null;
        if (clonePlayerobject != null)
        {
            Vector3 rightDir = Vector3.right * rightOffset;
            Vector3 spawnPos = lastplayerPositon + rightDir;
            quaternion spawanRot = player != null ? player.rotation : quaternion.identity;
            newPlayer = Instantiate(clonePlayerobject, spawnPos, spawanRot);
            newPlayer.layer = LayerMask.NameToLayer("ClearPlayer");

            newPlayer.transform.Rotate(20f, 40f, 0f, Space.Self);
        }

        // ==== 回転演出 ====
        if (newPlayer != null)
        {
            //float rotTime = 0f;
            //while (rotTime < 2f)
            //{
            //    rotTime += Time.unscaledDeltaTime;
            //    newPlayer.transform.Rotate(Vector3.up, rotateSpeed * Time.unscaledDeltaTime);
            //    yield return null;
            //}

            // 永続回転
            StartCoroutine(RotateForever(newPlayer));
            yield return new WaitForSecondsRealtime(1f);

            if (gameOverText != null)
            {
                gameOverText.gameObject.SetActive(true);
                Debug.Log("text表示");
            }
            if (spotLightImage != null)
            {
                spotLightImage.gameObject.SetActive(true);
                Debug.Log("spotLight表示");
            }
            else {
                Debug.Log("参照が切れている");
            }
                yield return new WaitForSecondsRealtime(0.5f);

            // ==== 回転終了後に爆発
            if (explodeEffect != null)
            {
                Time.timeScale = 1f;
                // 爆発のプレイヤーの位置に生成＆再生
                Vector3 explosionPos = newPlayer.transform.position - mainCamera.transform.forward * 0.5f;
                Quaternion explosionRot = Quaternion.identity;
                ParticleSystem explosion = Instantiate(explodeEffect, explosionPos, explosionRot);

                Instantiate(explodeEffect,explosionPos, explosionRot);

                Destroy(newPlayer);

                yield return new WaitForSecondsRealtime(1f);
                Time.timeScale = 0f;
            }
           
        }

        // ==== スコア表示 ====
        int score = 0;
        if(SkillPointManager.instance != null)
        {
            score = SkillPointManager.instance.GetTotalKillCount();
        }
        ShowScore(score);
        yield return new WaitForSecondsRealtime(0.5f);

        // ==== リトライ・タイトルボタン表示
        if (retryButton != null) retryButton.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1f);
        if (titleButton != null) titleButton.gameObject.SetActive(true);

        Time.timeScale = 1f;
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

    public void OnCTitleButton()
    {
        Time.timeScale = 1f;
        //UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
        Debug.Log("タイトルに戻る");
    }
}
