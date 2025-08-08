using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UniversalFixedDirectionFill : MonoBehaviour
{
    [System.Serializable]
    public class FillTarget
    {
        public string name;
        public Image image;
        public enum Axis { Horizontal, Vertical }
        public Axis fillAxis = Axis.Horizontal;
    }

    public List<FillTarget> fillTargets = new List<FillTarget>();

    private Vector2 dragStartScreenPos;
    private bool isDragging = false;

    void Update()
    {
        Vector2 inputPosition = Vector2.zero;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            inputPosition = touch.position;

            if (touch.phase == TouchPhase.Began)
                TryBeginDrag(inputPosition);
            else if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && isDragging)
                UpdateFill(inputPosition);
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                EndDrag();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            TryBeginDrag(Input.mousePosition);
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

    void TryBeginDrag(Vector2 screenPos)
    {
        foreach (var target in fillTargets)
        {
            if (!target.image) continue;

            if (RectTransformUtility.RectangleContainsScreenPoint(target.image.rectTransform, screenPos))
            {
                BeginDrag(screenPos);
                return;
            }
        }
    }

    void BeginDrag(Vector2 screenPos)
    {
        isDragging = true;
        dragStartScreenPos = screenPos;

        // Set fixed origin for all
        foreach (var target in fillTargets)
        {
            if (!target.image) continue;
            target.image.fillOrigin = 0; // Always from left or bottom
        }
    }

    void UpdateFill(Vector2 screenPos)
    {
        foreach (var target in fillTargets)
        {
            if (!target.image) continue;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                target.image.rectTransform, screenPos, null, out Vector2 localPos)) continue;

            RectTransform rt = target.image.rectTransform;
            Vector2 size = rt.rect.size;
            float fill = 0f;

            if (target.fillAxis == FillTarget.Axis.Horizontal)
            {
                float x = localPos.x + size.x * 0.5f;
                fill = Mathf.Clamp01(x / size.x);
            }
            else
            {
                float y = localPos.y + size.y * 0.5f;
                fill = Mathf.Clamp01(y / size.y);
            }

            target.image.fillAmount = fill;
        }
    }

    void EndDrag()
    {
        isDragging = false;
    }
}
