using UnityEngine;
using System.Collections;

public class Utility {


    public static bool IsLayer(LayerMask mask, int layer)
    {
        return (mask.value & 1 << layer) != 0;
    }
}
