using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class AutoImageFiller : MonoBehaviour
{
    [Header("List of Images to Fill (in order)")]
    [SerializeField]
    private List<Image> fillImages = new List<Image>();

    [Header("Total Fill Duration (seconds)")]
    [SerializeField]
    private float totalDuration = 3f;

    [SerializeField]
    private Image layer;

    private void Start()
    {
        layer.enabled = false;
    }


    public void PlayHint() 
    {
        layer.enabled = true;
        ResetAllFills();
        StartCoroutine(FillImagesSequentially());
        Invoke(nameof(ResetAllFills), totalDuration + 1);
    }
    private void ResetAllFills()
    {
        foreach (var img in fillImages)
        {
            if (img != null)
                img.fillAmount = 0f;
        }
        layer.enabled = false;
    }

    private IEnumerator FillImagesSequentially()
    {
        if (fillImages.Count == 0 || totalDuration <= 0f) yield break;

        float perImageDuration = totalDuration / fillImages.Count;

        foreach (var img in fillImages)
        {
            float elapsed = 0f;
            while (elapsed < perImageDuration)
            {
                elapsed += Time.deltaTime;
                if (img != null)
                    img.fillAmount = Mathf.Clamp01(elapsed / perImageDuration);
                yield return null;
            }

            if (img != null)
                img.fillAmount = 1f; // Ensure it's full at the end
        }


    }
}
