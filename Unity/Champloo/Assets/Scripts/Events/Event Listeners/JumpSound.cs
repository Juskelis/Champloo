using System;
using UnityEngine;

public class JumpSound : MonoBehaviour {

    [SerializeField]
    private PlayRandomSource jumpSound;


    void Awake()
    {
        GetComponentInParent<LocalEventDispatcher>().AddListener<JumpEvent>(OnJump);
    }

    void OnDestroy()
    {
        LocalEventDispatcher dispatcher = GetComponentInParent<LocalEventDispatcher>();
        if (dispatcher != null)
        {
            dispatcher.RemoveListener<JumpEvent>(OnJump);
        }
    }

    private void OnJump(object sender, EventArgs args)
    {
        jumpSound.Play();
    }
}
