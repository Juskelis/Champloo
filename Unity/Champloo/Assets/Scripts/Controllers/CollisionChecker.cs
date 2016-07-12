using UnityEngine;
using System.Collections;

namespace CollisionChecking
{
    public class Place
    {
        public static bool Meeting(BoxCollider2D box, Vector2 pos, float skin = -0.5f, LayerMask mask = default(LayerMask))
        {
            if (box == null) return false;

            Bounds colBounds = box.bounds;
            colBounds.Expand(skin);

            Collider2D lastCol = Physics2D.OverlapArea(
                new Vector2(
                    pos.x - colBounds.extents.x,
                    pos.y - colBounds.extents.y
                ),
                new Vector2(
                    pos.x + colBounds.extents.x,
                    pos.y + colBounds.extents.y
                ),
                mask
            );

            return lastCol != null;
        }

        public static Collider2D[] Contains(BoxCollider2D box, Vector2 pos, float skin = -0.5f, LayerMask mask = default(LayerMask))
        {
            if (box == null) return null;

            Bounds colBounds = box.bounds;
            colBounds.Expand(skin);

            Collider2D[] cols = Physics2D.OverlapAreaAll(
                new Vector2(
                    pos.x - colBounds.extents.x,
                    pos.y - colBounds.extents.y
                ),
                new Vector2(
                    pos.x + colBounds.extents.x,
                    pos.y + colBounds.extents.y
                ),
                mask
            );

            return cols;
        }
    }
}