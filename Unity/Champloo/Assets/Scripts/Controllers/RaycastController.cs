using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class RaycastController : MonoBehaviour
{
    protected struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }

    protected RaycastOrigins raycastOrigins;

    protected const float skinWidth = 0.15f;//.015f;
    private BoxCollider2D _collider;
    private Bounds _previousColliderBounds;
    public Bounds ColliderBounds { get { return _collider.bounds; } }
    public Bounds PreviousColliderBounds { get { return _previousColliderBounds; } }

    [SerializeField]
    protected int horizontalRayCount = 4;
    [SerializeField]
    protected int verticalRayCount = 4;

    protected float horizontalRaySpacing;
    protected float verticalRaySpacing;

    protected virtual void Start()
    {
        _collider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    public virtual void UpdateBounds(Bounds newBounds)
    {
        _previousColliderBounds = _collider.bounds;
        _collider.size = newBounds.size;
        _collider.offset = transform.InverseTransformPoint(newBounds.center);
    }

    public void UpdateRaycastOrigins()
    {
        CalculateRaySpacing();
        Bounds bounds = _collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = transform.position - bounds.extents;
        raycastOrigins.topRight = transform.position + bounds.extents;

        raycastOrigins.bottomRight = raycastOrigins.bottomLeft + Vector2.right * bounds.size.x;
        raycastOrigins.topLeft = raycastOrigins.bottomLeft + Vector2.up * bounds.size.y;
    }

    protected void CalculateRaySpacing()
    {
        Bounds bounds = _collider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }
}
