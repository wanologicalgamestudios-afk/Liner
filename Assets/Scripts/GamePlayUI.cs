using UnityEngine;

public class GamePlayUI : MonoBehaviour
{
    [SerializeField]
    private Transform currentLevelObjParent;
    [SerializeField]
    private string allLevelPathInResourcesFolder = "Levels/";

    private int currentLevel;
    private GameObject currentLevelObj;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LoadLevel();
    }

    private void LoadLevel() 
    {
        currentLevel = UIManager.GetInstance().GameManager.CurerntLevel;

        var prefabe = Resources.Load(allLevelPathInResourcesFolder+"Level"+ currentLevel);
        currentLevelObj = Instantiate(prefabe) as GameObject;
        currentLevelObj.transform.SetParent(currentLevelObjParent);
        currentLevelObj.transform.localScale = Vector3.one;
        RectTransform rectTrans = currentLevelObj.GetComponent<RectTransform>();
        rectTrans.offsetMin = new Vector2(0, 0);
        rectTrans.offsetMax = new Vector2(0, 0);
    }


}
