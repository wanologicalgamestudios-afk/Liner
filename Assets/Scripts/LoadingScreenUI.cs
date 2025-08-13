using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingScreenUI : MonoBehaviour
{
    [Header("Slider Settings")]
    public Slider slider;       // Assign in Inspector
    public float fillDuration = 5f; // Time in seconds to go from 0 to 1

    [Header("Loading Text Settings")]
    public TextMeshProUGUI loadingText;    // Assign a UI Text in Inspector
    public float dotCycleDuration = 3f; // Full cycle duration for 1 → 3 dots

    private bool isRunning = false;

    void Start()
    {
        if (slider != null)
        {
            slider.value = 0f;
            isRunning = true;
            StartCoroutine(FillSlider());
        }

        if (loadingText != null)
        {
            StartCoroutine(AnimateLoadingText());
        }
    }

    IEnumerator FillSlider()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fillDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / fillDuration);
            slider.value = Mathf.Lerp(0f, 1f, progress);
            yield return null;
        }

        OnComplete();
    }

    IEnumerator AnimateLoadingText()
    {
        string baseText = "Loading";
        int dotCount = 1;

        while (true) // Infinite loop
        {
            loadingText.text = baseText + new string('.', dotCount);

            // Number of steps in 3 seconds = 3 (1 dot, 2 dots, 3 dots)
            yield return new WaitForSeconds(dotCycleDuration / 3f);

            dotCount++;
            if (dotCount > 3) dotCount = 1;
        }
    }

    void OnComplete()
    {
        if (UIManager.GetInstance().GameManager.IsFirstTimeHowToPlayShown == 0) 
        {
            UIManager.GetInstance().GameManager.IsFirstTimeHowToPlayShown = 1;
            UIManager.GetInstance().SpawnNextPanel(nameof(HowToPlayUI), true);
        }
        else 
        {
            UIManager.GetInstance().SpawnNextPanel(nameof(GamePlayUI), true);
        }
        
        // Debug.Log("Slider reached the end!");
        // Add your completion logic here
    }
}
