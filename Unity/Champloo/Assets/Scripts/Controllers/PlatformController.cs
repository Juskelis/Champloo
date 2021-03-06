﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformController : MonoBehaviour// : RaycastController
{
    protected struct PassengerMovement
    {
        public Transform transform { get; set; }
        public Vector3 velocity { get; set; }
        public Vector3 relativePosition { get; set; }
        public bool standingOnPlatform { get; set; }
        public bool moveBeforePlatform { get; set; }
    }

    [SerializeField] protected LayerMask passengerMask;

    protected Vector3 move = Vector3.one;

    private List<PassengerMovement> passengerMovement;

    private BoxCollider2D _collider;

    protected virtual void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
    }
	
	// Update is called once per frame
	void Update ()
	{
        //UpdateRaycastOrigins();

	    Vector3 velocity = CalculatePlatformMovement();
        
        CalculatePassengerMovement(velocity);

        MovePassengers(true);
        transform.Translate(velocity);
        MovePassengers(false);
    }

    protected virtual Vector3 CalculatePlatformMovement()
    {
        return move*Time.deltaTime;
    }

    void MovePassengers(bool beforeMovePlatform)
    {
        foreach (var passenger in passengerMovement)
        {
            if (passenger.moveBeforePlatform == beforeMovePlatform)
            {
                passenger.transform.GetComponent<Controller2D>().Move(passenger.velocity, passenger.standingOnPlatform);
                //passenger.transform.Translate(passenger.velocity);
                //passenger.transform.position = transform.position + passenger.relativePosition;
            }
        }
    }

    void CalculatePassengerMovement(Vector3 velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengerMovement = new List<PassengerMovement>();

        float directionY = Mathf.Sign(velocity.y);

        RaycastHit2D[] hits = Physics2D.BoxCastAll(
            _collider.bounds.center,
            _collider.bounds.size,
            0f,
            velocity.normalized,
            velocity.magnitude,
            passengerMask);

        foreach (RaycastHit2D hit in hits)
        {
            if (movedPassengers.Add(hit.transform))
            {
                Vector2 push = velocity;
                passengerMovement.Add(new PassengerMovement
                {
                    transform = hit.transform,
                    velocity = new Vector3(push.x, push.y),
                    relativePosition = hit.transform.position - transform.position,
                    standingOnPlatform = hit.normal == Vector2.down,
                    moveBeforePlatform = directionY > 0
                });
            }
        }
    }
    /*
    void CalculatePassengerMovement(Vector3 velocity)
    {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengerMovement = new List<PassengerMovement>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        //vertical moving platform
        if (velocity.y != 0 && directionY > 0)
        {
            float magnitudeY = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right*(verticalRaySpacing*i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up*directionY, magnitudeY, passengerMask.value);

                Debug.DrawRay(rayOrigin, Vector2.up * directionY);

                if (hit && !movedPassengers.Contains(hit.transform))
                {
                    movedPassengers.Add(hit.transform);

                    float pushX = directionY == 1 ? velocity.x : 0;
                    float pushY = velocity.y - (hit.distance - skinWidth)*directionY;

                    //hit.transform.Translate(pushX, pushY, 0);
                    passengerMovement.Add(new PassengerMovement
                    {
                        transform = hit.transform,
                        velocity = new Vector3(pushX,pushY),
                        relativePosition = hit.transform.position - transform.position,
                        standingOnPlatform = directionY == 1,
                        moveBeforePlatform = true
                    });
                }
            }
        }

        //horizontal moving platform
        if (velocity.x != 0)
        {
            float magnitudeX = Mathf.Abs(velocity.x) + skinWidth;

            for (int i = 0; i < horizontalRayCount; i++)
            {
                Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up*(horizontalRaySpacing*i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right*directionX, magnitudeX, passengerMask.value);

                Debug.DrawRay(rayOrigin, Vector2.right * directionX);

                if (hit && !movedPassengers.Contains(hit.transform))
                {
                    movedPassengers.Add(hit.transform);

                    float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
                    float pushY = -skinWidth;

                    //hit.transform.Translate(pushX, pushY, 0);
                    passengerMovement.Add(new PassengerMovement
                    {
                        transform = hit.transform,
                        velocity = new Vector3(pushX, pushY),
                        relativePosition = hit.transform.position - transform.position,
                        standingOnPlatform = false,
                        moveBeforePlatform = true
                    });
                }
            }
        }

        //passenger on top of horizontal or downward platform
        if (directionY == -1 || (velocity.y == 0 && velocity.x != 0))
        {
            float magnitudeY = skinWidth * 2;

            for (int i = 0; i < verticalRayCount; i++)
            {
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);
                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, magnitudeY, passengerMask.value);

                Debug.DrawRay(rayOrigin, Vector2.up);

                if (hit && !movedPassengers.Contains(hit.transform))
                {
                    movedPassengers.Add(hit.transform);

                    float pushX = velocity.x;
                    float pushY = velocity.y;

                    //hit.transform.Translate(pushX, pushY, 0);
                    passengerMovement.Add(new PassengerMovement
                    {
                        transform = hit.transform,
                        velocity = new Vector3(pushX, pushY),
                        relativePosition = hit.transform.position - transform.position,
                        standingOnPlatform = true,
                        moveBeforePlatform = false
                    });
                }
            }
        }
    }
    */
}
