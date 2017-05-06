using UnityEngine;
using Rewired;
using UnityEngine.Networking;
using System.Collections;

public class Utility {

    public static T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy as T;
    }

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
        /*
        for (int i = 0; i < ClientScene.localPlayers.Count; i++)
        {
            if (ClientScene.localPlayers[i].playerControllerId == networkControllerID)
            {
                return ReInput.players.GetPlayer(i);
            }
        }
        return null;
        */
        int localID = GetLocalPlayerNumber(networkControllerID);
        return localID >= 0 ? ReInput.players.GetPlayer(localID) : null;
    }

    public static int GetLocalPlayerNumber(int networkControllerID)
    {
        for (int i = 0; i < ClientScene.localPlayers.Count; i++)
        {
            if (ClientScene.localPlayers[i].playerControllerId == networkControllerID)
            {
                return ClientScene.localPlayers[i].gameObject.GetComponent<PlayerSettings>().LocaInputPlayerID;
            }
        }
        return -1;
    }
}
