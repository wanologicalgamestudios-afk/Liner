using UnityEngine;
using TMPro;

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
    }
    private void SetUiOnStart()
    {
        timertext.text = UIManager.GetInstance().GameManager.LastLevelCompletionTime;
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
    }

    public void LoadNextLevel()
    {
        UIManager.GetInstance().BackButtonIsPressed();
        if (UIManager.GetInstance().GetCurrentPanel().GetComponent<GamePlayUI>())
            UIManager.GetInstance().GetCurrentPanel().GetComponent<GamePlayUI>().LoadNextLevel();
    }


    public void ReplayLevel()
    {
        UIManager.GetInstance().BackButtonIsPressed();
        if (UIManager.GetInstance().GetCurrentPanel().GetComponent<GamePlayUI>())
            UIManager.GetInstance().GetCurrentPanel().GetComponent<GamePlayUI>().ReplayLevel();
    }

}
