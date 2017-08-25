using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Invoker : MonoBehaviour
{
    private static Invoker instance;
    public static Invoker Instance
    {
        get { return instance ?? (instance = FindObjectOfType<Invoker>()); }
        private set { instance = value; }
    }

    public delegate void Callback();
    
    private Dictionary<Callback, float> callStack;

    private bool needsToProcess = false;

    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
        callStack = new Dictionary<Callback, float>();
    }

    private IEnumerator UpdateRoutine()
    {
        while (needsToProcess)
        {
            List<Callback> keys = new List<Callback>(callStack.Keys);
            foreach (Callback key in keys)
            {
                float newValue = callStack[key] - Time.unscaledDeltaTime;
                if (newValue <= 0)
                {
                    key.Invoke();
                    callStack.Remove(key);
                }
                else
                {
                    callStack[key] = newValue;
                }
            }
            needsToProcess = callStack.Count > 0;
            yield return null;
        }
    }

    public void InvokeUnscaled(Callback c, float t)
    {
        if (!callStack.ContainsKey(c))
        {
            callStack.Add(c, t);
            if (!needsToProcess)
            {
                needsToProcess = true;
                StartCoroutine(UpdateRoutine());
            }
        }
        else
        {
            Debug.LogError("Unable to Invoke Callback " + c);
        }
    }
}
