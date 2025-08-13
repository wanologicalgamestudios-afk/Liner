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
        if (_musicToggle.isOn) 
        {
            //UIManager
        }
    }

    public void SoundButtonCall(Toggle _soundToggle)
    {
    }

    public void ViberationButtonCall(Toggle _viberationToggle)
    {
        if (_viberationToggle.isOn)
        {
            UIManager.GetInstance().GameManager.IsVibrationOff = 1;
        }
        else 
        {
            UIManager.GetInstance().GameManager.IsVibrationOff = 0;
        }
    }
}
