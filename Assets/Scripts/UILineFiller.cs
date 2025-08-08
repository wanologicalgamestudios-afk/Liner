using UnityEngine;
using UnityEngine.UI;

public class UILineFiller : MonoBehaviour
{
    public Color fillColor = Color.green;

    private bool isFilled = false;
    private Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Fill()
    {
        if (isFilled) return;

        image.color = fillColor;
        isFilled = true;
    }

    public void ResetLine()
    {
        isFilled = false;
        image.color = Color.white;
    }

    public bool IsFilled() => isFilled;
}
