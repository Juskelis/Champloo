using System;
using UnityEngine;

public class WeaponSpecialListener : MonoBehaviour {
    
    [SerializeField]
    private Transform buildupEffect;

    void Awake()
    {
        GetComponentInParent<LocalEventDispatcher>().AddListener<WeaponSpecialTimingEvent>(OnSpecial);
    }

    private void OnSpecial(object sender, EventArgs args)
    {
        WeaponSpecialTimingEvent e = (WeaponSpecialTimingEvent)args;
        if (e.Timing == TimingState.WARMUP)
        {
            Instantiate(buildupEffect, transform, false);
        }
    }
}
