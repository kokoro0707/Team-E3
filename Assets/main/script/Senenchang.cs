using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement; // シーン管理に必要

public class SceneChanger : MonoBehaviour
{
    public GameObject canvasTitle;
    public GameObject canvasTutorial;

    public string gameSceneName = "arfa2";

    private bool isTutorialActive = false; // 操作説明画面が表示中か
    public AudioSource AudioSource;
    public AudioClip poi;

    public void OnStartButtonPressed()
    {
        if (poi != null) AudioSource.PlayOneShot(poi);
        canvasTitle.SetActive(false);
        canvasTutorial.SetActive(true);
        isTutorialActive = true; // 操作説明画面を表示した状態に
    }


    void Update()
    {
        // 操作説明画面が出てるときだけ左クリックでシーン遷移
        if (isTutorialActive && Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }
}


