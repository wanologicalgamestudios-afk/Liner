using UnityEngine;
using TMPro;
using System;

public class LevelCompleteUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timertext;
    [SerializeField]
    private string allLevelPathInResourcesFolder = "Levels/";
    [SerializeField]
    private Transform currentLevelObjParent;
    private GameObject currentLevelObj;

    private int currentLevel;

    void Start()
    {
        SetUiOnStart();
        PlayLevelSuccessfullSound();
    }

    private void PlayLevelSuccessfullSound()
    {
        UIManager.GetInstance().SoundManager.PlayConfetti();
    }
    private void SetUiOnStart()
    {
        string lastTimeStr = UIManager.GetInstance().GameManager.LastLevelCompletionTime;

        if (float.TryParse(lastTimeStr, out float seconds))
        {
            TimeSpan t = TimeSpan.FromSeconds(seconds);
            timertext.text = string.Format("{0:D2}:{1:D2}", t.Minutes, t.Seconds);
        }
        else
        {
            timertext.text = lastTimeStr; // fallback
        }
        SetLevelAvatar();
    }
  
    private void SetLevelAvatar()
    {
        currentLevel = UIManager.GetInstance().GameManager.CurerntLevel;
        if (Resources.Load(allLevelPathInResourcesFolder + "Level" + currentLevel) == null)
        {
            UIManager.GetInstance().GameManager.CurerntLevel = 1;
            currentLevel = UIManager.GetInstance().GameManager.CurerntLevel;
        }
        var prefabe = Resources.Load(allLevelPathInResourcesFolder + "Level" + currentLevel);
        currentLevelObj = Instantiate(prefabe) as GameObject;
        currentLevelObj.transform.SetParent(currentLevelObjParent);
        currentLevelObj.transform.localScale = Vector3.one;
        RectTransform rectTrans = currentLevelObj.GetComponent<RectTransform>();
        rectTrans.offsetMin = new Vector2(0, 0);
        rectTrans.offsetMax = new Vector2(0, 0);

        currentLevelObj.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        currentLevelObj.transform.rotation = Quaternion.Euler(0, 0, 0);

        Invoke(nameof(FillForCompleteLevelScreen), 0.1f);
    }

    private void FillForCompleteLevelScreen() 
    {
        currentLevelObj.GetComponent<AutoImageFiller>().FillForCompleteLevelScreen();
    }

    public void LoadNextLevel()
    {
        UIManager.GetInstance().SoundManager.StopConfetti();
        UIManager.GetInstance().BackButtonIsPressed();
        if (UIManager.GetInstance().GetCurrentPanel().GetComponent<GamePlayUI>())
            UIManager.GetInstance().GetCurrentPanel().GetComponent<GamePlayUI>().LoadNextLevel();
    }


    public void ReplayLevel()
    {
        UIManager.GetInstance().SoundManager.StopConfetti();
        UIManager.GetInstance().BackButtonIsPressed();
        if (UIManager.GetInstance().GetCurrentPanel().GetComponent<GamePlayUI>())
            UIManager.GetInstance().GetCurrentPanel().GetComponent<GamePlayUI>().ReplayLevel();
    }

}
