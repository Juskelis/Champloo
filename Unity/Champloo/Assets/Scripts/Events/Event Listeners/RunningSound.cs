using System;
using UnityEngine;

public class RunningSound : MonoBehaviour {

    [SerializeField]
    private PlayRandomSource runningSound;

    private bool inState = false;

    private Player player;

    private void Awake()
    {
        GetComponentInParent<LocalEventDispatcher>().AddListener<MovementStateChangedEvent>(OnTiming);
        player = GetComponentInParent<Player>();
    }

    private void OnDestroy()
    {
        LocalEventDispatcher dispatcher = GetComponentInParent<LocalEventDispatcher>();
        if (dispatcher != null)
        {
            dispatcher.RemoveListener<MovementStateChangedEvent>(OnTiming);
        }
    }

    private void OnTiming(object sender, EventArgs args)
    {
        MovementStateChangedEvent e = (MovementStateChangedEvent) args;
        inState = e.Next is OnGround;
    }

    private void Update()
    {
        if (inState && Mathf.Abs(player.Velocity.x) > 0)
        {
            runningSound.PlayLooped();
        }
        else
        {
            runningSound.Stop();
        }
    }
}
