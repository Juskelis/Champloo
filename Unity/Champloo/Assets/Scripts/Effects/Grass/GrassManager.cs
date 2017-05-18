using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Utility;

public class GrassManager : MonoBehaviour
{
    private static GrassManager _instance;

    private Player[] players;

    void Start()
    {
        if (_instance == null || _instance == this)
        {
            _instance = this;
            players = FindObjectsOfType<Player>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void LateUpdate()
    {
        foreach (GrassObject g in FindObjectsOfType<GrassObject>())
        {
            Vector3 pos = Vector3.one * Mathf.Infinity;
            float min = Mathf.Infinity;
            foreach (Player v in players)
            {
                if (v.Dead) continue;

                float dist = (v.transform.position - transform.position).sqrMagnitude;
                if (dist < min)
                {
                    pos = v.transform.position;
                    min = dist;
                }
            }
            g.SetObstacle(pos);
        }
    }
}
