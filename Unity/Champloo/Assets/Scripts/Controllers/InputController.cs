using System;
using UnityEngine;
using System.Collections;
using GamepadInput;
using Rewired;

public class InputController : MonoBehaviour
{
    [SerializeField]
    private float angleDeadzone;

    public float AngleDeadzone
    {
        get { return angleDeadzone;}
    }

    private float xAngleDeadZone;
    private float yAngleDeadZone;

    private void Start()
    {
        xAngleDeadZone = Mathf.Sin(angleDeadzone * Mathf.Deg2Rad);
        yAngleDeadZone = Mathf.Cos(angleDeadzone * Mathf.Deg2Rad);
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
}
