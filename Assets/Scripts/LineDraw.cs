using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineDraw : MonoBehaviour
{
    public Camera mainCamera;
    public Material lineMaterial;
    public float startWidth = 0.1f;
    public float endWidth = 0.1f;

    private LineRenderer currentLine;
    private List<Vector3> points = new List<Vector3>();
    public  List<LineRenderer> allLines =  new List<LineRenderer>();

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = GetWorldPosition(Input.mousePosition);
            StartNewLine(pos);
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 pos = GetWorldPosition(Input.mousePosition);
            AddPoint(pos);
        }
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 pos = GetWorldPosition(touch.position);

            if (touch.phase == TouchPhase.Began)
            {
                StartNewLine(pos);
            }
            else if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                AddPoint(pos);
            }
        }
#endif
    }

    Vector3 GetWorldPosition(Vector3 screenPosition)
    {
        return mainCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, 10f));
    }

    void StartNewLine(Vector3 startPos)
    {
        GameObject newLine = new GameObject("Line_" + Time.time);
        currentLine = newLine.AddComponent<LineRenderer>();

        currentLine.material = lineMaterial != null ? lineMaterial : new Material(Shader.Find("Sprites/Default"));
        currentLine.startColor = Color.red;
        currentLine.endColor = Color.red;
        currentLine.startWidth = startWidth;
        currentLine.endWidth = endWidth;
        currentLine.positionCount = 0;
        currentLine.numCapVertices = 10;
        currentLine.useWorldSpace = true;

        points.Clear();
        AddPoint(startPos);
        allLines.Add(currentLine);
    }

    void AddPoint(Vector3 point)
    {
        if (points.Count == 0 || Vector3.Distance(points[points.Count - 1], point) > 0.05f)
        {
            points.Add(point);
            currentLine.positionCount = points.Count;
            currentLine.SetPositions(points.ToArray());
        }
    }

    // Clear all drawn lines
    public void ClearAllLines()
    {
        foreach (var line in allLines)
        {
            Destroy(line.gameObject);
        }

        currentLine = null;
        points.Clear();
        allLines.Clear();
    }
}
