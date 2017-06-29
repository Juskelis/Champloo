using System;
using UnityEngine;

public class DashStateListener : MonoBehaviour
{
    [SerializeField]
    private Transform dashPuff;

    [SerializeField]
    private PlayRandomSource dashSound;

    private TrailRenderer trail;

    void Awake()
    {
        LocalEventDispatcher d = GetComponentInParent<LocalEventDispatcher>();
        if (d == null)
        {
            Debug.LogError("Component can't find event system to hook into!", this);
        }
        else
        {
            d.AddListener<MovementSpecialTimingEvent>(DashTimingChanged);
        }

        trail = GetComponent<TrailRenderer>();
    }

    private void DashTimingChanged(object sender, EventArgs args)
    {
        MovementSpecialTimingEvent e = (MovementSpecialTimingEvent) args;
        if (e.Special is OnDash && e.Timing == TimingState.IN_PROGRESS)
        {
            Instantiate(dashPuff, transform.position, transform.rotation);
            dashSound.Play();
            trail.enabled = true;
            trail.Clear();
        }
        else
        {
            trail.enabled = false;
            trail.Clear();
        }
    }

    void OnDestroy()
    {
        LocalEventDispatcher d = GetComponentInParent<LocalEventDispatcher>();
        if (d != null)
        {
            d.RemoveListener<MovementSpecialTimingEvent>(DashTimingChanged);
        }
    }
}
