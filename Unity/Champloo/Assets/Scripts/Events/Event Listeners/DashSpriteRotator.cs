using System;
using UnityEngine;

public class DashSpriteRotator : MonoBehaviour {

    [SerializeField]
    private Transform rotateAround;

    private bool rotating = false;

    private OnDash dash;

    private Vector3 defaultScale;

    public void Start()
    {
        defaultScale = transform.localScale;
        (GetComponentInParent<LocalEventDispatcher>()??GetComponentInChildren<LocalEventDispatcher>())
            .AddListener<MovementStateChangedEvent>(OnChange);
    }

    public void Update()
    {
        if (!rotating)
        {
            transform.position = transform.parent.position;
            transform.rotation = Quaternion.identity;
            transform.localScale = defaultScale;
        }
        else
        {
            Vector2 correctedDirection = dash.Direction;
            if (dash.Direction.x < 0)
            {
                correctedDirection.x *= -1;
                correctedDirection.y *= -1;
            }
            float angle = Utility.Vector2AsAngle(correctedDirection);
            Vector3 rotationVector = Vector3.forward * angle;
            transform.position = Utility.RotatePointAroundPivot(
                transform.parent.position,
                rotateAround.position,
                rotationVector);
            transform.rotation = Quaternion.Euler(rotationVector);
        }
    }

    private void OnChange(object sender, EventArgs args)
    {
        MovementStateChangedEvent e = (MovementStateChangedEvent)args;
        if(e.Next is OnDash)
        {
            rotating = true;
            dash = (OnDash)e.Next;
        }
        else
        {
            rotating = false;
        }
    }
}
