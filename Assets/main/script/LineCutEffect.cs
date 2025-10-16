using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class LineCutEffect_FastHide : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public RectTransform underlineRect;
    public float targetWidth = 100f;    // ü‚ÌÅ‘å•
    public float expandDuration = 0.3f; // ü‚ğL‚Î‚·ŠÔ

    private Coroutine currentCoroutine;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        currentCoroutine = StartCoroutine(ExpandLine());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentCoroutine != null)
            StopCoroutine(currentCoroutine);

        // ˆêu‚Åü‚ğÁ‚·i•‚ğ0‚Éj
        underlineRect.sizeDelta = new Vector2(0f, underlineRect.sizeDelta.y);

        currentCoroutine = null;
    }

    private IEnumerator ExpandLine()
    {
        float elapsed = 0f;
        Vector2 startSize = underlineRect.sizeDelta;
        Vector2 targetSize = new Vector2(targetWidth, underlineRect.sizeDelta.y);

        while (elapsed < expandDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / expandDuration);
            underlineRect.sizeDelta = Vector2.Lerp(startSize, targetSize, t);
            yield return null;
        }

        underlineRect.sizeDelta = targetSize;
        currentCoroutine = null;
    }
}

