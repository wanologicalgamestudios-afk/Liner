using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyCrossFade : MonoBehaviour
{
    [SerializeField]
    Animator crossFade;

    Action CallbackCrossFadeIn;
    Action CallbackCrossFadeOut;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void PlayCrossFadeInAnimation(Action _callbackCrossFadeIn)
    {
        crossFade.SetTrigger("in");
        CallbackCrossFadeIn = _callbackCrossFadeIn;
    }


    public void PlayCrossFadeOutAnimation()
    {
        crossFade.SetTrigger("out");
    }

    public void OnbackCrossFadeIn() 
    {
        CallbackCrossFadeIn.Invoke();
        Debug.Log("ON BACK CROSS FADE IN FUNCTION INVOKED");
    }
}
