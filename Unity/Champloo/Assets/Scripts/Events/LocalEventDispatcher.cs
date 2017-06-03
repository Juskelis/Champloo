using System;
using System.Collections.Generic;

public class LocalEventDispatcher : EventDispatcher {
    protected override void Awake()
    {
        Listeners = new Dictionary<Type, OnEventReceived>();
    }
}
