using System;
using UnityEngine;

public class ParryListener : MonoBehaviour
{

    [SerializeField] private Animator popup;

    void Start()
    {
        EventDispatcher.Instance.AddListener<ParryEvent>(OnParry);
    }
    
    private void OnParry(object sender, EventArgs args)
    {
        //ParryEvent convertedEvent = (ParryEvent) args;
        popup.SetTrigger("Open");
    }
}
