using UnityEngine;
using Rewired;
using UnityEngine.Networking;
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

    public static Rewired.Player GetNetworkPlayer(int networkControllerID)
    {
        for (int i = 0; i < ClientScene.localPlayers.Count; i++)
        {
            if (ClientScene.localPlayers[i].playerControllerId == networkControllerID)
            {
                return ReInput.players.GetPlayer(i);
            }
        }
        return null;
    }
}
