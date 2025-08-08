using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UILineFillManager : MonoBehaviour
{
    private GraphicRaycaster raycaster;
    private PointerEventData pointerData;
    private EventSystem eventSystem;

    void Awake()
    {
        raycaster = FindObjectOfType<Canvas>().GetComponent<GraphicRaycaster>();
        eventSystem = EventSystem.current;
    }

    void Update()
    {
#if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButton(0))
        {
            ProcessDrag(Input.mousePosition);
        }
#elif UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
        {
            ProcessDrag(Input.GetTouch(0).position);
        }
#endif
    }

    void ProcessDrag(Vector2 screenPos)
    {
        pointerData = new PointerEventData(eventSystem);
        pointerData.position = screenPos;

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        foreach (RaycastResult result in results)
        {
            UILineFiller filler = result.gameObject.GetComponent<UILineFiller>();
            if (filler != null)
            {
                filler.Fill();
            }
        }
    }
}
