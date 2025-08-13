using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TrakingAttributes
{
    public string name;
    public bool canDeletePrevious;
    public Transform transform;
}
public class UIManager : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private WarningsPanelUI warningsPanelUI;
    [SerializeField]
    private string allPanelPathInResourcesFolder = "Menus/";
    [SerializeField]
    private bool isShowingAds = false;
    [SerializeField]
    private bool startGamePlay = false;
    [SerializeField]
    private MyCrossFade myCrossFade;
    [SerializeField]
    private float transitionTime;

    public GameManager GameManager => gameManager;

    private string panelName;

    private static GameObject _thisGameObject;
    private static UIManager myInstance = null;

    private bool canDeletePrevious;

   // private Image crossFadeParent;

  //  private Action crossFadeAction;

    private GameObject currentPanel;

    private TrakingAttributes trakingAttributes;

    private List<TrakingAttributes> trakingList;
   // private GameObject currentVcam;

    // public Action crossFadeCallBack;
    /// <summary>
    /// just a Awake fuction.
    /// </summary>
    void Awake()
    {
        myInstance = null;
        _thisGameObject = this.gameObject;
    }
    /// <summary>
    /// this fuctoin will make a static instance of this class.
    /// </summary>
    /// <returns>UIManager</returns>
    public static UIManager GetInstance()
    {
        if (myInstance == null && _thisGameObject != null)
        {
            myInstance = _thisGameObject.GetComponent<UIManager>();
        }
        return myInstance;
    }
    // Start is called before the first frame update
    void Start()
    {
        //RestMenusToHome();
        SetAtStart();
    }
    /// <summary>
    /// this fuction will set some variables at the start function.
    /// </summary>
    void SetAtStart()
    {
        SpawnSplashScreen();
    }
    /// <summary>
    /// this function will active HomeUI;
    /// </summary>
    void SpawnSplashScreen()
    {
        currentPanel = null;
        trakingAttributes = null;
        canDeletePrevious = false;
        trakingList = new List<TrakingAttributes>();
        SpawnUIPanel(nameof(LoadingScreenUI), this.transform);
    }
    public void RestMenusToHome()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }
        currentPanel = null;
        trakingAttributes = null;
        canDeletePrevious = false;
        trakingList = new List<TrakingAttributes>();
        SpawnUIPanel(nameof(HomeUI), this.transform);
    }
    public void RestMenusToPerticularPanel(String _panelName)
    {
        int indexOfPanelToSpawn = trakingList.FindIndex(x => x.name == _panelName);
        if (indexOfPanelToSpawn != -1) 
        {
            string nameOfPanelToSpawn = trakingList[indexOfPanelToSpawn].name;

            for (int i = trakingList.Count - 1; i >= indexOfPanelToSpawn; i--)
            {
                Debug.Log(i);
                trakingList.RemoveAt(i);
            }
            for (int i = 0; i < this.transform.childCount; i++)
            {
                Destroy(this.transform.GetChild(i).gameObject);
            }


            canDeletePrevious = true;
            SpawnUIPanel(nameOfPanelToSpawn, this.transform);
        }

     
    }
    public void ResetOnLogout()
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Destroy(this.transform.GetChild(i).gameObject);
        }
        currentPanel = null;
        trakingAttributes = null;
        canDeletePrevious = false;
        trakingList = new List<TrakingAttributes>();
        SpawnUIPanel(nameof(HomeUI), this.transform);
    }
    void CrossFadeInAnimation(Action _callbackCrossFadeInAnimation)
    {
        myCrossFade.PlayCrossFadeInAnimation(_callbackCrossFadeInAnimation);
        //crossFade.SetTrigger("in");
        //crossFade.GetComponent<CanvasGroup>().blocksRaycasts = true;
        //crossFade.GetComponent<CanvasGroup>().interactable = true;
        //iTween.ValueTo(crossFade, iTween.Hash(
        //    "from", _from,
        //    "to", _to,
        //    "time", transitionTime,
        //    "onupdatetarget", this.gameObject,
        //    "onupdate", nameof(OnUpdateCrossFadeAnimation),
        //    "oncompletetarget", this.gameObject,
        //    "oncomplete", nameof(OnCompleteCrossFadeAnimation)
        //    ));
    }
    
    void CrossFadeOutAnimation()
    {
        myCrossFade.PlayCrossFadeOutAnimation();
    }
    //void OnUpdateCrossFadeAnimation(float _value)
    //{
    //    crossFadeParent = crossFade.GetComponent<Image>();
    //    crossFadeParent.color = new Color(crossFadeParent.color.r, crossFadeParent.color.g, crossFadeParent.color.b, _value);
    //}
    //void OnCompleteCrossFadeAnimation()
    //{
    //    crossFade.GetComponent<CanvasGroup>().blocksRaycasts = false;
    //    crossFade.GetComponent<CanvasGroup>().interactable = false;
    //    crossFadeAction?.Invoke();
    //    //crossFadeCallBack?.Invoke();
    //}
    /// <summary>
    /// this is just a update function.
    /// </summary>
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
           // SoundManager.Click();
            BackButtonIsPressed();
        }
    }
    /// <summary>
    /// will spawn a next panel. 
    /// </summary>
    /// <param name="_nameOfMenu"></param>
    /// <param name="_canDeletePrevious"></param>
    public void SpawnNextPanel(string _nameOfMenu, bool _canDeletePrevious)
    {
        canDeletePrevious = _canDeletePrevious;
        panelName = _nameOfMenu;
        if (canDeletePrevious)
        {
            CrossFadeInAnimation(SpawnNextPanelAfter);
          //crossFadeAction = SpawnNextPanelAfter;
        }
        else
        {
          SpawnNextPanelAfter();
        }
        //SpawnNextPanelAfter();
    }

    void SpawnNextPanelAfter()
    {
        if (canDeletePrevious)
        {
            StartCoroutine(DestroyGameObjects(currentPanel));
            // crossFadeAction = null;
            // CrossFadeAnimation(1.0f, 0.0f);
            CrossFadeOutAnimation();
        }

        SpawnUIPanel(panelName, this.transform);
    }

    /// <summary>
    /// this funtion will be called on the native android button or any back button that change ui to ome panel to anothe panel. 
    /// </summary>
    public void BackButtonIsPressed()
    {
        Debug.Log("Back");
        if (isShowingAds || currentPanel.name == nameof(GamePlayUI) || currentPanel.name == nameof(HowToPlayUI))
        {
        }
        else if (currentPanel.name == nameof(HomeUI))
        {
           // SpawnNextPanel(nameof(QuitPanelUI), false);
        }
        else
        {
            if (trakingList[trakingList.Count - 1].canDeletePrevious)
            {
                CrossFadeInAnimation(BackButtonIsPressedAfterAnimation);
               // CrossFadeAnimation(0.0f, 1.0f);
             //   crossFadeAction = BackButtonIsPressedAfterAnimation;
            }
            else
            {
                BackButtonIsPressedAfterAnimation();
            }
            //BackButtonIsPressedAfterAnimation();
        }
    }

    void BackButtonIsPressedAfterAnimation()
    {
        if (!trakingList[trakingList.Count - 1].canDeletePrevious)
        {
            StartCoroutine(DestroyGameObjects(currentPanel));
            trakingList.RemoveAt(trakingList.Count - 1);

            currentPanel = trakingList[trakingList.Count - 1].transform.gameObject;
        }
        else
        {
            StartCoroutine(DestroyGameObjects(currentPanel));
            trakingList.RemoveAt(trakingList.Count - 1);

            string panelNameToBeSpawn = trakingList[trakingList.Count - 1].name;

            trakingList.RemoveAt(trakingList.Count - 1);
            SpawnUIPanel(panelNameToBeSpawn, this.transform);

            //crossFadeAction = null;
            //CrossFadeAnimation(1.0f, 0.0f);
            CrossFadeOutAnimation();
        }

        //Screen.orientation = ScreenOrientation.LandscapeLeft;
    }
    /// <summary>
    /// this will spawn a panel in the ui on ask. 
    /// </summary>
    /// <param name="_nameOfMenu"></param>
    /// <param name="_parent"></param>
    void SpawnUIPanel(string _nameOfMenu, Transform _parent)
    {
        var prefabe = Resources.Load(allPanelPathInResourcesFolder+ _nameOfMenu);
        currentPanel = Instantiate(prefabe) as GameObject;
        currentPanel.transform.SetParent(_parent);
        currentPanel.transform.localScale = Vector3.one;
        currentPanel.name = _nameOfMenu;
        RectTransform rectTrans = currentPanel.GetComponent<RectTransform>();
        rectTrans.offsetMin = new Vector2(0, 0);
        rectTrans.offsetMax = new Vector2(0, 0);

        trakingAttributes = new TrakingAttributes();
        trakingAttributes.name = _nameOfMenu;
        trakingAttributes.canDeletePrevious = canDeletePrevious;
        trakingAttributes.transform = currentPanel.transform;

        trakingList.Add(trakingAttributes);

        //ScreenVcamInfo vcamInfo = vCamsList.Find(x => x.screenName ==  _nameOfMenu);
        //if (vcamInfo != null)
        //{
        //    if (currentVcam != null)
        //        currentVcam.SetActive(false);

        //    currentVcam = vcamInfo.vcamObj;
        //    currentVcam.SetActive(true);
        //}
    }
    /// <summary>
    /// this will destroy a game object using Coroutine.
    /// </summary>
    /// <param name="_gameObjectToDestroy"></param>
    /// <returns></returns>
    IEnumerator DestroyGameObjects(GameObject _gameObjectToDestroy)
    {
        yield return new WaitForSeconds(0.0f);
        Destroy(_gameObjectToDestroy);
    }

    /// <summary>
    /// will reture curret panel being showed in ui currently.
    /// </summary>
    /// <returns></returns>
    public GameObject GetCurrentPanel()
    {
        return currentPanel;
    }
    public GameObject GetSecondLastPanel()
    {
        return trakingList[^2].transform.gameObject;
    }

    //void MoveCamera(GameObject obj, Vector3 pos, bool isLocal, float time)
    //{
    //    iTween.MoveTo(obj, iTween.Hash(
    //         "position", pos,
    //         "time", time,
    //         "easeType", iTween.EaseType.linear,
    //         "islocal", isLocal,
    //         "oncomplete", "OnCompleteMoveObject",
    //         "oncompletetarget", this.gameObject
    //     )
    //     );
    //}

    public void ActiveNoWifi()
    {
        warningsPanelUI.SetActiveNoWifi();
    }
    public void ActiveLoadingPanel()
    {
        warningsPanelUI.SetActiveLoadingPanel();
    }
    public void InactiveLoadingPanel()
    {
        warningsPanelUI.InactiveLoadingPanel();
    }
    public void ActiveMessagePanel(string _message, float _duration = 3.0f)
    {
        warningsPanelUI.ActiveWarningMessagePanel(_message, _duration);
    }
    public void InactiveMessagePanel()
    {
        warningsPanelUI.InactiveWarningMessage();
    }
    //public bool IsInternetAvailable()
    //{
    //    return gameManager.IsInternetAvailable();
    //}
}

