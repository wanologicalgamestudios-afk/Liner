using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// GetWorldCorners():
//  Access the RectTransform and read the vertices
//  that define the location and size of the
//  object.

public class ExampleClass : MonoBehaviour
{
    [SerializeField]
    private Vector3 leftEdge;
    [SerializeField]
    private Vector3 rightEdge;
    [SerializeField]
    List<Image> allImages = new List<Image>();
   // [SerializeField]
   // List<Image> allImagesOverlaped = new List<Image>();
    [SerializeField] List<Image> allImagesOverlapedLeft = new List<Image>();
    [SerializeField] List<Image> allImagesOverlapedRight = new List<Image>();
    RectTransform myRectTr;
    Rect myRect ;
    Rect otherRect;

    void Start()
    {
        myRectTr = GetComponent<RectTransform>();
        myRect = GetWorldRect(myRectTr);
   
        DisplayWorldCorners();
        FindOverLapingImage();
    }

    private void Update()
    {
     
    }
    private void FindOverLapingImage()
    {
        allImagesOverlapedLeft.Clear();
        allImagesOverlapedRight.Clear();

        float leftEdge = myRect.xMin;
        float rightEdge = myRect.xMax;
        Debug.Log("QPPPPPPPPP" + leftEdge);
        Debug.Log("QPPPPPPPPP" + rightEdge);

        for (int i = 0; i < allImages.Count; i++)
        {
            otherRect = GetWorldRect(allImages[i].GetComponent<RectTransform>());

            Debug.Log("p otherRect" + otherRect);

            if (myRect.Overlaps(otherRect, true))
            {
                Debug.Log(myRectTr.name + "Overlap = " + allImages[i].name);
                // Find the horizontal center of the other rect
                float otherCenterX = otherRect.center.x;

                // Compare distances to left and right edges
                float distToLeft = Mathf.Abs(otherCenterX - leftEdge);
                float distToRight = Mathf.Abs(otherCenterX - rightEdge);

                if (distToLeft < distToRight)
                {
                    allImagesOverlapedLeft.Add(allImages[i]);
                }
                else
                {
                    allImagesOverlapedRight.Add(allImages[i]);
                }
            }
        }
    }

    Rect GetWorldRect(RectTransform rt)
    {
        Vector3[] corners = new Vector3[4];
        rt.GetWorldCorners(corners);

        float width = corners[2].x - corners[0].x;
        float height = corners[2].y - corners[0].y;

        return new Rect(corners[0].x, corners[0].y, width, height);
    }

    void DisplayWorldCorners()
    {
        Vector3[] v = new Vector3[4];
        myRectTr.GetWorldCorners(v);

        Debug.Log("World Corners");

        leftEdge = (v[0] + v[1]) / 2;
        rightEdge = (v[2] + v[3]) / 2;

        for (var i = 0; i < 4; i++)
        {
            Debug.Log("World Corner " + i + " : " + v[i]);
        }
    }


}