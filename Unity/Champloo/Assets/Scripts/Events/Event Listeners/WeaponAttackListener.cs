using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttackListener : MonoBehaviour
{
    [SerializeField]
    private PlayRandomSource attackSound;

    void Awake()
    {
        GetComponentInParent<LocalEventDispatcher>().AddListener<WeaponAttackTimingEvent>(OnAttack);
    }

    private void OnAttack(object sender, EventArgs args)
    {
        WeaponAttackTimingEvent e = (WeaponAttackTimingEvent) args;
        if (e.Timing == TimingState.IN_PROGRESS)
        {
            attackSound.Play();
        }
    }
}
