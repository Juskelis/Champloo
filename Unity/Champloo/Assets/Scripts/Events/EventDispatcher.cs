using System;
using System.Collections.Generic;
using UnityEngine;

public class EventDispatcher : MonoBehaviour
{
    private static EventDispatcher _instance;
    public static EventDispatcher Instance {
        get
        {
            return _instance;
        }
    }

    public delegate void OnEventReceived(object sender, EventArgs args);

    protected Dictionary<Type, OnEventReceived> Listeners;

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this);
        }
        else
        {
            _instance = this;
            Listeners = new Dictionary<Type, OnEventReceived>();
        }
    }

    public void FireEvent<T>(object sender, T args) where T : EventArgs
    {
        Type type = typeof(T);
        if (Listeners.ContainsKey(type))
        {
            Listeners[type].Invoke(sender, args);
        }
    }

    public void AddListener<T>(OnEventReceived toInvoke) where T : EventArgs
    {
        Type type = typeof(T);
        if (!Listeners.ContainsKey(type))
        {
            if (toInvoke != null) Listeners.Add(type, toInvoke);
        }
        else
        {
            Listeners[type] += toInvoke;
        }
    }

    public void RemoveListener<T>(OnEventReceived toRemove) where T : EventArgs
    {
        Type type = typeof(T);
        if (Listeners.ContainsKey(type))
        {
            Listeners[type] -= toRemove;
        }
    }
}
