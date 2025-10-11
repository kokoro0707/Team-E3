using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillContent : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI skillText;
    [SerializeField] private GameObject panel;

    private CanvasGroup group;

    private void Awake()
    {
        if(panel != null)
        {
            group = GetComponent<CanvasGroup>();
            if (group == null) group = panel.AddComponent<CanvasGroup>();

            group.alpha = 0;
            group.interactable = false;
            group.blocksRaycasts = false;
        }
    }

    public void ShowContent(string text)
    {
        Debug.Log("showcontentが呼ばれた" + text);
        if (skillText == null)
        {
            Debug.Log("テキストなし");
            return;
        }

        if (panel == null)
        {
            Debug.Log("panelなし");
            return;
        }
        skillText.text = text;
        panel.SetActive(true);

        Debug.Log($"{panel.name}を表示,text{skillText.text}");
    }

    public void HideContent()
    {
        if (group == null) return;
        group.alpha = 0;
    }

}
