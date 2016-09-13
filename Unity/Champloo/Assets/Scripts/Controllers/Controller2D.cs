using System;
using UnityEngine;
using CollisionChecking;
using System.Collections;
using System.Collections.Generic;

public class Controller2D : RaycastController
{
    public struct CollisionInfo
    {
        public bool Above { get; set; }
        public bool Below { get; set; }
        public bool Left { get; set; }
        public bool Right { get; set; }

        public bool ClimbingSlope { get; set; }
        public bool DescendingSlope { get; set; }

        public float SlopeAngle { get; set; }
        public float SlopeAngleOld { get; set; }

        public void Reset()
        {
            Above = Below = Left = Right = false;
            ClimbingSlope = DescendingSlope = false;

            SlopeAngleOld = SlopeAngle;
            SlopeAngle = 0;
        }
    }

    [SerializeField] private float maxClimbAngle = 80f;
    [SerializeField] private float maxDescendAngle = 75f;
    [SerializeField] public LayerMask collisionMask;
    [SerializeField] public LayerMask crushMask;
    public CollisionInfo collisions;

    private Vector3 previousVelocity;

    [HideInInspector]
    public int faceDirection;

    public event Action<object, GameObject> Crushed;
    public event Action<object, GameObject, CollisionInfo> Collision;
    public event Action<object, Player> Smashed;
    public event Action<object, Player> Stomped;
    public event Action<object, Player, bool> Bounced;

    private static float stompBounce = 10f;

    protected override void Start()
    {
        base.Start();
        faceDirection = 1;
    }

    protected virtual void OnCrushed(GameObject other)
    {
        if (Crushed != null)
        {
            Crushed(this, other);
        }
    }

    protected virtual void OnCollision(GameObject other, CollisionInfo info)
    {
        if (Collision != null && other.activeSelf)
        {
            Collision(this, other, info);
        }
    }
    /*
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
    */
    private RaycastHit2D Raycast(Vector2 rayOrigin, Vector2 direction, float distance, LayerMask mask)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, direction, distance, mask.value);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.gameObject != gameObject
                && hits[i].transform.gameObject.activeInHierarchy)
            {
                return hits[i];
            }
        }
        return new RaycastHit2D();
    }

    private RaycastHit2D[] RaycastAll(Vector2 rayOrigin, Vector2 direction, float distance, LayerMask mask)
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, direction, distance, mask.value);
        List<RaycastHit2D> hitsList = new List<RaycastHit2D>();
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.gameObject != gameObject
                && hits[i].transform.gameObject.activeInHierarchy
                && !hitsList.Contains(hits[i]))
            {
                hitsList.Add(hits[i]);
            }
            /*
            if (hits[i].transform.gameObject != gameObject
                && hits[i].transform.gameObject.activeSelf)
                return hits[i];
            if (hitsList[i].transform.gameObject == gameObject
                || !hitsList[i].transform.gameObject.activeSelf)
            {
                hitsList.RemoveAt(i);
            }
            */
        }

        return hitsList.ToArray();
    }

    public void UpdateTouching()
    {
        //above and below
        collisions.Above = false;
        collisions.Below = false;
        List<GameObject> allHitObjects = new List<GameObject>();
        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (horizontalRaySpacing * i);
            RaycastHit2D[] hits = RaycastAll(rayOrigin, Vector2.up, skinWidth*2, collisionMask);
            if(hits.Length > 0)
            {
                collisions.Above = true;
                /*
                Player other = hit.transform.GetComponent<Player>();
                if(other != null)
                {
                    if (other.CurrentMovementState is OnDash)
                        OnStompedBy(other);
                    else OnBounce(other, false);
                }
                */
            }
            foreach (RaycastHit2D hit in hits)
            {
                if (!allHitObjects.Contains(hit.transform.gameObject))
                {
                    allHitObjects.Add(hit.transform.gameObject);
                    OnCollision(hit.transform.gameObject, new CollisionInfo {Above = true});
                }
            }


            rayOrigin = raycastOrigins.bottomLeft;
            rayOrigin += Vector2.right * (horizontalRaySpacing * i);
            hits = RaycastAll(rayOrigin, Vector2.down, skinWidth * 2, collisionMask);
            if (hits.Length > 0)
            {
                collisions.Below = true;
                /*
                Player other = hit.transform.GetComponent<Player>();
                Player us = GetComponent<Player>();
                if(other != null && us != null)
                {
                    OnSmash(other);
                }
                */
            }
            foreach (RaycastHit2D hit in hits)
            {
                if (!allHitObjects.Contains(hit.transform.gameObject))
                {
                    allHitObjects.Add(hit.transform.gameObject);
                    OnCollision(hit.transform.gameObject, new CollisionInfo {Below = true});
                }
            }
        }

        //left and right
        collisions.Left = false;
        collisions.Right = false;
        for(int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = raycastOrigins.bottomLeft;
            rayOrigin += Vector2.up * (verticalRaySpacing * i);
            RaycastHit2D[] hits = RaycastAll(rayOrigin, Vector2.left, skinWidth * 2, collisionMask);
            if (hits.Length > 0)
            {
                collisions.Left = true;
                /*
                Player other = hit.transform.GetComponent<Player>();
                if(other != null)
                {
                    OnBounce(other, true);
                }
                */
            }
            foreach (RaycastHit2D hit in hits)
            {
                if (!allHitObjects.Contains(hit.transform.gameObject))
                {
                    allHitObjects.Add(hit.transform.gameObject);
                    OnCollision(hit.transform.gameObject, new CollisionInfo {Left = true});
                }
            }

            rayOrigin = raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (verticalRaySpacing * i);
            hits = RaycastAll(rayOrigin, Vector2.right, skinWidth * 2, collisionMask);
            if(hits.Length > 0)
            {
                collisions.Right = true;
                /*
                Player other = hit.transform.GetComponent<Player>();
                if (other != null)
                {
                    OnBounce(other, true);
                }
                */
            }
            foreach (RaycastHit2D hit in hits)
            {
                if (!allHitObjects.Contains(hit.transform.gameObject))
                {
                    allHitObjects.Add(hit.transform.gameObject);
                    OnCollision(hit.transform.gameObject, new CollisionInfo {Right = true});
                }
            }
        }
    }

    public void Move(Vector3 velocity, bool standingOnPlatform = false)
    {
        //handle collisions
        UpdateRaycastOrigins();

        previousVelocity = velocity;
        collisions.Reset();

        UpdateTouching();
        if (velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }
        if (velocity.x != 0)
        {
            faceDirection = (int)Mathf.Sign(velocity.x);
            HorizontalCollisions(ref velocity);
        }

        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }
        transform.Translate(velocity);

        if (standingOnPlatform)
        {
            collisions.Below = true;
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
            RaycastHit2D[] hits = RaycastAll(rayOrigin, Vector2.up * directionY, magnitudeY, crushMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * magnitudeY, Color.red);

            //check being crushed
            Vector3 newVelocity = Vector3.zero;
            foreach (RaycastHit2D h in hits)
            {
                if (h.distance == 0)
                {
                    OnCrushed(h.transform.gameObject);
                }

                newVelocity.y += (hit.distance - skinWidth) * directionY;

                if (collisions.ClimbingSlope)
                {
                    newVelocity.x += velocity.y / Mathf.Tan(collisions.SlopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                collisions.Below = directionY == -1;
                collisions.Above = directionY == 1;
            }

            if (hits.Length > 0)
            {
                newVelocity = newVelocity/hits.Length;
                velocity.y = newVelocity.y;
            }

            if (hit)
            {
                //OnCollision(hit.transform.gameObject, new CollisionInfo {Above = directionY == 1, Below = directionY == -1});
                /*
                if (hit.distance == 0 && !Utility.IsLayer(ignoreCrushMask, hit.transform.gameObject.layer))
                {
                    OnCrushed(EventArgs.Empty);
                    continue;
                }
                */
                /*
                velocity.y = (hit.distance - skinWidth) * directionY;
                magnitudeY = hit.distance;
                if (collisions.ClimbingSlope)
                {
                    velocity.x = velocity.y/Mathf.Tan(collisions.SlopeAngle*Mathf.Deg2Rad)*Mathf.Sign(velocity.x);
                }
                
                collisions.Below = directionY == -1;
                collisions.Above = directionY == 1;
                */
            }
        }

        if (collisions.ClimbingSlope)
        {
            float directionX = faceDirection;
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) +
                                Vector2.up*velocity.y;
            RaycastHit2D[] hits = RaycastAll(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            float averageX = 0f;
            float averageAngle = 0f;
            foreach (RaycastHit2D hit in hits)
            {
                averageAngle = Vector2.Angle(hit.normal, Vector2.up);
                averageX += (hit.distance - skinWidth)*directionX;
            }
            if (hits.Length > 0)
            {
                collisions.SlopeAngle = averageAngle/hits.Length;
                velocity.x = averageAngle / hits.Length;
            }
            /*
            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.SlopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth)*directionX;
                    collisions.SlopeAngle = slopeAngle;
                }
            }
            */
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
            RaycastHit2D[] hits = RaycastAll(rayOrigin, Vector2.right * directionX, magnitudeX, crushMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * magnitudeX, Color.red);

            //check being crushed
            Vector3 newVelocity = Vector3.zero;
            foreach (RaycastHit2D h in hits)
            {
                if (h.distance == 0)
                {
                    OnCrushed(h.transform.gameObject);
                }

                newVelocity.x += (hit.distance - skinWidth) * directionX;

                if (collisions.ClimbingSlope)
                {
                    newVelocity.y = Mathf.Tan(collisions.SlopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                }

                collisions.Left = directionX == -1;
                collisions.Right = directionX == 1;
            }
            if (hits.Length > 0)
            {
                newVelocity = newVelocity / hits.Length;
                velocity.x = newVelocity.x;
            }

            if (hit)
            {
                //OnCollision(hit.transform.gameObject, new CollisionInfo { Right = directionX == 1, Left = directionX == -1});
                /*
                if (hit.distance == 0 && !Utility.IsLayer(ignoreCrushMask, hit.transform.gameObject.layer))
                {
                    OnCrushed(EventArgs.Empty);
                    continue;
                }
                */
                /*
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxClimbAngle)
                {
                    if (collisions.DescendingSlope)
                    {
                        collisions.DescendingSlope = false;
                        velocity = previousVelocity;
                    }
                    float distanceToSlopeStart = 0;
                    if (slopeAngle != collisions.SlopeAngleOld)
                    {
                        distanceToSlopeStart = hit.distance - skinWidth;
                        velocity.x -= distanceToSlopeStart*directionX;
                    }
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distanceToSlopeStart*directionX;
                }

                if (!collisions.ClimbingSlope || slopeAngle > maxClimbAngle)
                {
                    velocity.x = (hit.distance - skinWidth)*directionX;
                    magnitudeX = hit.distance;

                    if (collisions.ClimbingSlope)
                    {
                        velocity.y = Mathf.Tan(collisions.SlopeAngle*Mathf.Deg2Rad)*Mathf.Abs(velocity.x);
                    }

                    collisions.Left = directionX == -1;
                    collisions.Right = directionX == 1;
                }
                */
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
            collisions.Below = true;
            collisions.ClimbingSlope = true;
            collisions.SlopeAngle = slopeAngle;
        }
    }

    void DescendSlope(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Raycast(rayOrigin, Vector2.down, Mathf.Infinity, collisionMask);

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

                collisions.SlopeAngle = slopeAngle;
                collisions.DescendingSlope = true;
                collisions.Below = true;
            }
        }
    }
}