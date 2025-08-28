using UnityEngine;

public class BossLevelNotificationUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Invoke(nameof(DisactiveNotification), 1.30f);
    }

    private void DisactiveNotification() 
    {
        UIManager.GetInstance().BackButtonIsPressed();
    }
}
