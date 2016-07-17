using UnityEngine;
using System.Collections;

public class DummyPlayer : MonoBehaviour {

    private InputController input;

    private Rigidbody2D body;

    [SerializeField]
    private float force = float.Epsilon;

    void Start()
    {
        input = GetComponent<InputController>();
        body = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        input.UpdateInputs();

        Vector2 toApply = input.leftStick;
        toApply.y = 0;

        body.AddForce(toApply * force);
        body.velocity = Vector2.ClampMagnitude(body.velocity, 30f);
    }

}
