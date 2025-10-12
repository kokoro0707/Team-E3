using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillContent : MonoBehaviour
{
    [SerializeField] private Image skillimage;
    [SerializeField] private GameObject panel;

    private CanvasGroup group;

    private void Awake()
    {
        if(panel != null)
        {
            group = panel.GetComponent<CanvasGroup>();
            if (group == null) group = panel.AddComponent<CanvasGroup>();

            group.alpha = 0;
            group.interactable = false;
            group.blocksRaycasts = false;
            panel.SetActive(false);
        }
    }

    public void ShowContent(Sprite sprite)
    {
      if(skillimage == null || panel == null) return;

      panel.SetActive(true);

        skillimage.sprite = sprite;
        skillimage.SetNativeSize();
        group.alpha = 1;
        group.interactable = false ;
        group.blocksRaycasts = false;
    }

    public void HideContent()
    {
        if (group != null)
            group.alpha = 0;
        if(panel != null)
        panel.SetActive(false);
    }
}
