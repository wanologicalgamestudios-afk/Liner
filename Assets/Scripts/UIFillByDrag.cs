using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIFillMultiImagesByDrag : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField]
    private List<Image> fillImages = new List<Image>();

    private Image currentImage;
    private RectTransform currentRectTransform;
    private bool isDragging = false;
    private bool fillFromLeft = true;

    // New variable to control image switching
    private bool canSelectNextImageToFill = false;

    void Start()
    {
        if (fillImages.Count == 0)
        {
            foreach (Transform child in transform)
            {
                Image img = child.GetComponent<Image>();
                if (img != null)
                {
                    img.fillAmount = 0f;
                    fillImages.Add(img);
                }
            }
        }

        if (fillImages.Count == 0)
        {
            Debug.LogError("No Image components found.");
            enabled = false;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Image hitImage = GetImageUnderPointer(eventData);
        // if (hitImage != null && !lockedImages.Contains(hitImage))
        if (hitImage != null)
        {
            currentImage = hitImage;
            currentRectTransform = currentImage.GetComponent<RectTransform>();
            isDragging = true;
            DetectFillDirection(eventData);
            UpdateFill(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Image hitImage = GetImageUnderPointer(eventData);

        if (hitImage != null)
        {
            if (hitImage.fillAmount >= 1) 
            {
                hitImage = null;
            }
        }

        // if (hitImage != null && hitImage != currentImage && !lockedImages.Contains(hitImage))
        if (hitImage != null && hitImage != currentImage)
        {
            // Only allow switching if the flag is true
            if (canSelectNextImageToFill)
            {
                currentImage = hitImage;
                currentRectTransform = currentImage.GetComponent<RectTransform>();
                DetectFillDirection(eventData);
                canSelectNextImageToFill = false; // reset for the new image
            }
        }

        UpdateFill(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        CheckPuzzleStatus();
    }

    private Image GetImageUnderPointer(PointerEventData eventData)
    {
        foreach (Image img in fillImages)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(img.rectTransform, eventData.position, eventData.pressEventCamera))
            {
                return img;
            }
        }
        return null;
    }

    private void DetectFillDirection(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            currentImage.rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        if (currentImage.fillMethod == Image.FillMethod.Horizontal)
        {
            fillFromLeft = (localPoint.x < 0);
            currentImage.fillOrigin = fillFromLeft ? (int)Image.OriginHorizontal.Left : (int)Image.OriginHorizontal.Right;
        }
        else if (currentImage.fillMethod == Image.FillMethod.Vertical)
        {
            bool fillFromBottom = (localPoint.y < 0);
            currentImage.fillOrigin = fillFromBottom ? (int)Image.OriginVertical.Bottom : (int)Image.OriginVertical.Top;
        }
    }

    private void UpdateFill(PointerEventData eventData)
    {
        // if (currentImage == null || lockedImages.Contains(currentImage)) return;
        if (currentImage == null) return;


        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            currentImage.rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint))
        {
            float amount = 0f;

            if (currentImage.fillMethod == Image.FillMethod.Horizontal)
            {
                float width = currentRectTransform.rect.width;
                float normalizedX = Mathf.Clamp01((localPoint.x + width * 0.5f) / width);
                amount = fillFromLeft ? normalizedX : 1f - normalizedX;
            }
            else if (currentImage.fillMethod == Image.FillMethod.Vertical)
            {
                float height = currentRectTransform.rect.height;
                float normalizedY = Mathf.Clamp01((localPoint.y + height * 0.5f) / height);
                amount = currentImage.fillOrigin == (int)Image.OriginVertical.Bottom ? normalizedY : 1f - normalizedY;
            }

            currentImage.fillAmount = Mathf.Clamp01(amount);
            SetLevelCompletionBar();

            // Update the flag for switching
            canSelectNextImageToFill = currentImage.fillAmount >= 0.9f;

            if (canSelectNextImageToFill) 
            {
                currentImage.fillAmount = 1.0f;
                canSelectNextImageToFill = true;
            }

            if (currentImage.fillAmount >= 1.0f)
            {
                currentImage.fillAmount = 1.0f;
                canSelectNextImageToFill = true;
            }
        }
    }

    private void CheckPuzzleStatus()
    {
        foreach (Image image in fillImages)
        {
            if (image.fillAmount < 1f)
            {
                PuzzleFail();
                return;
            }
        }
        PuzzleSuccess();
    }

    private float CombineFillAmout() 
    {
        float combineFillAmout = 0;

        foreach (Image image in fillImages) 
        {
            combineFillAmout = combineFillAmout + image.fillAmount;
        }
        combineFillAmout = combineFillAmout / fillImages.Count;

        return combineFillAmout;
    }

    private void PuzzleFail()
    {
        Debug.Log("Puzzle failed — resetting images.");
        if (UIManager.GetInstance().GameManager.IsVibrationOff == 0) 
        {
            // Short vibration cross-platform
            Handheld.Vibrate();
        }


        Debug.Log("Puzzle failed — resetting images.");
        foreach (Image image in fillImages)
        {
            image.fillAmount = 0;
        }
        SetLevelCompletionBar();
    }

    private void PuzzleSuccess()
    {
        Debug.Log("Puzzle solved!");
        LoadNextLevel();
    }

    private void LoadNextLevel()
    {
        if (UIManager.GetInstance().GetCurrentPanel().GetComponent<GamePlayUI>())
            UIManager.GetInstance().GetCurrentPanel().GetComponent<GamePlayUI>().LoadNextLevel();
    }

    private void SetLevelCompletionBar() 
    {
        if (UIManager.GetInstance().GetCurrentPanel().GetComponent<GamePlayUI>())
            UIManager.GetInstance().GetCurrentPanel().GetComponent<GamePlayUI>().SetLevelCompletionBar(CombineFillAmout());
    }
}
