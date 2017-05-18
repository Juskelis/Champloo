using UnityEngine;
using UnityStandardAssets.Utility;

[RequireComponent(typeof(Renderer))]
public class GrassObject : MonoBehaviour
{
    private Vector3 target;
    
    private Renderer r;
    public void Awake()
    {
        r = GetComponent<Renderer>();
    }

    public void SetObstacle(Vector3 v)
    {
        r.material.SetVector("_WorldPosObstacle", v);
        target = v;
    }
}
