using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using System.Collections.Generic;

public class SplatRenderer : MonoBehaviour
{
    [SerializeField]
    private FilterMode textureFilterMode = FilterMode.Point;

    private enum Levels { Zero = 0, Sixteen = 16, TwentyFour = 24 }
    [SerializeField]
    private Levels Depth = Levels.TwentyFour;

    [SerializeField]
    private int scaleFactor = 100;

    private RenderTexture tex;

    private Camera cam;

    private MeshRenderer child;

    private bool firstPass = true;

    void Start()
    {

        child = GetComponentInChildren<MeshRenderer>();

        Vector3 parentScale = transform.parent.localScale;

        //make render texture that fits the object
        if(parentScale.x < parentScale.y)
            tex = new RenderTexture(Mathf.CeilToInt(parentScale.x) * scaleFactor, Mathf.CeilToInt(parentScale.y) * scaleFactor, 0);
        else
            tex = new RenderTexture(Mathf.CeilToInt(parentScale.x) * scaleFactor, Mathf.CeilToInt(parentScale.y) * scaleFactor/2, 0);

        //tex.Create();

        tex.filterMode = FilterMode.Point;
        tex.anisoLevel = 0;

        tex.depth = (int)Depth;

        //tex.Create();

        //get camera
        cam = GetComponent<Camera>();
        cam.orthographicSize = parentScale.y/2;

        cam.targetTexture = tex;

        //cam.clearFlags = CameraClearFlags.Depth;

        child.material.mainTexture = tex;
    }

    void OnPreRender()
    {
        if (firstPass)
        {
            Color clearColor = new Color(0, 0, 0, 0);
            GL.Clear(true, true, clearColor);
            firstPass = false;
        }
    }

    void Destroy()
    {
        tex.Release();
    }
}
