// Copyright (c) 2014 Augie R. Maddox, Guavaman Enterprises. All rights reserved.

namespace Rewired.Demos {

    using UnityEngine;
    using System.Collections;
    using Rewired;

    [AddComponentMenu("")]
    [RequireComponent(typeof(CharacterController))]
    public class EightPlayersExample_Player : MonoBehaviour {

        public int playerId = 0; // The Rewired player id of this character

        public float moveSpeed = 3.0f;
        public float bulletSpeed = 15.0f;
        public GameObject bulletPrefab;

        public float angleDeadZone = 10f;
        private float xAngleDeadZone;
        private float yAngleDeadZone;

        private Player player; // The Rewired Player
        private CharacterController cc;
        private Vector3 moveVector;
        private bool fire;

        [System.NonSerialized] // Don't serialize this so the value is lost on an editor script recompile.
        private bool initialized;

        void Awake() {
            // Get the character controller
            cc = GetComponent<CharacterController>();
        }

        private void Initialize() {
            // Get the Rewired Player object for this player.
            player = ReInput.players.GetPlayer(playerId);
            
            initialized = true;

            xAngleDeadZone = Mathf.Sin(angleDeadZone * Mathf.Deg2Rad); //divided by 2 to center it
            yAngleDeadZone = Mathf.Cos(angleDeadZone * Mathf.Deg2Rad);
        }

        void Update() {
            if(!ReInput.isReady) return; // Exit if Rewired isn't ready. This would only happen during a script recompile in the editor.
            if(!initialized) Initialize(); // Reinitialize after a recompile in the editor

            GetInput();
            ProcessInput();
        }

        private void GetInput() {
            // Get the input from the Rewired Player. All controllers that the Player owns will contribute, so it doesn't matter
            // whether the input is coming from a joystick, the keyboard, mouse, or a custom controller.

            moveVector.x = player.GetAxis("Move Horizontal"); // get input by name or action id
            moveVector.y = player.GetAxis("Move Vertical");
            
            Vector2 normalizedMoveVector = moveVector.normalized;
            if (Mathf.Abs(normalizedMoveVector.x) < xAngleDeadZone) moveVector.x = 0;
            if (Mathf.Abs(normalizedMoveVector.y) < xAngleDeadZone) moveVector.y = 0;

            moveVector.Normalize();
            
            fire = player.GetButtonDown("Fire");
        }

        private void ProcessInput() {
            // Process movement
            if(moveVector.x != 0.0f || moveVector.y != 0.0f) {
                cc.Move(moveVector * moveSpeed * Time.deltaTime);
            }

            // Process fire
            if(fire) {
                GameObject bullet = (GameObject)Instantiate(bulletPrefab, transform.position + transform.right, transform.rotation);
                bullet.GetComponent<Rigidbody>().AddForce(transform.right * bulletSpeed, ForceMode.VelocityChange);
            }
        }
    }

}
