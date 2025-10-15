using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using Unity.Mathematics;
using UnityEditor;

public class GameClearManager : MonoBehaviour
{
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
    [SerializeField] private float zoomspeed = 2f;
    [SerializeField] private float zoomDistance = 1.5f;
    [SerializeField] private float fadespeed = 1f;
    [SerializeField] private float rotateSpeed = 180f;
    [SerializeField] private float rightOffset;

    private bool isClearing = false;

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
        if(Input.GetKeyDown(KeyCode.G)) // デバック用キー　Gキー
        {
            StartCoroutine(StartGameClear());
        }
    }

    private IEnumerator StartGameClear()
    {
        if(ClearCanvas != null) ClearCanvas.SetActive(true);

       isClearing = true;
        Time.timeScale = 0f; // ゲーム停止

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

        // ==== SE部分 ====
       

        // ==== 新プレイヤー生成 ====
        GameObject newPlayer = null;
        if (clonePlayerobject != null && player != null)
        {
            Vector3 rightDir = player.right * rightOffset;
            Vector3 spawnPos = player.position + rightDir;
            quaternion spawanRot = player.rotation;
            newPlayer = Instantiate(clonePlayerobject, spawnPos, spawanRot);
            newPlayer.layer = LayerMask.NameToLayer("ClearPlayer");

            newPlayer.transform.Rotate(20f, 40f, 0f,Space.Self);
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

            StartCoroutine(RotateForever(newPlayer));
            yield return new WaitForSecondsRealtime(1f);
        }

        // ==== クリアテキストとスポットライト表示 ====
        if(gameClearText != null) gameClearText.gameObject.SetActive(true);
        if (spotLightImage != null) spotLightImage.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1f);

        // ==== スコア表示 ====
        ShowScore(100);
        yield return new WaitForSecondsRealtime(1f);

        // ==== リトライ・タイトルボタン表示
        if(retryButton != null) retryButton.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1f);
        if(titleButton != null) titleButton.gameObject.SetActive(true);

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

    public void OnCTitleButton()
    {
        Time.timeScale = 1f;
        //UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
        Debug.Log("タイトルに戻る");
    }
}
