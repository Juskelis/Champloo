using System;
using UnityEngine;
using System.Collections.Generic;

public class Controller2D : RaycastController
{
    [Serializable]
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

    [SerializeField] private float maxSlopeAngle = 80f;
    [SerializeField] public LayerMask collisionMask;
    [SerializeField] public LayerMask notifyMask; //notify us when we hit it, but don't interact with it directly
    [SerializeField] public LayerMask crushMask;
    public CollisionInfo collisions;

    private Vector3 previousVelocity;

    private int faceDirection;

    public event Action<object, GameObject> Crushed;
    public event Action<object, GameObject, CollisionInfo> Collision;

    private static float stompBounce = 10f;

    private List<RaycastHit2D> hitsList;
    private List<GameObject> allHitObjects;
    private RaycastHit2D[] hitsArray = new RaycastHit2D[100];
    private Collider2D[] crushersArray = new Collider2D[50];

    protected override void Start()
    {
        base.Start();
        faceDirection = 1;

        hitsList = new List<RaycastHit2D>();
        allHitObjects = new List<GameObject>();
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

    private RaycastHit2D Raycast(Vector2 rayOrigin, Vector2 direction, float distance, LayerMask mask)
    {
        Debug.DrawLine(rayOrigin, rayOrigin + direction * distance, Color.blue);
        int hitCount = Physics2D.RaycastNonAlloc(rayOrigin, direction, hitsArray, distance, mask.value);
        for (int i = 0; i < hitCount; i++)
        {
            if (hitsArray[i].transform.gameObject != gameObject
                && hitsArray[i].transform.gameObject.activeInHierarchy)
            {
                return hitsArray[i];
            }
        }
        return new RaycastHit2D();
    }

    private void RaycastAll(Vector2 rayOrigin, Vector2 direction, float distance, LayerMask mask)
    {
        Debug.DrawLine(rayOrigin, rayOrigin + direction*distance, Color.blue);
        int hitCount = Physics2D.RaycastNonAlloc(rayOrigin, direction, hitsArray, distance, mask.value);
        hitsList.Clear();
        for (int i = 0; i < hitCount; i++)
        {
            if (hitsArray[i].transform.gameObject != gameObject
                && hitsArray[i].transform.gameObject.activeInHierarchy
                && !hitsList.Contains(hitsArray[i]))
            {
                hitsList.Add(hitsArray[i]);
            }
        }
    }

    private void NotifyContact()
    {
        allHitObjects.Clear();

        //left and right
        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = raycastOrigins.bottomLeft;
            rayOrigin += Vector2.up * (verticalRaySpacing * i);
            RaycastAll(rayOrigin, Vector2.left, skinWidth * 2, notifyMask);
            foreach (RaycastHit2D hit in hitsList)
            {
                if (!allHitObjects.Contains(hit.transform.gameObject))
                {
                    allHitObjects.Add(hit.transform.gameObject);
                    OnCollision(hit.transform.gameObject, new CollisionInfo { Left = true });
                }
            }

            rayOrigin = raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (verticalRaySpacing * i);
            RaycastAll(rayOrigin, Vector2.right, skinWidth * 2, notifyMask);
            foreach (RaycastHit2D hit in hitsList)
            {
                if (!allHitObjects.Contains(hit.transform.gameObject))
                {
                    allHitObjects.Add(hit.transform.gameObject);
                    OnCollision(hit.transform.gameObject, new CollisionInfo { Right = true });
                }
            }
        }

        //above and below
        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (horizontalRaySpacing * i);
            RaycastAll(rayOrigin, Vector2.up, skinWidth * 2, notifyMask);
            foreach (RaycastHit2D hit in hitsList)
            {
                if (!allHitObjects.Contains(hit.transform.gameObject))
                {
                    allHitObjects.Add(hit.transform.gameObject);
                    OnCollision(hit.transform.gameObject, new CollisionInfo { Above = true });
                }
            }


            rayOrigin = raycastOrigins.bottomLeft;
            rayOrigin += Vector2.right * (horizontalRaySpacing * i);
            RaycastAll(rayOrigin, Vector2.down, skinWidth * 2, notifyMask);
            foreach (RaycastHit2D hit in hitsList)
            {
                if (!allHitObjects.Contains(hit.transform.gameObject))
                {
                    allHitObjects.Add(hit.transform.gameObject);
                    OnCollision(hit.transform.gameObject, new CollisionInfo { Below = true });
                }
            }
        }
    }

    private void UpdateTouching()
    {
        //above and below
        collisions.Above = false;
        collisions.Below = false;
        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (horizontalRaySpacing * i);
            RaycastAll(rayOrigin, Vector2.up, skinWidth*2, collisionMask);
            if(hitsList.Count > 0)
            {
                collisions.Above = true;
            }

            rayOrigin = raycastOrigins.bottomLeft;
            rayOrigin += Vector2.right * (horizontalRaySpacing * i);
            RaycastAll(rayOrigin, Vector2.down, skinWidth * 2, collisionMask);
            if (hitsList.Count > 0)
            {
                collisions.Below = true;
            }
        }

        //left and right
        collisions.Left = false;
        collisions.Right = false;
        for(int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = raycastOrigins.bottomLeft;
            rayOrigin += Vector2.up * (verticalRaySpacing * i);
            RaycastAll(rayOrigin, Vector2.left, skinWidth * 2, collisionMask);
            if (hitsList.Count > 0)
            {
                collisions.Left = true;
            }

            rayOrigin = raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (verticalRaySpacing * i);
            RaycastAll(rayOrigin, Vector2.right, skinWidth * 2, collisionMask);
            if(hitsList.Count > 0)
            {
                collisions.Right = true;
            }
        }
    }

    private void GetCrushers()
    {
        Bounds crushBounds = ColliderBounds;
        crushBounds.Expand(skinWidth*-2.5f);

        //Draw box
        Vector3 boxMin = transform.position - crushBounds.extents;
        Vector3 boxMax = transform.position + crushBounds.extents;
        Debug.DrawLine(boxMin, boxMin + Vector3.up * crushBounds.size.y, Color.yellow);
        Debug.DrawLine(boxMin, boxMin + Vector3.right * crushBounds.size.x, Color.yellow);
        Debug.DrawLine(boxMax, boxMax + Vector3.down * crushBounds.size.y, Color.yellow);
        Debug.DrawLine(boxMax, boxMax + Vector3.left * crushBounds.size.x, Color.yellow);

        int crushCount = Physics2D.OverlapBoxNonAlloc(
            transform.position,
            crushBounds.size,
            0f,
            crushersArray,
            crushMask.value);

        allHitObjects.Clear();
        for (int i = 0; i < crushCount; i++)
        {
            Collider2D curr = crushersArray[i];
            if (curr.transform.gameObject != gameObject)
            {
                allHitObjects.Add(curr.transform.gameObject);
            }
        }
    }

    public void Move(Vector3 velocity, bool standingOnPlatform = false)
    {

        UpdateRaycastOrigins();

        previousVelocity = velocity;
        collisions.Reset();

        UpdateTouching();
        NotifyContact();

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

        GetCrushers();
        foreach (GameObject obj in allHitObjects)
        {
            OnCrushed(obj);
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
            RaycastHit2D hit = Raycast(rayOrigin, Vector2.up*directionY, magnitudeY, collisionMask);

            if (hit)
            {
                velocity.y = (hit.distance - skinWidth)*directionY;
                magnitudeY = hit.distance;

                if (collisions.ClimbingSlope)
                {
                    velocity.x += velocity.y / Mathf.Tan(collisions.SlopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }
            }
        }

        if (collisions.ClimbingSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) +
                                Vector2.up*velocity.y;

            RaycastHit2D hit = Raycast(rayOrigin, Vector2.right*directionX, rayLength, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.SlopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth)*directionX;
                    collisions.SlopeAngle = slopeAngle;
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
            RaycastHit2D hit = Raycast(rayOrigin, Vector2.right*directionX, magnitudeX, collisionMask);

            if (hit)
            {
                if (hit.distance == float.Epsilon) continue;

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                //when i == 0 we're casting the bottommost ray
                if (i == 0 && slopeAngle <= maxSlopeAngle)
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

                if (!collisions.ClimbingSlope || slopeAngle > maxSlopeAngle)
                {
                    velocity.x = (hit.distance - skinWidth)*directionX;
                    magnitudeX = hit.distance;

                    if (collisions.ClimbingSlope)
                    {
                        velocity.y = Mathf.Tan(collisions.SlopeAngle*Mathf.Deg2Rad)*Mathf.Abs(velocity.x);
                    }
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
            velocity.x = Mathf.Cos(slopeAngle*Mathf.Deg2Rad)*moveDistance*Mathf.Sign(velocity.x);
            collisions.Below = true;
            collisions.ClimbingSlope = true;
            collisions.SlopeAngle = slopeAngle;
        }
    }

    void DescendSlope(ref Vector3 velocity)
    {

        RaycastHit2D slopeHitLeft = Raycast(
            raycastOrigins.bottomLeft,
            Vector2.down,
            Mathf.Abs(velocity.y) + skinWidth,
            collisionMask);
        RaycastHit2D slopeHitRight = Raycast(
            raycastOrigins.bottomRight,
            Vector2.down,
            Mathf.Abs(velocity.y) + skinWidth,
            collisionMask);

        bool slidingDownSlope = false;
        if (slopeHitLeft ^ slopeHitRight)
        {
            slidingDownSlope |= SlideDownSlope(slopeHitLeft, ref velocity);
            slidingDownSlope |= SlideDownSlope(slopeHitRight, ref velocity);
        }

        if (!slidingDownSlope)
        {
            float directionX = Mathf.Sign(velocity.x);
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
            RaycastHit2D hit = Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle && Mathf.Sign(hit.normal.x) == directionX
                    && hit.distance - skinWidth <= Mathf.Tan(slopeAngle*Mathf.Deg2Rad)*Mathf.Abs(velocity.x))
                {
                    float moveDistance = Mathf.Abs(velocity.x);
                    float descendMoveAmountY = Mathf.Sin(slopeAngle*Mathf.Deg2Rad)*moveDistance;
                    velocity.x = Mathf.Cos(slopeAngle*Mathf.Deg2Rad)*moveDistance*directionX;
                    velocity.y -= descendMoveAmountY;

                    collisions.SlopeAngle = slopeAngle;
                    collisions.DescendingSlope = true;
                    collisions.Below = true;
                }
            }
        }
    }

    bool SlideDownSlope(RaycastHit2D hit, ref Vector3 velocity)
    {
        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle > maxSlopeAngle)
            {
                velocity.x = Mathf.Sign(hit.normal.x)
                             *(Mathf.Abs(velocity.y) - hit.distance)/Mathf.Tan(slopeAngle*Mathf.Deg2Rad);

                collisions.SlopeAngle = slopeAngle;
                return true;
            }
        }
        return false;
    }
}