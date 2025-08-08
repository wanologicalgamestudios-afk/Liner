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

        [HideInInspector] public bool isFilled = false;
    }

    public List<FillTarget> fillTargets = new List<FillTarget>();

    private Vector2 dragStartScreenPos;
    private bool isDragging = false;
    private Dictionary<Image, int> imageFillOrigins = new Dictionary<Image, int>();
    private int activeTargetIndex = 0;

    void Start()
    {
        ResetAllFills();
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
                BeginDrag(touch.position);
            else if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && isDragging)
                UpdateFill(touch.position);
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
        // Check if pointer is over the active image
        if (!IsPointerOverTargetImage(screenPos))
            return;

        isDragging = true;
        dragStartScreenPos = screenPos;
        imageFillOrigins.Clear();

        if (activeTargetIndex < fillTargets.Count && fillTargets[activeTargetIndex].image != null)
        {
            var image = fillTargets[activeTargetIndex].image;
            RectTransform rt = image.rectTransform;
            int origin = GetNearestOrigin(fillTargets[activeTargetIndex].fillAxis, screenPos, rt);
            image.fillOrigin = origin;
            imageFillOrigins[image] = origin;
        }
    }

    void UpdateFill(Vector2 screenPos)
    {
        while (activeTargetIndex < fillTargets.Count)
        {
            var target = fillTargets[activeTargetIndex];
            if (target.isFilled || target.image == null)
            {
                activeTargetIndex++;
                continue;
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                target.image.rectTransform, screenPos, null, out Vector2 localPos);

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

            if (fill >= 1f)
            {
                target.isFilled = true;
                activeTargetIndex++;

                if (activeTargetIndex < fillTargets.Count)
                {
                    var next = fillTargets[activeTargetIndex];
                    if (next.image != null)
                    {
                        RectTransform prevRT = target.image.rectTransform;
                        RectTransform nextRT = next.image.rectTransform;

                        int nextOrigin = GetNearestOrigin(next.fillAxis, prevRT, nextRT);
                        next.image.fillOrigin = nextOrigin;
                        imageFillOrigins[next.image] = nextOrigin;
                    }
                }
            }

            break;
        }
    }

    void EndDrag()
    {
        isDragging = false;
    }

    public void ResetAllFills()
    {
        foreach (var target in fillTargets)
        {
            if (target.image != null)
            {
                target.image.fillAmount = 0f;
                target.isFilled = false;
            }
        }

        activeTargetIndex = 0;
        isDragging = false;
    }

    // Checks if pointer is on the current image
    bool IsPointerOverTargetImage(Vector2 screenPos)
    {
        if (activeTargetIndex >= fillTargets.Count)
            return false;

        var target = fillTargets[activeTargetIndex];
        if (target.image == null)
            return false;

        RectTransform rt = target.image.rectTransform;
        return RectTransformUtility.RectangleContainsScreenPoint(rt, screenPos);
    }

    int GetNearestOrigin(FillTarget.Axis axis, Vector2 screenPos, RectTransform rectTransform)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPos, null, out Vector2 localPoint);
        Vector2 size = rectTransform.rect.size;

        if (axis == FillTarget.Axis.Horizontal)
        {
            float distToLeft = Mathf.Abs(localPoint.x + size.x * 0.5f);
            float distToRight = Mathf.Abs(localPoint.x - size.x * 0.5f);
            return distToLeft < distToRight ? 0 : 1;
        }
        else
        {
            float distToBottom = Mathf.Abs(localPoint.y + size.y * 0.5f);
            float distToTop = Mathf.Abs(localPoint.y - size.y * 0.5f);
            return distToBottom < distToTop ? 0 : 1;
        }
    }

    int GetNearestOrigin(FillTarget.Axis axis, RectTransform lastFilled, RectTransform nextToFill)
    {
        Vector3 fromCenter = lastFilled.TransformPoint(lastFilled.rect.center);
        Vector3 toCenter = nextToFill.TransformPoint(nextToFill.rect.center);
        Vector3 worldDir = toCenter - fromCenter;

        Vector2 localDir = nextToFill.InverseTransformDirection(worldDir);

        if (axis == FillTarget.Axis.Horizontal)
        {
            return localDir.x >= 0 ? 0 : 1;
        }
        else
        {
            return localDir.y >= 0 ? 0 : 1;
        }
    }
}
