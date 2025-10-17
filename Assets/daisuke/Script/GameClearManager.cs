using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using Unity.Mathematics;
using UnityEditor;

public class GameClearManager : MonoBehaviour
{
    [SerializeField] public GameObject clearUI;
    [SerializeField] public GameObject playerObject;
    public static GameClearManager instance { get; private set; }

    [SerializeField] private Camera mainCamera;
    [SerializeField] private Transform player;
   
    [Header("Fade/ClearPlaer")]
    [SerializeField] private Image fadeimage;
    [SerializeField] private GameObject clonePlayerobject;

    [Header("Clear UI")]
    [SerializeField] private GameObject ClearCanvas;
    [SerializeField] private Image spotLightImage;
    [SerializeField] private Image gameClearText;

    [Header("Score UI")]
    [SerializeField] private Transform scoreParet;
    [SerializeField] private Sprite[] numberSprite;
    [Header("数字同士の幅")]
    [SerializeField] private float numberSpacing;

    [Header("Buttons")]
    [SerializeField] private GameObject retryButton;
    [SerializeField] private GameObject titleButton;

    [Header("数値設定")]
    [SerializeField] private float zoomspeed = 2f;       // ズームスピード
    [SerializeField] private float zoomDistance = 1.5f;　// ズーム距離
    [SerializeField] private float fadespeed = 1f;　　　 // フェードスピード 
    [SerializeField] private float rotateSpeed = 180f;
    [SerializeField] private float rightOffset;

    [Header("オーディオ関連")]
    [SerializeField] private AudioSource seSource;
    [SerializeField] private AudioClip drumClip;
    [SerializeField] private AudioClip scoreClip;
    [SerializeField] private AudioClip retryClip;
    [SerializeField] private AudioClip titleClip;

    private bool isClearing = false;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        if(ClearCanvas != null)
        {
            ClearCanvas.SetActive(false);
        }

        if(mainCamera == null)
            mainCamera = Camera.main;

        if(player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        // フェード初期化
        if(fadeimage != null )
        {
            var c = fadeimage.color;
            c.a = 0f;
            fadeimage.color = c;
        }

        // UI初期化
        if(spotLightImage != null ) spotLightImage.gameObject.SetActive(false);
        if(gameClearText != null ) gameClearText.gameObject.SetActive(false);
   
        // リトライ・タイトルへ戻るボタン初期化
        if(retryButton != null ) retryButton.SetActive(false);
        if(titleButton != null ) titleButton.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G)) // デバック用キー　Gキー
        {
            StartCoroutine(StartGameClear());
        }
        if (retryButton != null && retryButton.activeInHierarchy)
        {
            Debug.Log("Retryボタンが表示中。Raycast Target: " + fadeimage.raycastTarget);
        }
    }

    public IEnumerator StartGameClear()
    {
        if(ClearCanvas != null) ClearCanvas.SetActive(true);

       isClearing = true;
        //Time.timeScale = 0f; // ゲーム停止

        // ==== ズーム処理 ====

        Vector3 startPos = mainCamera.transform.position;
        Vector3 targetPos = player.transform.position + new Vector3(0,1.5f,-zoomDistance);

        float t = 0f;
        while(t < 1f)
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

            if(fadeimage != null )
            {
                var c = fadeimage.color;
                c.a = Mathf.Clamp01(f);
                fadeimage.color = c;
            }

            yield return null;
        }

        // ==== 新プレイヤー生成 ====
        GameObject newPlayer = null;
        if (clonePlayerobject != null && player != null)
        {
            // スポットライトのスクリーン座標を取得
            Vector3 spotlightScreenPos = RectTransformUtility.WorldToScreenPoint(mainCamera, spotLightImage.rectTransform.position);

            // ここでスクリーン座標をワールド座標に変換
            Vector3 worldPos;
            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(
                spotLightImage.rectTransform,
                spotlightScreenPos,
                mainCamera,
                out worldPos))
            {
                // 少し下にずらす（画像の下側に出す）
                worldPos += new Vector3(0f, -2f, 2f); // ←zはカメラの距離調整に応じて調整

                // クローン生成（固定回転）
                Quaternion fixedRot = Quaternion.Euler(20f, 40f, 0f);
                newPlayer = Instantiate(clonePlayerobject, worldPos, fixedRot);
                newPlayer.layer = LayerMask.NameToLayer("ClearPlayer");
            }
            // ==== プレイヤー削除 ====
            GameObject scenePlayer = GameObject.FindWithTag("Player");
            if (scenePlayer != null)
            {
                Destroy(scenePlayer);
            }
        }

        // ==== 回転演出 ====
        if (newPlayer != null)
        {
            StartCoroutine(RotateForever(newPlayer));
            yield return new WaitForSecondsRealtime(1f);
        }

        //// ==== オーディオ停止 ====
        if (seSource != null && seSource.isPlaying) seSource.Stop();

        // ==== クリアテキストとスポットライト表示 ====
        if (gameClearText != null) gameClearText.gameObject.SetActive(true);
        if (spotLightImage != null) spotLightImage.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1f);

        // ==== スコア表示 ====
        ShowScore(100);
        if (seSource != null && scoreClip != null)
        {
            seSource.PlayOneShot(scoreClip);
        }
        yield return new WaitForSecondsRealtime(1f);

        Time.timeScale = 1f;
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

        // 1フレーム待って EventSystem を更新
        yield return null;


        isClearing = false;
    }

    private IEnumerator RotateForever(GameObject target)
    {
        while (true)
        {
            target.transform.Rotate(Vector3.up * rotateSpeed * Time.unscaledDeltaTime);
            yield return null;
        }
    }

    private void ShowScore(int score)
    {
        if(scoreParet == null || numberSprite == null || numberSprite.Length < 10) return;
        
        // 一旦クリア
        foreach (Transform child in scoreParet)
            Destroy(child.gameObject);

        string scoreText = score.ToString();
        float startX = -((scoreText.Length - 1) * numberSpacing) / 2f;

        for(int i = 0; i < scoreText.Length; i++)
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
        if(SkillPointManager.instance != null)
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

    void ShowClearUI()
    {
        clearUI.SetActive(true);

        // プレイヤーの動きを完全に停止
        if (playerObject != null)
        {
            var rb = playerObject.GetComponent<Rigidbody2D>();
            if (rb != null) rb.linearVelocity = Vector2.zero;

            // 物理的に止める
            rb.simulated = false;

            // スクリプト無効化（PlayerClrを止める）
            var playerScript = playerObject.GetComponent<PlayerClr>();
            if (playerScript != null)
                playerScript.enabled = false;
        }
    }
}
