using System;
using UnityEngine;

public class ShieldStatusListener : MonoBehaviour {

    [SerializeField]
    private PlayRandomSource breakSound;

    [SerializeField]
    private Transform breakEffect;

    [SerializeField]
    private PlayRandomSource rechargeSound;

    [SerializeField]
    private Transform rechargeEffect;

    private void Start()
    {
        EventDispatcher.Instance.AddListener<ShieldBreakEvent>(OnBreak);
        EventDispatcher.Instance.AddListener<ShieldRechargeEvent>(OnRecharge);
    }

    private void OnDestroy()
    {
        if (EventDispatcher.Instance != null)
        {
            EventDispatcher.Instance.RemoveListener<ShieldBreakEvent>(OnBreak);
            EventDispatcher.Instance.RemoveListener<ShieldRechargeEvent>(OnRecharge);
        }
    }

    private void OnBreak(object sender, EventArgs args)
    {
        breakSound.Play();

        Instantiate(breakEffect, ((ShieldBreakEvent)args).OurShield.transform.position, transform.rotation);
    }

    private void OnRecharge(object sender, EventArgs args)
    {
        rechargeSound.Play();

        Instantiate(rechargeEffect, ((ShieldRechargeEvent)args).OurShield.transform.position, transform.rotation);
    }
}
