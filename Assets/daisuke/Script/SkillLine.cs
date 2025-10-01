using UnityEngine;
using UnityEngine.UI;

public class SkillLine : MonoBehaviour
{
    [SerializeField] Image fillLine;
    [SerializeField] Image baseLine;

    [SerializeField] Color baseColor = Color.gray;
    [SerializeField] Color fillColor = Color.cyan;

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
    }
    public void SetFillProgress(float t)
    {
        t = Mathf.Clamp01(t);
        if (fillLine != null)
        {
            fillLine.fillAmount = t;
            Debug.Log("fillAmount" +  t);
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
}
