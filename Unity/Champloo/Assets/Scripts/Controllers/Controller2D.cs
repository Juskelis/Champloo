using System;
using UnityEngine;
using System.Collections;

public class Controller2D : RaycastController
{

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope, descendingSlope;

        public float slopeAngle, slopeAngleOld;

        public void Reset()
        {
            above = below = left = right = false;
            climbingSlope = descendingSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }

    [SerializeField] private float maxClimbAngle = 80f;
    [SerializeField] private float maxDescendAngle = 75f;
    [SerializeField] public LayerMask collisionMask;
    public CollisionInfo collisions;

    private Vector3 previousVelocity;

    [HideInInspector]
    public int faceDirection;

    public event EventHandler Crushed;
    public event Action<object, Player> Smashed;
    public event Action<object, Player> Stomped;
    public event Action<object, Player, bool> Bounced;

    private static float stompBounce = 10f;

    protected override void Start()
    {
        base.Start();
        faceDirection = 1;
    }

    protected virtual void OnCrushed(EventArgs e)
    {
        if (Crushed != null)
        {
            Crushed(this, e);
        }
    }

    protected virtual void OnSmash(Player other)
    {
        if (Smashed != null && other.gameObject.activeSelf)
        {
            Smashed(this, other);
        }
    }

    protected virtual void OnStompedBy(Player other)
    {
        Player us = GetComponent<Player>();
        if(Stomped != null && other.gameObject.activeSelf && us.isActiveAndEnabled)
        {
            other.GetComponent<Controller2D>().OnSmash(us);
            Stomped(this, other);
        }
    }

    protected virtual void OnBounce(Player other, bool horizontal)
    {
        if(Bounced != null)
        {
            Bounced(this, other, horizontal);
        }
    }

    private RaycastHit2D Raycast(Vector2 rayOrigin, Vector2 direction, float distance, LayerMask mask)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, direction, distance, mask);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.gameObject != gameObject
                && hits[i].transform.gameObject.activeSelf) return hits[i];
        }
        return new RaycastHit2D();
    }

    public void UpdateTouching()
    {
        //above and below
        collisions.above = false;
        collisions.below = false;
        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (horizontalRaySpacing * i);
            RaycastHit2D hit = Raycast(rayOrigin, Vector2.up, skinWidth*2, collisionMask);
            //Physics2D.Raycast(rayOrigin, Vector2.up, skinWidth * 2, collisionMask);
            if(hit)
            {
                collisions.above = true;
                Player other = hit.transform.GetComponent<Player>();
                if(other != null)
                {
                    if (other.CurrentMovementState is OnDash)
                        OnStompedBy(other);
                    else OnBounce(other, false);
                }
            }

            rayOrigin = raycastOrigins.bottomLeft;
            rayOrigin += Vector2.right * (horizontalRaySpacing * i);
            hit = Raycast(rayOrigin, Vector2.down, skinWidth * 2, collisionMask);
            //Physics2D.Raycast(rayOrigin, Vector2.down, skinWidth * 2, collisionMask);
            if (hit)
            {
                collisions.below = true;
                Player other = hit.transform.GetComponent<Player>();
                Player us = GetComponent<Player>();
                if(other != null && us != null)
                {
                    OnSmash(other);
                }
            }
        }

        //left and right
        collisions.left = false;
        collisions.right = false;
        for(int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = raycastOrigins.bottomLeft;
            rayOrigin += Vector2.up * (verticalRaySpacing * i);
            RaycastHit2D hit = Raycast(rayOrigin, Vector2.left, skinWidth * 2, collisionMask);
            //Physics2D.Raycast(rayOrigin, Vector2.left, skinWidth * 2, collisionMask);
            if (hit)
            {
                collisions.left = true;
                Player other = hit.transform.GetComponent<Player>();
                if(other != null)
                {
                    OnBounce(other, true);
                }
            }

            rayOrigin = raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (verticalRaySpacing * i);
            hit = Raycast(rayOrigin, Vector2.right, skinWidth * 2, collisionMask);
            //Physics2D.Raycast(rayOrigin, Vector2.right, skinWidth * 2, collisionMask);
            if(hit)
            {
                collisions.right = true;
                Player other = hit.transform.GetComponent<Player>();
                if (other != null)
                {
                    OnBounce(other, true);
                }
            }
        }
    }

    public void Move(Vector3 velocity, bool standingOnPlatform = false)
    {
        //handle collisions
        UpdateRaycastOrigins();

        collisions.Reset();
        previousVelocity = velocity;

        UpdateTouching();

        if (velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }
        if (velocity.x != 0)
        {
            faceDirection = (int)Mathf.Sign(velocity.x);
        }

        HorizontalCollisions(ref velocity);

        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }

        transform.Translate(velocity);

        if (standingOnPlatform)
        {
            collisions.below = true;
        }
    }

    void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float magnitudeY = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Raycast(rayOrigin, Vector2.up * directionY, magnitudeY, collisionMask);
            //Physics2D.Raycast(rayOrigin, Vector2.up * directionY, magnitudeY, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * magnitudeY, Color.red);

            if (hit)
            {
                if (hit.distance == 0)
                {
                    OnCrushed(EventArgs.Empty);
                    continue;
                }

                velocity.y = (hit.distance - skinWidth) * directionY;
                magnitudeY = hit.distance;

                if (collisions.climbingSlope)
                {
                    velocity.x = velocity.y/Mathf.Tan(collisions.slopeAngle*Mathf.Deg2Rad)*Mathf.Sign(velocity.x);
                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }
        }

        if (collisions.climbingSlope)
        {
            float directionX = faceDirection;
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) +
                                Vector2.up*velocity.y;
            RaycastHit2D hit = Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);
            //Physics2D.Raycast(rayOrigin, Vector2.right*directionX, rayLength, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth)*directionX;
                    collisions.slopeAngle = slopeAngle;
                }
            }
        }
    }
    void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = faceDirection;
        float magnitudeX = Mathf.Abs(velocity.x) + skinWidth;

        if (Mathf.Abs(velocity.x) < skinWidth)
        {
            magnitudeX = skinWidth*2;
        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Raycast(rayOrigin, Vector2.right * directionX, magnitudeX, collisionMask);
            //Physics2D.Raycast(rayOrigin, Vector2.right * directionX, magnitudeX, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * magnitudeX, Color.red);

            if (hit)
            {
                if (hit.distance == 0)
                {
                    OnCrushed(EventArgs.Empty);
                    continue;
                }

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxClimbAngle)
                {
                    if (collisions.descendingSlope)
                    {
                        collisions.descendingSlope = false;
                        velocity = previousVelocity;
                    }
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        velocity.x -= distanceToSlopeStart*directionX;
                    }
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart*directionX;
                }

                if (!collisions.climbingSlope || slopeAngle > maxClimbAngle)
                {
                    velocity.x = (hit.distance - skinWidth)*directionX;
                    magnitudeX = hit.distance;

                    if (collisions.climbingSlope)
                    {
                        velocity.y = Mathf.Tan(collisions.slopeAngle*Mathf.Deg2Rad)*Mathf.Abs(velocity.x);
                    }

                    collisions.left = directionX == -1;
                    collisions.right = directionX == 1;
                }
            }
        }
    }

    void ClimbSlope(ref Vector3 velocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        if (velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle*Mathf.Deg2Rad)*moveDistance;
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
    }

    void DescendSlope(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Raycast(rayOrigin, Vector2.down, Mathf.Infinity, collisionMask); 
        //Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, collisionMask);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

            if (slopeAngle != 0 && slopeAngle <= maxDescendAngle
                && Mathf.Sign(hit.normal.x) == directionX
                && hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
            {
                float moveDistance = Mathf.Abs(velocity.x);
                float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                velocity.y -= descendVelocityY;

                collisions.slopeAngle = slopeAngle;
                collisions.descendingSlope = true;
                collisions.below = true;
            }
        }
    }
}