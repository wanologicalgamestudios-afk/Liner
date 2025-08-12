using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIFillMultiImagesByDrag : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public List<Image> fillImages = new List<Image>();
    public bool canSelectNextToFillUp = false;

    private int currentIndex = 0;
    private RectTransform currentRectTransform;
    private bool isDragging = false;
    private bool fillFromLeft = true;

    void Start()
    {
        // Automatically get all child images
        fillImages.Clear();
        foreach (Transform child in transform)
        {
            Image img = child.GetComponent<Image>();
            if (img != null)
            {
                img.fillAmount = 0f; // Reset
                fillImages.Add(img);
            }
        }

        if (fillImages.Count == 0)
        {
            Debug.LogError("No child Image components found.");
            enabled = false;
            return;
        }

        currentIndex = 0;
        SetupCurrentImage();
    }

    private void SetupCurrentImage()
    {
        currentRectTransform = fillImages[currentIndex].GetComponent<RectTransform>();
        canSelectNextToFillUp = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (currentIndex >= fillImages.Count)
            return;

        isDragging = true;
        DetectFillDirection(eventData);
        UpdateFill(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging)
            return;

        // Detect if pointer moved into next image
        if (currentIndex < fillImages.Count)
        {
            RectTransform rt = fillImages[currentIndex].rectTransform;
            if (!RectTransformUtility.RectangleContainsScreenPoint(rt, eventData.position, eventData.pressEventCamera))
            {
                // If pointer leaves current image and it's filled enough
                if (canSelectNextToFillUp && currentIndex < fillImages.Count - 1)
                {
                    currentIndex++;
                    SetupCurrentImage();
                    DetectFillDirection(eventData);
                }
            }
        }

        UpdateFill(eventData);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        CheckPuzzleStatus();
    }

    private void DetectFillDirection(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            fillImages[currentIndex].rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        if (fillImages[currentIndex].fillMethod == Image.FillMethod.Horizontal)
        {
            fillFromLeft = (localPoint.x < 0);
            fillImages[currentIndex].fillOrigin = fillFromLeft ? (int)Image.OriginHorizontal.Left : (int)Image.OriginHorizontal.Right;
        }
        else if (fillImages[currentIndex].fillMethod == Image.FillMethod.Vertical)
        {
            bool fillFromBottom = (localPoint.y < 0);
            fillImages[currentIndex].fillOrigin = fillFromBottom ? (int)Image.OriginVertical.Bottom : (int)Image.OriginVertical.Top;
        }
    }

    private void UpdateFill(PointerEventData eventData)
    {
        if (currentIndex >= fillImages.Count) return;

        Vector2 localPoint;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            fillImages[currentIndex].rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint))
        {
            float amount = 0f;

            if (fillImages[currentIndex].fillMethod == Image.FillMethod.Horizontal)
            {
                float width = currentRectTransform.rect.width;
                float normalizedX = Mathf.Clamp01((localPoint.x + width * 0.5f) / width);
                amount = fillFromLeft ? normalizedX : 1f - normalizedX;
            }
            else if (fillImages[currentIndex].fillMethod == Image.FillMethod.Vertical)
            {
                float height = currentRectTransform.rect.height;
                float normalizedY = Mathf.Clamp01((localPoint.y + height * 0.5f) / height);
                amount = fillImages[currentIndex].fillOrigin == (int)Image.OriginVertical.Bottom ? normalizedY : 1f - normalizedY;
            }

            fillImages[currentIndex].fillAmount = amount;
            canSelectNextToFillUp = (fillImages[currentIndex].fillAmount >= 0.9f);
            if (canSelectNextToFillUp) 
            {
                fillImages[currentIndex].fillAmount = 1;
            }
        }
    }

    private void CheckPuzzleStatus() 
    {
        bool isPuzzleSolved = false;

        foreach (Image image in fillImages) 
        {
            if (image.fillAmount < 1)
            {
                Debug.Log("fail" + image.transform.name + " amount = "+ image.fillAmount);
              
                isPuzzleSolved = false;
                break;
            }
            else 
            {
                isPuzzleSolved = true ;
            }
        }

        if (!isPuzzleSolved) 
        {
            PuzzleFail();
        }
        else 
        {
            PuzzpleSuccess();
        }
    }

    private void PuzzleFail() 
    {
        foreach (Image image in fillImages) 
        {
            image.fillAmount = 0;
        }

        Debug.Log("puzzle fail");
    }
    private void PuzzpleSuccess() 
    {
        Debug.Log("Puzzle solved");
    }
}
