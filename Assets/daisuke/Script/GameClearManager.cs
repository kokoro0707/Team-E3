using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameClearManager : MonoBehaviour
{
    public Image GameClearImage;
    public float slideTime = 5f; // スライドにかける時間
    public string nextStageName; // 次のステージのシーン名

    private Vector2 starttPos;
    private Vector2 midPos;
    private Vector2 endPos;

    private bool isTransrationStarted = false;
    private bool hasLoadScene = false;
    private bool isSlidingout = false;
    private float t = 0f;

    private void Awake()
    {
        var root = transform.root.gameObject;
        DontDestroyOnLoad(root);
        SceneManager.sceneLoaded += OnSceneLoad;
    }

    void Start()
    { 
        RectTransform rect = GameClearImage.rectTransform;
        float canvasWidth = rect.rect.width * 1.2f;
        starttPos = new Vector2(-canvasWidth, 0); // 画面左外
        midPos = new Vector2(0, 0); // 画面中央
        endPos = new Vector2(canvasWidth, 0); // 画面右外
        rect.anchoredPosition = starttPos;
        GameClearImage.gameObject.SetActive(false);
    }

    public void StartGameClear()
    {
        if (isTransrationStarted) return;

        GameClearImage.rectTransform.anchoredPosition = starttPos;

        GameClearImage.gameObject.SetActive(true);
        isTransrationStarted = true;
        hasLoadScene = false;
        t = 0f;

        // ゲーム全体の時間を停止
        Time.timeScale = 0f;

        Debug.Log("gameclearが呼ばれた");
    }

    void Update()
    {
        if (!isTransrationStarted) return;

        // 時間に依存しないアニメーション
        t += Time.unscaledDeltaTime;

        // 左から中央へスライドイン
        if (!hasLoadScene && !isSlidingout)
        {
            float slideT = Mathf.Clamp01(t /slideTime); // 1秒で中央に到達
            GameClearImage.rectTransform.anchoredPosition = Vector2.Lerp(starttPos, midPos, slideT);

            // 中央に完全到達した瞬間
            if (slideT >= 1f)
            {
                StartCoroutine(LoadNextScene());
                hasLoadScene = true;
            }
        }

        // 右へスライドアウト
        if (isSlidingout)
        {
            float slideOutT = Mathf.Clamp01(t / slideTime);
            GameClearImage.rectTransform.anchoredPosition = Vector2.Lerp(midPos, endPos, slideOutT);
            // 右外に完全到達したら終了
            if (slideOutT >= 1f)
            {
                Debug.Log("GameClear完了");
                isTransrationStarted = false;
                GameClearImage.gameObject.SetActive(false);
                Time.timeScale = 1f; // 時間を再開
            }
        }
    }

    private System.Collections.IEnumerator LoadNextScene()
    {
        yield return new WaitForSecondsRealtime(0.5f);

        SceneManager.LoadScene(nextStageName);
    }

    private void OnSceneLoad(Scene scene, LoadSceneMode mode)
    {
        if(isTransrationStarted && !isSlidingout)
        {
            Debug.Log("シーンロード完了");
            // シーンがロードされた直後にスライドアウトを開始
            t = 0f;
            isSlidingout = true;
        }
    }
}
