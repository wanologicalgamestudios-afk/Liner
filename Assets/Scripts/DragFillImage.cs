using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DragFillWithDirection : MonoBehaviour
{
    public Image fillImage;
    public enum FillAxis { Horizontal, Vertical }
    public FillAxis fillAxis = FillAxis.Horizontal;

    private bool isDragging = false;
    private bool directionLocked = false;
    private int fillOrigin = 0; // 0 = Left or Bottom, 1 = Right or Top

    void Update()
    {
        Vector2 inputPosition = Vector2.zero;
        bool validInput = false;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            inputPosition = touch.position;

            if (touch.phase == TouchPhase.Began)
            {
                TryStartDrag(inputPosition);
            }
            else if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && isDragging)
            {
                UpdateFill(inputPosition);
            }
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
            {
                EndDrag();
            }
        }
        else if (Input.GetMouseButtonDown(0))
        {
            TryStartDrag(Input.mousePosition);
        }
        else if (Input.GetMouseButton(0) && isDragging)
        {
            UpdateFill(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndDrag();
        }
    }

    void TryStartDrag(Vector2 screenPos)
    {
        if (IsPointerOverUI(screenPos, out Vector2 localPos))
        {
            isDragging = true;
            directionLocked = true;

            // Detect initial side (left/right or top/bottom)
            Vector2 size = fillImage.rectTransform.rect.size;

            if (fillAxis == FillAxis.Horizontal)
            {
                float x = localPos.x + size.x * 0.5f;
                fillOrigin = (x < size.x / 2f) ? 0 : 1; // 0 = left, 1 = right
                fillImage.fillOrigin = fillOrigin;
            }
            else
            {
                float y = localPos.y + size.y * 0.5f;
                fillOrigin = (y < size.y / 2f) ? 0 : 1; // 0 = bottom, 1 = top
                fillImage.fillOrigin = fillOrigin;
            }
        }
    }

    void UpdateFill(Vector2 screenPos)
    {
        if (IsPointerOverUI(screenPos, out Vector2 localPos))
        {
            Vector2 size = fillImage.rectTransform.rect.size;
            float fill = 0f;

            if (fillAxis == FillAxis.Horizontal)
            {
                float x = localPos.x + size.x * 0.5f;
                fill = Mathf.Clamp01(x / size.x);

                if (fillOrigin == 1) // From Right
                    fill = 1f - fill;
            }
            else
            {
                float y = localPos.y + size.y * 0.5f;
                fill = Mathf.Clamp01(y / size.y);

                if (fillOrigin == 1) // From Top
                    fill = 1f - fill;
            }

            fillImage.fillAmount = fill;
        }
    }

    void EndDrag()
    {
        isDragging = false;
        directionLocked = false;
    }

    bool IsPointerOverUI(Vector2 screenPos, out Vector2 localPoint)
    {
        return RectTransformUtility.ScreenPointToLocalPointInRectangle(
            fillImage.rectTransform,
            screenPos,
            null,
            out localPoint
        );
    }
}
