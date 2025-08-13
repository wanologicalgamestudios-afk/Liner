using UnityEngine;

public class HowToPlayUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void GotitButtonCall() 
    {
        UIManager.GetInstance().SpawnNextPanel(nameof(GamePlayUI), true);
    }
}
