using UnityEngine;
using UnityEngine.UI;

public class SpecialGauge : MonoBehaviour
{
    public static SpecialGauge instance; // シングルトン
    public Slider gaugeSlider;

    public AudioSource AudioSource;
    public AudioClip tamaru;
    private int currentKills = 0;
    private int maxKills = 20;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        gaugeSlider.maxValue = maxKills;
        gaugeSlider.value = 0;
    }

    public void AddKill()
    {
        currentKills++;
        gaugeSlider.value = currentKills;

        if (currentKills >= maxKills)
        {
            if (tamaru != null) AudioSource.PlayOneShot(tamaru);
            gaugeSlider.value = maxKills;
            Debug.Log("必殺ゲージMAX!");
        }
    }

    public bool IsFull()
    {
        return currentKills >= maxKills;
    }

    public void ResetGauge()
    {
        currentKills = 0;
        gaugeSlider.value = 0;
    }
}
