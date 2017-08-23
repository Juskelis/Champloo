using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandListener : MonoBehaviour
{
    [SerializeField]
    private float velocityMagnitude;

    [SerializeField]
    [Range(0,1)]
    private float neededDirection;

    [SerializeField]
    private CamerShakeSettings shakeSettings;
    
    void Awake()
    {
        GetComponentInParent<LocalEventDispatcher>().AddListener<MovementStateChangedEvent>(OnChange);
    }

    private void OnChange(object sender, EventArgs args)
    {
        MovementStateChangedEvent ev = (MovementStateChangedEvent) args;

        if ((ev.Previous is InAir || ev.Previous is OnDash) && ev.Next is OnGround)
        {

            shakeSettings.Shake();
        }
    }
}
