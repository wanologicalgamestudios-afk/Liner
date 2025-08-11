using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SmartMultiImageFill : MonoBehaviour
{
    [System.Serializable]
    public class FillTarget
    {
       // public string name;
        public Image image;
        public enum Axis { Horizontal, Vertical }
        public Axis fillAxis = Axis.Horizontal;

        [HideInInspector] public bool isFilled = false;
    }

    public List<FillTarget> fillTargets = new List<FillTarget>();

    private Vector2 dragStartScreenPos;
    private bool isDragging = false;
    private Dictionary<Image, int> imageFillOrigins = new Dictionary<Image, int>();

    public string currentImageName = ""; // Public string for current image name under drag

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

        UpdateCurrentImageName(screenPos);
    }

    void UpdateFill(Vector2 screenPos)
    {
        UpdateCurrentImageName(screenPos);

        foreach (var target in fillTargets)
        {
            if (target.image == null || target.isFilled)
                continue;

            RectTransform rt = target.image.rectTransform;
            if (!RectTransformUtility.RectangleContainsScreenPoint(rt, screenPos))
                continue;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rt, screenPos, null, out Vector2 localPos);

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

            int origin = imageFillOrigins.ContainsKey(target.image) ? imageFillOrigins[target.image] : GetNearestOrigin(target.fillAxis, screenPos, rt);
            imageFillOrigins[target.image] = origin;
            if (origin == 1)
                fill = 1f - fill;

            target.image.fillOrigin = origin;
            target.image.fillAmount = fill;

            if (fill >= 0.98f)
            {
                target.image.fillAmount = 1f; // Snap to 100%
                target.isFilled = true;
            }

            break; // Only fill one image at a time
        }
    }

    void EndDrag()
    {
        isDragging = false;

        bool allFilled = true;
        foreach (var target in fillTargets)
        {
            if (!target.isFilled)
            {
                allFilled = false;
                break;
            }
        }

        if (allFilled)
            OnPuzzleComplete();
        else
            OnFailed();
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

        isDragging = false;
        currentImageName = ""; // Reset current image name
    }

    void UpdateCurrentImageName(Vector2 screenPos)
    {
        foreach (var target in fillTargets)
        {
            if (target.image == null)
                continue;

            RectTransform rt = target.image.rectTransform;
            if (RectTransformUtility.RectangleContainsScreenPoint(rt, screenPos))
            {
                currentImageName = target.image.gameObject.name;
                return;
            }
        }

        currentImageName = "";
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

    void OnPuzzleComplete()
    {
        Debug.Log("Puzzle Completed!");
    }

    void OnFailed()
    {
        Debug.Log("Puzzle Failed. Resetting...");
        ResetAllFills();
    }
}
