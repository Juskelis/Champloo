using UnityEngine;
using System.Collections;

public class Utility {


    public static bool IsLayer(LayerMask mask, int layer)
    {
        return (mask.value & 1 << layer) != 0;
    }

    public static float Vector2AsAngle(Vector2 vec)
    {
        if (Mathf.Abs(vec.x) < float.Epsilon) return vec.y > 0 ? 90 : -90;
        return Mathf.Atan(vec.y / vec.x) * Mathf.Rad2Deg + (vec.x < 0 ? 180 : 0);
    }
}
