using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIFillByDrag : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public Image fillImage; // Assign your UI Image here with Fill Method set to Horizontal
    public int currentFillingAmount = 0; // Fill percentage from 1 to 100
    public bool canSelectNextToFillUp = false; // True if fill amount > 90

    private RectTransform rectTransform;
    private bool isDragging = false;
    private bool fillFromLeft = true; // true = left to right, false = right to left

    void Start()
    {
        if (fillImage == null)
        {
            Debug.LogError("Fill Image is not assigned.");
            enabled = false;
            return;
        }

        rectTransform = fillImage.GetComponent<RectTransform>();
        fillImage.fillAmount = 0f;
        fillImage.fillOrigin = (int)Image.OriginHorizontal.Left; // default
        currentFillingAmount = 0;
        canSelectNextToFillUp = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            float width = rectTransform.rect.width;
            fillFromLeft = (localPoint.x < 0); // left half if x < 0, else right half
            fillImage.fillOrigin = fillFromLeft ? (int)Image.OriginHorizontal.Left : (int)Image.OriginHorizontal.Right;
        }

        UpdateFill(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDragging)
            UpdateFill(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    private void UpdateFill(PointerEventData eventData)
    {
        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, eventData.position, eventData.pressEventCamera, out localPoint))
        {
            float width = rectTransform.rect.width;
            float normalizedX = Mathf.Clamp01((localPoint.x + width * 0.5f) / width);

            if (fillFromLeft)
            {
                fillImage.fillAmount = normalizedX;
            }
            else
            {
                fillImage.fillAmount = 1f - normalizedX;
            }

            currentFillingAmount = Mathf.Clamp(Mathf.RoundToInt(fillImage.fillAmount * 100), 1, 100);
            canSelectNextToFillUp = currentFillingAmount > 90;
        }
    }
}
