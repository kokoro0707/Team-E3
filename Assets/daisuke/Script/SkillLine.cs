using UnityEngine;
using UnityEngine.UI;

public class SkillLine : MonoBehaviour
{
    [SerializeField] Image fillLine;
    [SerializeField] Color baseColor = Color.gray;
    [SerializeField] Color SkillColor = Color.cyan;

    public void SetFillProgress(float t)
    {
        t = Mathf.Clamp01(t);
        if(fillLine != null )
        {
            fillLine.fillAmount = t;
            fillLine.color = Color.Lerp(baseColor,SkillColor,t);
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

    private void Awake()
    {
        if(fillLine != null )
        {
            fillLine.type = Image.Type.Filled;
            fillLine.fillMethod = Image.FillMethod.Horizontal;
            fillLine.fillOrigin = 0;
            fillLine.fillAmount = 0f;
            fillLine.color = baseColor;
        }  
    }
}
