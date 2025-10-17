using System.Collections;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager instance;

    [Header("BGM設定")]
    public AudioSource bgmSource;
    public AudioClip bgmClip;  // ← WAVファイルをInspectorにセット

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // シーン切り替えでも残す
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        PlayBGM(); // ゲーム開始時に自動再生したいなら
    }

    public void PlayBGM()
    {
        if (bgmSource == null || bgmClip == null) return;

        bgmSource.clip = bgmClip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        if (bgmSource != null && bgmSource.isPlaying)
            bgmSource.Stop();
    }

    public void FadeOut(float duration = 1f)
    {
        if (bgmSource != null && bgmSource.isPlaying)
            StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float startVolume = bgmSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            bgmSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        bgmSource.Stop();
        bgmSource.volume = startVolume;
    }
}

