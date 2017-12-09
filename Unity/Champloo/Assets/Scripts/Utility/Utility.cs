using System;
using UnityEngine;
using Rewired;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class Utility {

    public static Transform[] FindAll(Transform parent, String childName)
    {
        List<Transform> ret = new List<Transform>();
        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i).name.Contains(childName))
            {
                ret.Add(parent.GetChild(i));
            }
        }
        return ret.ToArray();
    }

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

    /*
        see: https://rootllama.wordpress.com/2014/06/20/ray-line-segment-intersection-test-in-2d/
    */
    /// <summary>
    /// Determines if a given ray can intersect a given line segment. If it can, returns the point where they intersect.
    /// </summary>
    /// <param name="rayOrigin">The start of the ray</param>
    /// <param name="rayDir">The direction the ray is aiming in</param>
    /// <param name="segmentStart">The starting point of the line segment</param>
    /// <param name="segmentEnd">The ending point of the line segment</param>
    /// <param name="intersectionPoint">The point where the ray and line segment intersect, if they intersect at all</param>
    /// <returns></returns>
    public static bool RayLineSegmentIntersection2D(
        Vector2 rayOrigin,
        Vector2 rayDir,
        Vector2 segmentStart,
        Vector2 segmentEnd,
        ref Vector2 intersectionPoint)
    {
        Vector2 v1 = rayOrigin - segmentStart;
        Vector2 v2 = segmentEnd - segmentStart;
        Vector2 v3 = new Vector2(-rayDir.y, rayDir.x);

        float v2Dotv3 = Vector2.Dot(v2, v3);

        float t1 = CrossProduct(v2, v1)/ v2Dotv3;
        float t2 = Vector2.Dot(v1, v3)/ v2Dotv3;

        if (t1 < 0 || t2 > 1 || t2 < 0 || float.IsNaN(t2)) return false;

        intersectionPoint = segmentStart + t2*v2;
        return true;
    }

    public static float CrossProduct(Vector2 v1, Vector2 v2)
    {
        return (v1.x* v2.y) - (v1.y* v2.x);
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
