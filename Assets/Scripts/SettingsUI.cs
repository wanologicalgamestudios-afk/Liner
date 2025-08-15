using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [SerializeField]
    private Toggle vibrationToggle;
    [SerializeField]
    private Toggle musicToggle;
    [SerializeField]
    private Toggle soundToggle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetToggleUIOnStart();
    }

    private void SetToggleUIOnStart() 
    {
        if (UIManager.GetInstance().GameManager.IsVibrationOff == 1)
        {
            vibrationToggle.isOn = true;
        }
        else 
        {
            vibrationToggle.isOn = false;
        }
    }

    public void CrossButtonCall() 
    {
        UIManager.GetInstance().BackButtonIsPressed();
    }

    public void MusicButtonCall(Toggle _musicToggle) 
    {
        if (_musicToggle.isOn)
        {
        }
        else 
        {
        }
    }

    public void SoundButtonCall(Toggle _soundToggle)
    {
        if (_soundToggle.isOn)
        {
        }
        else
        {
        }
    }

    public void VibrationButtonCall(Toggle _viberationToggle)
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
