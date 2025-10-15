using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIFillMultiImagesByDrag : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private List<Image> fillImages = new List<Image>();

    private Image currentImage;
    private RectTransform currentRectTransform;
    private bool isDragging = false;
    private bool fillFromLeft = true;

    private Image mainImage;
    private Image offDrageNextImage;

    private RectTransform mainImageRectTransform;
    private RectTransform otherImageRectTransform;

    private Dictionary<string, ImagesOverlapedMetaData> imagesOverlapedMetaDatas;
    private Dictionary<string, OverlapedImageInfo> nextToBeSelectedFromImages;

    Vector3[] imageCorners;
    List <Vector3> imageEdges;
    List <Vector3> mainImageEdges;
    Vector3 mainImageLeftEdge;
    Vector3 mainImageRightEdge;
    List<Vector3> otherImageEdges;
    Vector3 otherImageLeftEdge;
    Vector3 otherImageRightEdge;

    float distanceMainRectLeftAndOtherRectLeft;
    float distanceMainRectLeftAndOtherRectRight;
    float distanceMainRectRightAndOtherRectLeft;
    float distanceMainRectRightAndOtherRectRight;

    private bool isFirstFilled;
    private Image imageBeingFilled;
    private int unfilledImagesCount;
    private Image unfilledImageForCheck;

    private Vector2 lastPointerPosition;

    private GameObject drawingToShake;


    void Start()
    {
        GetAllFillAbleImages();
        ImageOverlapingSetup();

        
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

    private void ImageOverlapingSetup()
    {
        imagesOverlapedMetaDatas = new Dictionary<string, ImagesOverlapedMetaData>();

        for (int i = 0; i < fillImages.Count; i++)
        {
            ImagesOverlapedMetaData imagesOverlapedMetaData = new ImagesOverlapedMetaData();
            imagesOverlapedMetaData.allImagesOverlapedOnLeft = new Dictionary<string, OverlapedImageInfo>();
            imagesOverlapedMetaData.allImagesOverlapedOnRight = new Dictionary<string, OverlapedImageInfo>();

            mainImage = fillImages[i].GetComponent<Image>();
            mainImageRectTransform = mainImage.GetComponent<RectTransform>();

            mainImageEdges = GetEdgesPosition(mainImage.GetComponent<RectTransform>());
            mainImageLeftEdge = mainImageEdges[0];
            mainImageRightEdge = mainImageEdges[1];


            for (int j = 0; j < fillImages.Count; j++)
            {
                if (fillImages[j].name != mainImage.name)
                {
                    otherImageRectTransform = fillImages[j].GetComponent<RectTransform>();


                    if (IsOverlaped(mainImageRectTransform, otherImageRectTransform))
                    {
                       // Debug.Log(mainImage.name + " " + fillImages[j].name);

                        // Left Right overlaping Info storage
                        OverlapedImageInfo overlapedImageInfo = new OverlapedImageInfo();

                        otherImageEdges = GetEdgesPosition(fillImages[j].GetComponent<RectTransform>());
                        otherImageLeftEdge = otherImageEdges[0];
                        otherImageRightEdge = otherImageEdges[1];

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
                          //  Debug.Log(fillImages[j].name + "Smallest: MainRect LEFT / OtherRect LEFT, Distance = " + minDistance);
                            overlapedImageInfo.overlapedSide = OverlapedSide.LEFT;
                            imagesOverlapedMetaData.allImagesOverlapedOnLeft.Add(fillImages[j].name, overlapedImageInfo);
                        }
                        // If MainRectLeft And OtherRectRight Overlaped
                        else if (minDistance == distanceMainRectLeftAndOtherRectRight)
                        {
                          //  Debug.Log(fillImages[j].name + "Smallest: MainRect LEFT / OtherRect RIGHT, Distance = " + minDistance);
                            overlapedImageInfo.overlapedSide = OverlapedSide.RIGHT;
                            imagesOverlapedMetaData.allImagesOverlapedOnLeft.Add(fillImages[j].name, overlapedImageInfo);
                        }
                        // If MainRectLeft And OtherRectLeft Overlaped
                        else if (minDistance == distanceMainRectRightAndOtherRectLeft)
                        {
                           // Debug.Log(fillImages[j].name + "Smallest: MainRect RIGHT / OtherRect LEFT, Distance = " + minDistance);
                            overlapedImageInfo.overlapedSide = OverlapedSide.LEFT;
                            imagesOverlapedMetaData.allImagesOverlapedOnRight.Add(fillImages[j].name, overlapedImageInfo);
                        }
                        // If MainRectLeft And OtherRectLeft Overlaped
                        else
                        {
                           // Debug.Log(fillImages[j].name + "Smallest: MainRect RIGHT / OtherRect RIGHT, Distance = " + minDistance);
                            overlapedImageInfo.overlapedSide = OverlapedSide.RIGHT;
                            imagesOverlapedMetaData.allImagesOverlapedOnRight.Add(fillImages[j].name, overlapedImageInfo);
                        }
                    }
                }
            }

            imagesOverlapedMetaDatas.Add(mainImage.name, imagesOverlapedMetaData);
        }
    }

    private Vector3[] GetWorldCornersArray(RectTransform rt)
    {
        imageCorners = new Vector3[4];
        rt.GetWorldCorners(imageCorners);
        return imageCorners;
    }

    // Polygon intersection test (Separating Axis Theorem)
    private bool IsOverlaped(RectTransform a, RectTransform b)
    {
        return PolygonsIntersect(GetWorldCornersArray(a), GetWorldCornersArray(b));
    }

    private bool PolygonsIntersect(Vector3[] poly1, Vector3[] poly2)
    {
        return !(IsSeparated(poly1, poly2) || IsSeparated(poly2, poly1));
    }

    private bool IsSeparated(Vector3[] poly1, Vector3[] poly2)
    {
        for (int i = 0; i < poly1.Length; i++)
        {
            Vector2 p1 = poly1[i];
            Vector2 p2 = poly1[(i + 1) % poly1.Length];

            // Get axis perpendicular to edge
            Vector2 axis = new Vector2(-(p2.y - p1.y), p2.x - p1.x).normalized;

            // Project both polygons onto axis
            ProjectPolygon(axis, poly1, out float min1, out float max1);
            ProjectPolygon(axis, poly2, out float min2, out float max2);

            // If no overlap -> polygons don’t intersect
            if (max1 < min2 || max2 < min1)
                return true;
        }
        return false;
    }

    private void ProjectPolygon(Vector2 axis, Vector3[] poly, out float min, out float max)
    {
        float dot = Vector2.Dot(axis, poly[0]);
        min = max = dot;

        for (int i = 1; i < poly.Length; i++)
        {
            dot = Vector2.Dot(axis, poly[i]);
            if (dot < min) min = dot;
            if (dot > max) max = dot;
        }
    }

    Rect GetWorldRect(RectTransform rt)
    {
        imageCorners = new Vector3[4];
        rt.GetWorldCorners(imageCorners);
        //for (var i = 0; i < 4; i++)
        //{
        //    Debug.Log(this.transform.name + " World Corner " + i + " : " + imageCorners[i]);
        //}
        //Debug.Log(imageCorners[2].x - imageCorners[0].x);
        //Debug.Log(imageCorners[2].y - imageCorners[0].y);
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
        lastPointerPosition = eventData.position;

        isFirstFilled = false;
        Image hitImage = GetImageUnderPointer(eventData);
        if (hitImage != null)
        {
            currentImage = hitImage;
            currentImage.transform.SetAsFirstSibling();
            currentRectTransform = currentImage.GetComponent<RectTransform>();
            isDragging = true;
            DetectFillDirectionGeneral(eventData);
            imageBeingFilled = hitImage;
            SetNextImagesToBeSelectedWhomOnDragStart(imageBeingFilled);
            UpdateFill(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        Image hitImage = GetImageUnderPointer(eventData);

        if (currentImage.fillAmount < 1)
        {
            if (fillImages.Contains(hitImage) && hitImage != currentImage && hitImage.fillAmount != 1)
            {
                if (currentImage.fillAmount >= 0.6f)
                {
                    currentImage.fillAmount = 1.0f;
                    isFirstFilled = true;
                    imageBeingFilled = currentImage;
                    SetNextImagesToBeSelectedWhomOnDrag(imageBeingFilled);
                }
                else
                {
                    currentImage.fillAmount = 0.0f;
                }

                if (!nextToBeSelectedFromImages.ContainsKey(hitImage.name) && isFirstFilled) return;

                currentImage = hitImage;
                currentImage.transform.SetAsFirstSibling();

                if (!isFirstFilled)
                {
                    DetectFillDirectionGeneral(eventData);
                }
                else
                {
                    DetectFillDirectionNormal();
                }

                currentRectTransform = currentImage.GetComponent<RectTransform>();
            }
        }
        else if (currentImage.fillAmount >= 1)
        {
            currentImage.fillAmount = 1.0f;
            isFirstFilled = true;
            imageBeingFilled = currentImage;
            SetNextImagesToBeSelectedWhomOnDrag(imageBeingFilled);

            // Direction-aware selection logic starts here
            Vector2 dragVector = (eventData.position - lastPointerPosition).normalized;
            lastPointerPosition = eventData.position;

            Image bestCandidate = null;
            float bestScore = float.MinValue;

            foreach (var kvp in nextToBeSelectedFromImages)
            {
                Image candidate = fillImages.Find(img => img.name == kvp.Key);
                if (candidate == null || candidate.fillAmount == 1) continue;

                Vector2 toCandidate = (Vector2)(candidate.rectTransform.position - currentImage.rectTransform.position).normalized;
                float score = Vector2.Dot(dragVector, toCandidate);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestCandidate = candidate;
                }
            }

            if (bestCandidate == null) return;

            currentImage = bestCandidate;
            currentImage.transform.SetAsFirstSibling();
            DetectFillDirectionNormal();
            currentRectTransform = currentImage.GetComponent<RectTransform>();
        }

        UpdateFill(eventData);
    }


    private void UpdateFill(PointerEventData eventData)
    {
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
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
        CheckPuzzleStatus();
        currentImage = null;
    }

    private void SetNextImagesToBeSelectedWhomOnDragStart(Image imageToSetNext)
    {
        nextToBeSelectedFromImages = new Dictionary<string, OverlapedImageInfo>();
        //To Right
        if (!fillFromLeft)
        {
            nextToBeSelectedFromImages = imagesOverlapedMetaDatas[imageToSetNext.name].allImagesOverlapedOnLeft;
        }
        //To Left
        else
        {
            nextToBeSelectedFromImages = imagesOverlapedMetaDatas[imageToSetNext.name].allImagesOverlapedOnRight;
        }
        foreach (KeyValuePair<string, OverlapedImageInfo> kvp in nextToBeSelectedFromImages)
        {
            //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
           // Debug.Log("Next iamges = " + kvp.Key);
        }
    }

    private void SetNextImagesToBeSelectedWhomOnDrag(Image imageToSetNext)
    {
        nextToBeSelectedFromImages = new Dictionary<string, OverlapedImageInfo>();
        //To Right
        if (!fillFromLeft)
        {
            nextToBeSelectedFromImages = imagesOverlapedMetaDatas[imageToSetNext.name].allImagesOverlapedOnLeft;
        }
        //To Left
        else
        {
            nextToBeSelectedFromImages = imagesOverlapedMetaDatas[imageToSetNext.name].allImagesOverlapedOnRight;
        }

        for (int i = 0; i < fillImages.Count; i++) 
        {
            if (nextToBeSelectedFromImages.ContainsKey(fillImages[i].name))
            {
                if (nextToBeSelectedFromImages[fillImages[i].name].overlapedSide == OverlapedSide.LEFT)
                {
                    fillImages[i].fillOrigin = (int)Image.OriginHorizontal.Left;
                }
                else 
                {
                    fillImages[i].fillOrigin = (int)Image.OriginHorizontal.Right;
                }
               
            }
        }
        foreach (KeyValuePair<string, OverlapedImageInfo> kvp in nextToBeSelectedFromImages)
        {
            //textBox3.Text += ("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
            //Debug.Log("Next iamges = " + kvp.Key);
        }
    }

    private Image GetImageUnderPointer(PointerEventData eventData)
    {
        // List of candidates within finger area
        List<Image> candidates = new List<Image>();

        foreach (Image img in fillImages)
        {
            // Use an expanded hit box — makes touch feel smoother
            RectTransform rt = img.rectTransform;
            Rect rect = rt.rect;
            rect.xMin -= 20f; // expand touch area (tune if needed)
            rect.xMax += 20f;
            rect.yMin -= 20f;
            rect.yMax += 20f;

            Vector2 localPoint;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out localPoint))
            {
                if (rect.Contains(localPoint))
                {
                    candidates.Add(img);
                }
            }
        }

        if (candidates.Count == 0) return null;
        if (candidates.Count == 1) return candidates[0];

        // Choose the closest to the current image (smoother transitions)
        Image best = candidates[0];
        float bestDist = float.MaxValue;

        if (currentImage != null)
        {
            Vector3 currentCenter = currentImage.rectTransform.position;
            foreach (Image img in candidates)
            {
                float dist = Vector3.Distance(currentCenter, img.rectTransform.position);
                if (dist < bestDist)
                {
                    best = img;
                    bestDist = dist;
                }
            }
        }

        return best;
    }


    private void DetectFillDirectionGeneral(PointerEventData eventData)
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

    private void DetectFillDirectionNormal()
    {
        //Direction is already set on letf right overlaping.
        if (currentImage.fillOrigin == (int)Image.OriginHorizontal.Left)
        {
            fillFromLeft = true;
        }
        else
        {
            fillFromLeft = false;
        }
    }

    private void CheckPuzzleStatus()
    {
        unfilledImagesCount = 0;
        for (int i = 0; i < fillImages.Count; i++) 
        {
            if (fillImages[i].fillAmount < 1)
            {
                unfilledImagesCount++;
                // To Store last unfilled image.
                unfilledImageForCheck = fillImages[i];
            }
        }

        if (unfilledImagesCount > 0) 
        {
            if (unfilledImagesCount > 1 || unfilledImageForCheck.fillAmount < 0.7)
            {
                PuzzleFail();
                return;
            }
        }


        //foreach (Image image in fillImages)
        //{
        //    if (image.fillAmount < 1f)
        //    {
        //        PuzzleFail();
        //        return;
        //    }
        //}
        GreenAllImages();
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

    private void RedAllImages()
    {
        foreach (Image image in fillImages)
        {
            image.color = Color.red;
        }
    }

    private void GreenAllImages()
    {
        foreach (Image image in fillImages)
        {
            image.color = Color.green;
        }
    }

    private void ResetAllImages()
    {
        foreach (Image image in fillImages)
        {
            image.fillAmount = 0.0f;
            image.color = Color.white;
        }
        if (UIManager.GetInstance().GetCurrentPanel().GetComponent<GamePlayUI>())
            UIManager.GetInstance().GetCurrentPanel().GetComponent<GamePlayUI>().SetLevelCompletionBar(0.0f);
    }

    private void RestartLevelOnFailLevel() 
    {
        UIManager.GetInstance().BackButtonIsPressed();
        ResetAllImages();
    }

    private void ActiveLevelFailUI()
    {
        UIManager.GetInstance().SpawnNextPanel(nameof(LevelFailUI),false);
        Invoke(nameof(RestartLevelOnFailLevel), 1.0f);
    }

    private void PuzzleFail()
    {
        if (UIManager.GetInstance().GetCurrentPanel().GetComponent<GamePlayUI>())
            UIManager.GetInstance().GetCurrentPanel().GetComponent<GamePlayUI>().OnLevelFail();
        RedAllImages();

        drawingToShake = GameObject.Find("Middle");
        StartCoroutine(ShakeObject(drawingToShake, 1f, 5f)); // 1 sec shake, low intensity

        ActiveLevelFailUI();
    }

    private void PuzzleSuccess()
    {
        if (UIManager.GetInstance().GetCurrentPanel().GetComponent<GamePlayUI>())
            UIManager.GetInstance().GetCurrentPanel().GetComponent<GamePlayUI>().OnPuzzleSuccessfull();
    }

    private void SetLevelCompletionBar()
    {
        if (UIManager.GetInstance().GetCurrentPanel().GetComponent<GamePlayUI>())
            UIManager.GetInstance().GetCurrentPanel().GetComponent<GamePlayUI>().SetLevelCompletionBar(CombineFillAmout());
    }

    IEnumerator ShakeObject(GameObject target, float duration = 1f, float magnitude = 5f)
    {
        if (target == null) yield break;

        RectTransform rectTransform = target.GetComponent<RectTransform>();
        Vector3 originalPos = rectTransform.anchoredPosition;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;

            rectTransform.anchoredPosition = originalPos + new Vector3(offsetX, offsetY, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = originalPos;
    }


}
public class ImagesOverlapedMetaData
{
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
