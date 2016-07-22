using System;
using UnityEngine;
using System.Collections;
using GamepadInput;

public class InputController : MonoBehaviour
{
    private static bool HandleInput = true;

    public static void SetInputs(bool on)
    {
        HandleInput = on;
    }

    [Serializable]
    public class ButtonSetting
    {
        private GamePad.Index playerIndex;
        [SerializeField] private GamePad.Button buttonIndex;
        //private string input;

        private bool isDown;
        private bool isUp;
        private bool isPressed;

        public bool Down { get { return isDown || onDownTime > 0; } }
        public bool Up { get { return isUp || onUpTime > 0; } }
        public bool Pressed { get { return isPressed || onPressedTime > 0; } }

        /// <summary>
        /// Time (in seconds) to allow down event after it occurs
        /// </summary>
        [SerializeField]
        private float windowDownTime;

        /// <summary>
        /// Time (in seconds) to allow up event after it occurs
        /// </summary>
        [SerializeField]
        private float windowUpTime;

        /// <summary>
        /// Time (in seconds) to allow press event after it occurs
        /// </summary>
        [SerializeField]
        private float windowPressedTime;

        private float onDownTime;
        private float onUpTime;
        private float onPressedTime;

        public void Update(GamePad.Index player = GamePad.Index.Any)
        {
            playerIndex = player;

            isDown = GamePad.GetButtonDown(buttonIndex, playerIndex);//Input.GetButtonDown(input);
            isUp = GamePad.GetButtonUp(buttonIndex, playerIndex);//Input.GetButtonUp(input);
            isPressed = GamePad.GetButton(buttonIndex, playerIndex);//Input.GetButton(input);

            if (isDown) onDownTime = windowDownTime;
            if (isUp) onUpTime = windowUpTime;
            if (isPressed) onPressedTime = windowPressedTime;

            onDownTime -= Time.deltaTime;
            onUpTime -= Time.deltaTime;
            onPressedTime -= Time.deltaTime;
        }

        public void ResetTimers()
        {
            onDownTime = 0;
            onUpTime = 0;
            onPressedTime = 0;
        }
    }

    public ButtonSetting jump;
    public ButtonSetting attack;
    public ButtonSetting weaponSpecial;
    public ButtonSetting movementSpecial;

    public ButtonSetting block;
    public ButtonSetting parry;

    public ButtonSetting taunt;
    public ButtonSetting pause;

    [HideInInspector] public Vector2 leftStick;
    public float leftStickAngle { get { return Vector2AsAngle(leftStick);} }
    [HideInInspector] public Vector2 rightStick;
    public float rightStickAngle { get { return Vector2AsAngle(rightStick); } }
    [HideInInspector] public Vector2 dPad;

    [HideInInspector] public int playerNumber;
    private GamePad.Index playerIndex;

    private float Vector2AsAngle(Vector2 vec)
    {
        if (Mathf.Abs(vec.x) < float.Epsilon) return vec.y > 0 ? 90 : -90;
        return Mathf.Atan(vec.y/vec.x)*Mathf.Rad2Deg + (vec.x < 0 ? 180 : 0);
    }

    public void UpdateInputs()
    {
        if (!HandleInput) return;
        switch (playerNumber)
        {
            case 1:
                playerIndex = GamePad.Index.One;
                break;
            case 2:
                playerIndex = GamePad.Index.Two;
                break;
            case 3:
                playerIndex = GamePad.Index.Three;
                break;
            case 4:
                playerIndex = GamePad.Index.Four;
                break;
            default:
                playerIndex = GamePad.Index.Any;
                break;
        }

        jump.Update(playerIndex);
        attack.Update(playerIndex);
        weaponSpecial.Update(playerIndex);
        movementSpecial.Update(playerIndex);

        taunt.Update(playerIndex);
        pause.Update(playerIndex);

        leftStick = GamePad.GetAxis(GamePad.Axis.LeftStick, playerIndex);
        rightStick = GamePad.GetAxis(GamePad.Axis.RightStick, playerIndex);
        dPad = GamePad.GetAxis(GamePad.Axis.Dpad, playerIndex);
    }
}
