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

    private Dictionary<String, bool> consumedActions;

    List<String> releasedKeys;

    private void Start()
    {
        xAngleDeadZone = Mathf.Sin(angleDeadzone * Mathf.Deg2Rad);
        yAngleDeadZone = Mathf.Cos(angleDeadzone * Mathf.Deg2Rad);

        consumedActions = new Dictionary<string, bool>();

        inputPlayer = GetComponentInParent<Player>().InputPlayer;
        releasedKeys = new List<String>();
    }

    private void LateUpdate()
    {
        //unconsume non-pressed buttons
        releasedKeys.Clear();
        foreach (KeyValuePair<string, bool> pair in consumedActions)
        {
            if (!inputPlayer.GetButtonDown(pair.Key))
            {
                releasedKeys.Add(pair.Key);
            }
        }

        foreach (String key in releasedKeys)
        {
            consumedActions[key] = false;
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

    public bool IsConsumed(String actionName)
    {
        return consumedActions.ContainsKey(actionName) && consumedActions[actionName];
    }

    public void ConsumeButton(String actionName)
    {
        if (inputPlayer.GetButton(actionName))
        {
            consumedActions[actionName] = true;
        }
    }
}
