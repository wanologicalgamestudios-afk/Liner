using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SmartMultiImageFill : MonoBehaviour
{
    [System.Serializable]
    public class FillTarget
    {
        public string name;
        public Image image;
        public enum Axis { Horizontal, Vertical }
        public Axis fillAxis = Axis.Horizontal;
    }

    [Header("Assign UI Images")]
    public List<FillTarget> fillTargets = new List<FillTarget>();

    private Vector2 dragStartScreenPos;
    private bool isDragging = false;

    private Dictionary<Image, int> imageFillOrigins = new Dictionary<Image, int>();

    void Update()
    {
        Vector2 inputPosition = Vector2.zero;
        bool validInput = false;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            inputPosition = touch.position;

            if (touch.phase == TouchPhase.Began)
                BeginDrag(inputPosition);
            else if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && isDragging)
                UpdateFill(inputPosition);
            else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                EndDrag();
        }
        else if (Input.GetMouseButtonDown(0))
        {
            BeginDrag(Input.mousePosition);
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

    void BeginDrag(Vector2 screenPos)
    {
        isDragging = true;
        dragStartScreenPos = screenPos;
        imageFillOrigins.Clear();

        foreach (var target in fillTargets)
        {
            if (!target.image || !target.image.gameObject.activeInHierarchy)
                continue;

            RectTransform rt = target.image.rectTransform;
            Vector3[] worldCorners = new Vector3[4];
            rt.GetWorldCorners(worldCorners);

            Vector2 closestSidePos;
            int origin = 0;

            if (target.fillAxis == FillTarget.Axis.Horizontal)
            {
                float left = worldCorners[0].x;
                float right = worldCorners[3].x;
                float distToLeft = Mathf.Abs(screenPos.x - left);
                float distToRight = Mathf.Abs(screenPos.x - right);
                origin = distToLeft < distToRight ? 0 : 1;
            }
            else // Vertical
            {
                float bottom = worldCorners[0].y;
                float top = worldCorners[1].y;
                float distToBottom = Mathf.Abs(screenPos.y - bottom);
                float distToTop = Mathf.Abs(screenPos.y - top);
                origin = distToBottom < distToTop ? 0 : 1;
            }

            imageFillOrigins[target.image] = origin;
            target.image.fillOrigin = origin;
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

            int origin = imageFillOrigins.ContainsKey(target.image) ? imageFillOrigins[target.image] : 0;
            if (origin == 1)
                fill = 1f - fill;

            target.image.fillAmount = fill;
        }
    }

    void EndDrag()
    {
        isDragging = false;
        imageFillOrigins.Clear();
    }
}