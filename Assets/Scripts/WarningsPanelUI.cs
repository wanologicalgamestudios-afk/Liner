using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.TimeZoneInfo;

public class WarningsPanelUI : MonoBehaviour
{
    [SerializeField]
    private GameObject noWifiImageObj;
    [SerializeField]
    private GameObject loadingPanelObj;
    [SerializeField]
    private Animator warningMessageAnimator;
    [SerializeField]
    private Animator messageAnimator;
    [SerializeField]
    private float warningMessageAnimationTime;
    [SerializeField]
    private TextMeshProUGUI warningMessageText;
    [SerializeField]
    private TextMeshProUGUI messageHeadingText;
    [SerializeField]
    private TextMeshProUGUI messageText;

    private void Start()
    {
        noWifiImageObj.SetActive(false);
        loadingPanelObj.SetActive(false);
    }
    private void DisactiveNoWifi()
    {
        noWifiImageObj.SetActive(false);
    }
    public void ActiveWarningMessagePanel(string _message, float duration)
    {
        CancelInvoke(nameof(InactiveWarningMessage));
        warningMessageAnimator.SetTrigger("show");
        warningMessageAnimator.SetBool("canHide", false);
        warningMessageText.text = _message;
        Invoke(nameof(InactiveWarningMessage), duration + warningMessageAnimationTime);
    }
    public void InactiveWarningMessage()
    {
        warningMessageAnimator.SetBool("canShow", false);
        warningMessageAnimator.SetBool("canHide", true);
    }

    public void ActiveMessagePanel(string _heading,string _message, float duration)
    {
        CancelInvoke(nameof(DisactiveMessage));
        messageAnimator.SetTrigger("show");
        messageAnimator.SetBool("canHide", false);
        messageHeadingText.text = _heading;
        messageText.text = _message;
        Invoke(nameof(DisactiveMessage), duration + warningMessageAnimationTime);
    }
    public void DisactiveMessage()
    {
        messageAnimator.SetBool("canShow", false);
        messageAnimator.SetBool("canHide", true);
    }
    public void SetActiveNoWifi()
    {
        noWifiImageObj.SetActive(true);
        Invoke(nameof(DisactiveNoWifi), 0.3f);
    }

    public void SetActiveLoadingPanel()
    {
        loadingPanelObj.SetActive(true);
    }
    public void InactiveLoadingPanel()
    {
        loadingPanelObj.SetActive(false);
    }

}
