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
    [SerializeField]
    List<Image> allImagesOverlaped = new List<Image>();
    RectTransform myRectTr;
    Rect myRect ;
    Rect otherrect;

    void Start()
    {
        myRectTr = GetComponent<RectTransform>();
        myRect = myRectTr.rect;
   
        DisplayWorldCorners();
    }

    private void Update()
    {
        FindOverLapingImage();
    }
    private void FindOverLapingImage() 
    {
        allImagesOverlaped = new List<Image>();

        for( int i = 0; i < allImages.Count; i++) 
        {
            otherrect = allImages[i].GetComponent<RectTransform>().rect;

            if (myRect.Overlaps(otherrect,true)) 
            {
                allImagesOverlaped.Add(allImages[i]);
            }
        }
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