using UnityEngine;
using UnityEngine.UI;

public class SkillLine : MonoBehaviour
{
    [SerializeField] Image lineImage;
    [SerializeField] Color startColor = Color.gray;
    [SerializeField] Color endColor = Color.cyan;

    private void Awake()
    {
        lineImage.type = Image.Type.Filled;
        lineImage.fillMethod = Image.FillMethod.Horizontal;
        lineImage.fillOrigin = 0;
        lineImage.fillAmount = 0f;
        lineImage.color = startColor;
    }

    public void SetFillProgres(float t)
    {
        lineImage.fillAmount = t;
        lineImage.color = Color.Lerp(startColor, endColor, t);
    }
}
