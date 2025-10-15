using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GamePlayUI : MonoBehaviour
{
    [SerializeField]
    private Transform currentLevelObjParent;
    [SerializeField]
    private string allLevelPathInResourcesFolder = "Levels/";
    [SerializeField]
    private Slider levelCompletionBar;
    [SerializeField]
    private TextMeshProUGUI levelName;
    [SerializeField] TextMeshProUGUI timerText;
    private float elapsedTime;
    private bool canStartTimer = false;

    private int currentLevel;
    private GameObject currentLevelObj;
    private AutoImageFiller autoImageFiller;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        //TimerManager.Instance?.StartTimer();
        LoadLevel();
    }



    private void LoadLevel() 
    {
        currentLevel = UIManager.GetInstance().GameManager.CurerntLevel;

        if (Resources.Load(allLevelPathInResourcesFolder + "Level" + currentLevel) ==  null) 
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

        autoImageFiller = new AutoImageFiller();
        autoImageFiller = currentLevelObj.GetComponent<AutoImageFiller>();

        SetLevelCompletionBar(0);
        SetLevelName();


        if (UIManager.GetInstance().GameManager.CurerntLevel % 10 == 0)
        {
            UIManager.GetInstance().SpawnNextPanel(nameof(BossLevelNotificationUI), false);
            Invoke(nameof(StartTime), 1.5f);
        }
        else 
        {
            StartTime();
        }
    }

    private void SetLevelName() 
    {
        levelName.text = "LEVEL " + currentLevel;
    }

    private void ShowHint() 
    {
        autoImageFiller.PlayHint();
    }

    private void OnAdNotReadyYet() 
    {
        UIManager.GetInstance().ActiveMessagePanel("Ad is not ready yet.");
    }

    public void LoadNextLevel() 
    {
        UIManager.GetInstance().GameManager.CurerntLevel = UIManager.GetInstance().GameManager.CurerntLevel + 1;
        Destroy(currentLevelObj);
        LoadLevel();
    }

    public void ReplayLevel() 
    {
        Destroy(currentLevelObj);
        LoadLevel();
    }

    public void SetLevelCompletionBar(float _value) 
    {
        levelCompletionBar.value = _value;
    }

    public void HowToPlayButtonCall()
    {
        UIManager.GetInstance().SoundManager.PlayButton();
        UIManager.GetInstance().SpawnNextPanel(nameof(HowToPlayUI),true);
    }

    public void SettingsButtonCall() 
    {
        UIManager.GetInstance().SoundManager.PlayButton();
        UIManager.GetInstance().SpawnNextPanel(nameof(SettingsUI), false);
    }

    public void HintButtonCall() 
    {
        UIManager.GetInstance().SoundManager.PlayButton();
        AdManager.Instance.ShowRewardedAd(ShowHint, OnAdNotReadyYet);
    }

    public void SkipPuzzleButtonCall() 
    {
        UIManager.GetInstance().SoundManager.PlayButton();
        AdManager.Instance.ShowRewardedAd(LoadNextLevel, OnAdNotReadyYet);
    }

    private void StartTime()
        {
        elapsedTime = 0f;
        timerText.text = "0:00";
        canStartTimer = true;
    }

    private void Viberate()
    {
        if (UIManager.GetInstance().GameManager.IsVibrationOff == 0)
        {
            // Short vibration cross-platform
            Handheld.Vibrate();
        }
    }

    private void PlayLevelFailSound()
    {
        UIManager.GetInstance().SoundManager.PlayLevelFail();
    }

    private void PlayLevelSuccessfullSound()
    {
        UIManager.GetInstance().SoundManager.PlayLevelDone();
    }

    public void OnLevelFail() 
    {
        PlayLevelFailSound();
        Viberate();
    }

    public void OnPuzzleSuccessfull()
    {
        PlayLevelSuccessfullSound();
        UIManager.GetInstance().GameManager.LastLevelCompletionTime = elapsedTime.ToString();
        canStartTimer = false;
        UIManager.GetInstance().SpawnNextPanel(nameof(LevelDoneUI), false);
    }

    private void Update()
    {
        if (canStartTimer)
        {
            elapsedTime += Time.deltaTime;
            int minutes = Mathf.FloorToInt(elapsedTime / 60F);
            int seconds = Mathf.FloorToInt(elapsedTime - minutes * 60);
            timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        }
           
    }

}
