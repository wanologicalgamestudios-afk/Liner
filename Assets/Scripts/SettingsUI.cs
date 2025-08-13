using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void CrossButtonCall() 
    {
        UIManager.GetInstance().BackButtonIsPressed();
    }

    public void MusicButtonCall(Toggle _musicToggle) 
    {
    }

    public void SoundButtonCall(Toggle _soundToggle)
    {
    }

    public void ViberationButtonCall(Toggle _viberationToggle)
    {
    }
}
