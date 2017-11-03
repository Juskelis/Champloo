using System;
using UnityEngine;

public class DashStateListener : MonoBehaviour
{
    [SerializeField]
    private Transform dashPuff;

    [SerializeField]
    private PlayRandomSource dashSound;

    [SerializeField]
    private TrailRenderer trail;

    private Player p;

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
            d.AddListener<MovementStateChangedEvent>(MovementStateChanged);
        }
        p = GetComponentInParent<Player>();
    }

    private void MovementStateChanged(object sender, EventArgs args)
    {
        MovementStateChangedEvent e = (MovementStateChangedEvent) args;
        if (e.Next.GetType() != typeof(OnMovementSpecial))
        {
            trail.enabled = false;
            trail.Clear();
        }
    }

    void Start()
    {
        trail.enabled = false;
        trail.Clear();

        GradientColorKey[] colorKeys = trail.colorGradient.colorKeys;
        for (var i = 0; i < colorKeys.Length; i++)
        {
            colorKeys[i].color = p.PlayerColor;
        }
        Gradient g = trail.colorGradient;
        g.SetKeys(colorKeys, trail.colorGradient.alphaKeys);
        trail.colorGradient = g;
    }

    private void DashTimingChanged(object sender, EventArgs args)
    {
        MovementSpecialTimingEvent e = (MovementSpecialTimingEvent) args;
        if (e.Special is OnDash && e.Timing == TimingState.IN_PROGRESS)
        {
            Instantiate(dashPuff, p.CenterOfSprite, transform.rotation);
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
