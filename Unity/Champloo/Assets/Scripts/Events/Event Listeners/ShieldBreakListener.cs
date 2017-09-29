using System;
using UnityEngine;

public class ShieldBreakListener : MonoBehaviour {

    [SerializeField]
    private PlayRandomSource breakSound;

    [SerializeField]
    private Transform breakEffect;
    
    private void Start()
    {
        EventDispatcher.Instance.AddListener<ShieldBreakEvent>(OnBreak);
    }

    private void OnDestroy()
    {
        if (EventDispatcher.Instance != null)
        {
            EventDispatcher.Instance.RemoveListener<ShieldBreakEvent>(OnBreak);
        }
    }

    private void OnBreak(object sender, EventArgs args)
    {
        breakSound.Play();

        Instantiate(breakEffect, ((ShieldBreakEvent)args).OurShield.transform.position, transform.rotation);
    }
}
