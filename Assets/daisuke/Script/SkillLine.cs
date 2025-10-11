using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SkillLine : MonoBehaviour
{
    [SerializeField] Image fillLine;
    [SerializeField] Image baseLine;

    [SerializeField] Color baseColor = Color.gray;
    [SerializeField] Color fillColor = Color.black;
    [SerializeField] float fillDuration = 0.5f;

    private void Awake()
    {
        if(fillLine != null )
        {
            fillLine.type = Image.Type.Filled;
            fillLine.fillMethod = Image.FillMethod.Vertical;
            fillLine.fillOrigin = 1;
            fillLine.fillAmount = 0f;
            fillLine.color = fillColor;
        }
        if (baseLine != null)
        {
            baseLine.color = baseColor;
        }
    }

    public void SetFillProgress(float t)
    {
        t = Mathf.Clamp01(t);
        if (fillLine != null)
        {
            fillLine.fillAmount = t;
            //Debug.Log("fillAmount" +  t);
        }
    }
    public void SetComplete()
    {
        SetFillProgress(1f);
    }

    public void ResetLine()
    {
        SetFillProgress(0f);
    }

    // アニメーションを開始
    public void AnimaterFill()
    {
        StopAllCoroutines();
        StartCoroutine(FillAnimeCoroutine());
    }

    private IEnumerator FillAnimeCoroutine()
    {
        float elapsed = 0f;
        float start = fillLine != null ? fillLine.fillAmount : 0f;
        while(elapsed < fillDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fillDuration);
            SetFillProgress(Mathf.Lerp(start, 1f, t));
            yield return null;
        }
        SetComplete();
    }
}
