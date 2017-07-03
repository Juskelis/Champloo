using System;
using UnityEngine;

public class BlockEvent : EventArgs {
    public Player Blocker { get; set; }
    public Player Attacker { get; set; }
}
