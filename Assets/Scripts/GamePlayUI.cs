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
    float elapsedTime;

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
    }

    private void SetLevelName() 
    {
        levelName.text = "Level " + currentLevel;
    }
    private void ShowHint() 
    {
        autoImageFiller.PlayHint();
    }
    private void OnAdNotReayYet() 
    {
        UIManager.GetInstance().ActiveMessagePanel("Ad is not ready yet.");
    }
    public void LoadNextLevel() 
    {
        UIManager.GetInstance().GameManager.CurerntLevel = UIManager.GetInstance().GameManager.CurerntLevel + 1;
        Destroy(currentLevelObj);
        LoadLevel();
    }

    public void SetLevelCompletionBar(float _value) 
    {
        levelCompletionBar.value = _value;
    }

    public void HowToPlayButtonCall()
    {
        UIManager.GetInstance().SpawnNextPanel(nameof(HowToPlayUI),true);
    }

    public void SettingsButtonCall() 
    {
        UIManager.GetInstance().SpawnNextPanel(nameof(SettingsUI), false);
    }

    public void HintButtonCall() 
    {
        AdManager.Instance.ShowRewardedAd(ShowHint, OnAdNotReayYet);
    }

    public void SkipPuzzleButtonCall() 
    {
        AdManager.Instance.ShowRewardedAd(LoadNextLevel, OnAdNotReayYet);
    }

    private void Update()
    {
            elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60F);
            int seconds = Mathf.FloorToInt(elapsedTime - minutes * 60);
            timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

}
