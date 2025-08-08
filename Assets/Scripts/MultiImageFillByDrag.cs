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
        isDragging = true;
        dragStartScreenPos = screenPos;
        imageFillOrigins.Clear();
        activeTargetIndex = 0;

        // Set initial fill origin for index 0
        if (fillTargets.Count > 0 && fillTargets[0].image != null)
        {
            var image = fillTargets[0].image;
            RectTransform rt = image.rectTransform;
            Vector3[] worldCorners = new Vector3[4];
            rt.GetWorldCorners(worldCorners);

            int origin = GetNearestOrigin(fillTargets[0].fillAxis, screenPos, worldCorners);
            imageFillOrigins[image] = origin;
            image.fillOrigin = origin;
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

                        Vector3[] prevCorners = new Vector3[4];
                        Vector3[] nextCorners = new Vector3[4];
                        prevRT.GetWorldCorners(prevCorners);
                        nextRT.GetWorldCorners(nextCorners);

                        int nextOrigin = GetNearestOrigin(next.fillAxis, prevCorners, nextCorners);
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
    }

    int GetNearestOrigin(FillTarget.Axis axis, Vector2 screenPos, Vector3[] corners)
    {
        if (axis == FillTarget.Axis.Horizontal)
        {
            float distToLeft = Mathf.Abs(screenPos.x - corners[0].x);
            float distToRight = Mathf.Abs(screenPos.x - corners[3].x);
            return distToLeft < distToRight ? 0 : 1;
        }
        else
        {
            float distToBottom = Mathf.Abs(screenPos.y - corners[0].y);
            float distToTop = Mathf.Abs(screenPos.y - corners[1].y);
            return distToBottom < distToTop ? 0 : 1;
        }
    }

    int GetNearestOrigin(FillTarget.Axis axis, Vector3[] from, Vector3[] to)
    {
        if (axis == FillTarget.Axis.Horizontal)
        {
            float distToLeft = Mathf.Abs(from[3].x - to[0].x);
            float distToRight = Mathf.Abs(from[0].x - to[3].x);
            return distToLeft < distToRight ? 0 : 1;
        }
        else
        {
            float distToBottom = Mathf.Abs(from[1].y - to[0].y);
            float distToTop = Mathf.Abs(from[0].y - to[1].y);
            return distToBottom < distToTop ? 0 : 1;
        }
    }
}
