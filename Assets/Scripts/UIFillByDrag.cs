using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIFillMultiImagesByDrag : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private List<Image> fillImages = new List<Image>();

    private Image currentImage;
    private RectTransform currentRectTransform;
    private bool isDragging = false;
    private bool fillFromLeft = true;

    private Dictionary<string, ImagesOverlapedMetaData> imagesOverlapedMetaDatas;

    private Rect mainImageRect;
    private Rect otherImageRect;

    Vector3[] imageCorners;
    List <Vector3> imageEdges;
    List<Vector3> mainImageEdges;
    Vector3 mainImageLeftEdge;
    Vector3 mainImageRightEdge;
    List<Vector3> otherImageEdges;
    Vector3 otherImageLeftEdge;
    Vector3 otherImageRightEdge;

    private Image mainImage;

    float distanceMainRectLeftAndOtherRectLeft;
    float distanceMainRectLeftAndOtherRectRight;
    float distanceMainRectRightAndOtherRectLeft;
    float distanceMainRectRightAndOtherRectRight;

    Dictionary<string, OverlapedImageInfo> nextToBeSelectedFormImages;
   // private List<Image> nextToBeSelectedFormImages;
    private bool isFirstFilled;
    private Image imageBeingFilled;

    // New variable to control image switching
    //private bool canSelectNextImageToFill = false;

    void Start()
    {
        GetAllFillAbleImages();
        ImageHavingSetup();
    }

    private void GetAllFillAbleImages() 
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

    private void ImageHavingSetup()
    {
        imagesOverlapedMetaDatas = new Dictionary<string, ImagesOverlapedMetaData>();

        for (int i = 0; i < fillImages.Count; i++)
        {
            ImagesOverlapedMetaData imagesOverlapedMetaData = new ImagesOverlapedMetaData();
            imagesOverlapedMetaData.allImagesOverlaped = new List<Image>();
            imagesOverlapedMetaData.allImagesOverlapedOnLeft = new Dictionary<string, OverlapedImageInfo>();
            imagesOverlapedMetaData.allImagesOverlapedOnRight = new Dictionary<string, OverlapedImageInfo>();

            mainImage = fillImages[i].GetComponent<Image>();
            mainImageRect = GetWorldRect(mainImage.GetComponent<RectTransform>());

            mainImageEdges = GetEdgesPosition(mainImage.GetComponent<RectTransform>());
            mainImageLeftEdge = mainImageEdges[0];
            mainImageRightEdge = mainImageEdges[1];

          //  Debug.Log(mainImage.name + " mainRectLeftEdge = " + mainImageLeftEdge);
          // Debug.Log(mainImage.name + " mainRectRightEdge = " + mainImageRightEdge);

            for (int j = 0; j < fillImages.Count; j++)
            {
                if (fillImages[j].name != mainImage.name)
                {
                    otherImageRect = GetWorldRect(fillImages[j].GetComponent<RectTransform>());
                    otherImageEdges = GetEdgesPosition(fillImages[j].GetComponent<RectTransform>());

                    if (mainImageRect.Overlaps(otherImageRect, true))
                    {
                       // Debug.Log(mainImage.name + " " + fillImages[j].name);

                        imagesOverlapedMetaData.allImagesOverlaped.Add(fillImages[j]);

                        // Left Right overlaping Info storage
                        OverlapedImageInfo overlapedImageInfo = new OverlapedImageInfo();

                        otherImageLeftEdge = otherImageEdges[0];
                        otherImageRightEdge = otherImageEdges[1];

                       // Debug.Log(fillImages[j].name +" otherRectLeftEdge = " + otherImageLeftEdge);
                       // Debug.Log(fillImages[j].name +" otherRectRightEdge = " + otherImageRightEdge);

                        //mainLeft and otherleft distance
                        distanceMainRectLeftAndOtherRectLeft = Vector3.Distance(mainImageLeftEdge , otherImageLeftEdge);
                        distanceMainRectLeftAndOtherRectRight = Vector3.Distance(mainImageLeftEdge , otherImageRightEdge);
                        distanceMainRectRightAndOtherRectLeft = Vector3.Distance(mainImageRightEdge , otherImageLeftEdge);
                        distanceMainRectRightAndOtherRectRight = Vector3.Distance(mainImageRightEdge , otherImageRightEdge);

                        // Find smallest
                        float[] distances = {
                            distanceMainRectLeftAndOtherRectLeft,
                            distanceMainRectLeftAndOtherRectRight,
                            distanceMainRectRightAndOtherRectLeft,
                            distanceMainRectRightAndOtherRectRight
                        };

                        float minDistance = Mathf.Min(distances);

                        // If MainRectLeft And OtherRectLeft Overlaped
                        if (minDistance == distanceMainRectLeftAndOtherRectLeft)
                        {
                            //Debug.Log(fillImages[j].name + "Smallest: MainRect LEFT / OtherRect LEFT, Distance = " + minDistance);
                            overlapedImageInfo.overlapedSide = OverlapedSide.LEFT;
                            imagesOverlapedMetaData.allImagesOverlapedOnLeft.Add(fillImages[j].name, overlapedImageInfo);
                        }
                        // If MainRectLeft And OtherRectRight Overlaped
                        else if (minDistance == distanceMainRectLeftAndOtherRectRight)
                        {
                            //Debug.Log(fillImages[j].name + "Smallest: MainRect LEFT / OtherRect RIGHT, Distance = " + minDistance);
                            overlapedImageInfo.overlapedSide = OverlapedSide.RIGHT;
                            imagesOverlapedMetaData.allImagesOverlapedOnLeft.Add(fillImages[j].name, overlapedImageInfo);
                        }
                        // If MainRectLeft And OtherRectLeft Overlaped
                        else if (minDistance == distanceMainRectRightAndOtherRectLeft)
                        {
                            //Debug.Log(fillImages[j].name + "Smallest: MainRect RIGHT / OtherRect LEFT, Distance = " + minDistance);
                            overlapedImageInfo.overlapedSide = OverlapedSide.LEFT;
                            imagesOverlapedMetaData.allImagesOverlapedOnRight.Add(fillImages[j].name, overlapedImageInfo);
                        }
                        // If MainRectLeft And OtherRectLeft Overlaped
                        else
                        {
                            //Debug.Log(fillImages[j].name + "Smallest: MainRect RIGHT / OtherRect RIGHT, Distance = " + minDistance);
                            overlapedImageInfo.overlapedSide = OverlapedSide.RIGHT;
                            imagesOverlapedMetaData.allImagesOverlapedOnRight.Add(fillImages[j].name, overlapedImageInfo);
                        }
                    }
                }
            }

            imagesOverlapedMetaDatas.Add(mainImage.name, imagesOverlapedMetaData);
        }
    }

    Rect GetWorldRect(RectTransform rt)
    {
        imageCorners = new Vector3[4];
        rt.GetWorldCorners(imageCorners);

        return new Rect(imageCorners[0].x, imageCorners[0].y, imageCorners[2].x - imageCorners[0].x, imageCorners[2].y - imageCorners[0].y);
    }

    List <Vector3> GetEdgesPosition(RectTransform rt)
    {
        imageCorners = new Vector3[4];
        rt.GetWorldCorners(imageCorners);

        imageEdges = new List<Vector3>();
        imageEdges.Add((imageCorners[0] + imageCorners[1]) / 2);
        imageEdges.Add((imageCorners[2] + imageCorners[3]) / 2);

        return imageEdges;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isFirstFilled = false;
        Image hitImage = GetImageUnderPointer(eventData);
        if (hitImage != null)
        {
            currentImage = hitImage;
            currentRectTransform = currentImage.GetComponent<RectTransform>();
            isDragging = true;
            DetectFillDirection(eventData);
            imageBeingFilled = hitImage;
            SetNextImagesToBeSelectedWhom(imageBeingFilled);
            UpdateFill(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Image hitImage = GetImageUnderPointer(eventData);

        if (hitImage != null && hitImage != currentImage && hitImage.fillAmount < 1)
        {
            if (!fillImages.Contains(hitImage)) return;

            if (!nextToBeSelectedFormImages.ContainsKey(hitImage.name) && isFirstFilled) return;

            if (currentImage.fillAmount >= 0.8f)
            {
                currentImage.fillAmount = 1.0f;
                isFirstFilled = true;
                imageBeingFilled = currentImage;
                SetNextImagesToBeSelectedWhom(imageBeingFilled);
            }
            else
            {
                currentImage.fillAmount = 0.0f;
            }

            currentImage = hitImage;
            currentRectTransform = currentImage.GetComponent<RectTransform>();
           // DetectFillDirection(eventData);
        }

        UpdateFill(eventData);
    }

    private void SetNextImagesToBeSelectedWhom(Image imageToSetNext)
    {
        nextToBeSelectedFormImages = new Dictionary<string, OverlapedImageInfo>();
        //To Right
        if (!fillFromLeft)
        {
            nextToBeSelectedFormImages = imagesOverlapedMetaDatas[imageToSetNext.name].allImagesOverlapedOnLeft;
        }
        //To Left
        else
        {
            nextToBeSelectedFormImages = imagesOverlapedMetaDatas[imageToSetNext.name].allImagesOverlapedOnRight;
        }
        foreach (KeyValuePair<string, OverlapedImageInfo> kvp in nextToBeSelectedFormImages)
        {
            //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            Debug.Log("Next iamges = " + kvp.Key);
        }
    }

    private void SetNextImagesToBeSelectedWhomDrag(Image imageToSetNext)
    {
        nextToBeSelectedFormImages = new Dictionary<string, OverlapedImageInfo>();
        //To Right
        if (!fillFromLeft)
        {
            nextToBeSelectedFormImages = imagesOverlapedMetaDatas[imageToSetNext.name].allImagesOverlapedOnLeft;
        }
        //To Left
        else
        {
            nextToBeSelectedFormImages = imagesOverlapedMetaDatas[imageToSetNext.name].allImagesOverlapedOnRight;
        }

        for (int i = 0; i <= fillImages.Count;i++) 
        {

        }
        foreach (KeyValuePair<string, OverlapedImageInfo> kvp in nextToBeSelectedFormImages)
        {
            //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            Debug.Log("Next iamges = " + kvp.Key);
        }
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

            if (currentImage.fillAmount >= 0.8f)
            {
                imageBeingFilled = currentImage;
                SetNextImagesToBeSelectedWhom(imageBeingFilled);
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
        if (UIManager.GetInstance().GameManager.IsVibrationOff == 0)
        {
            // Short vibration cross-platform
            Handheld.Vibrate();
        }
        foreach (Image image in fillImages)
        {
            image.fillAmount = 0;
        }
        SetLevelCompletionBar();
    }

    private void PuzzleSuccess()
    {
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


    public class ImagesOverlapedMetaData
    {
        public List<Image> allImagesOverlaped = new List<Image>();
        public Dictionary<string, OverlapedImageInfo> allImagesOverlapedOnLeft = new Dictionary<string, OverlapedImageInfo>();
        public Dictionary<string, OverlapedImageInfo> allImagesOverlapedOnRight = new Dictionary<string, OverlapedImageInfo>();
    }

    public class OverlapedImageInfo
    {
        public OverlapedSide overlapedSide; // other image side
    }

    public enum OverlapedSide 
    {
        RIGHT,
        LEFT,
    }
}
