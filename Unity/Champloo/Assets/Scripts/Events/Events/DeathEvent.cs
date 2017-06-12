using System;
using UnityEngine;

public class DeathEvent : EventArgs
{
    public Player deadPlayer { get; set; }

    //if we want to send the corpse in a direction, we can use this
    public Vector3 corpsePushDirection { get; set; }
}
