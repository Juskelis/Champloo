using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GamepadInput;
using Rewired;

public class InputController : MonoBehaviour
{
    private Rewired.Player inputPlayer;

    [SerializeField]
    private float angleDeadzone;

    public float AngleDeadzone
    {
        get { return angleDeadzone;}
    }

    private float xAngleDeadZone;
    private float yAngleDeadZone;

    //private Dictionary<String, float> timeSinceLastPress;
    private List<String> actions;
    private List<float> times;

    private void Start()
    {
        xAngleDeadZone = Mathf.Sin(angleDeadzone * Mathf.Deg2Rad);
        yAngleDeadZone = Mathf.Cos(angleDeadzone * Mathf.Deg2Rad);

        inputPlayer = GetComponentInParent<Player>().InputPlayer;
        
        actions = new List<string>();
        times = new List<float>();
        foreach (InputAction action in Rewired.ReInput.mapping.Actions)
        {
            actions.Add(action.name);
            times.Add(-1f);
        }
    }

    private void LateUpdate()
    {
        for (int i = 0; i < actions.Count; i++)
        {
            if (inputPlayer.GetButtonDown(actions[i]))
            {
                times[i] = Time.time;
            }
        }
    }

    public Vector2 ApplyDeadZone(Vector2 input)
    {
        Vector2 normalizedInput = input.normalized;
        if (Mathf.Abs(normalizedInput.x) < xAngleDeadZone) input.x = 0;
        if (Mathf.Abs(normalizedInput.y) < yAngleDeadZone) input.y = 0;
        return input.normalized;
    }

    public float ApplyDeadZone(float axis)
    {
        if (Mathf.Abs(axis) < xAngleDeadZone) return 0f;
        return axis;
    }

    public bool IsDown(string actionName, float timeWindow = 0.2f)
    {
        if (!actions.Contains(actionName))
        {
            return false;
        }
        int index = actions.FindIndex(s => s == actionName);
        return times[index] >= 0
               && Time.time - times[index] < timeWindow;
    }

    public void ConsumeButton(String actionName)
    {
        if (actions.Contains(actionName))
        {
            int index = actions.FindIndex(s => s == actionName);
            times[index] = -1f;
        }
    }
}
