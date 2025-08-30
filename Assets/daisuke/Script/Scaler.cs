using UnityEngine;
using UnityEngine.EventSystems;

public class Scaler: MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 ChangeScale = new Vector3(1.2f, 1.2f, 1);
    private Vector3 originalScale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalScale = transform.localScale; // å≥ÇÃÉXÉPÅ[Éã
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = ChangeScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;
    }
}
