using System;
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

    public static string ColorToHex(Color color)
    {
        int red = Mathf.RoundToInt(color.r * 255);
        int green = Mathf.RoundToInt(color.g * 255);
        int blue = Mathf.RoundToInt(color.b * 255);

        string a = GetHex((int)Mathf.Floor(red / 16.0f));
        string b = GetHex((int)Mathf.Round(red % 16.0f));
        string c = GetHex((int)Mathf.Floor(green / 16.0f));
        string d = GetHex((int)Mathf.Round(green % 16.0f));
        string e = GetHex((int)Mathf.Floor(blue / 16.0f));
        string f = GetHex((int)Mathf.Round(blue % 16.0f));

        return a + b + c + d + e + f;
    }

    private static string GetHex(int num)
    {
        string alpha = "0123456789ABCDEF";
        return alpha[num].ToString();
    }
}
