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

    private bool firstPass = true;

    void Start()
    {

        Vector3 parentScale = transform.parent.localScale;

        //make render texture that fits the object
        //  note: this only allocates a block of memory for the texture; it does NOT clear that memory first.
        //  clearing must be done manually
        tex = new RenderTexture(Mathf.CeilToInt(parentScale.x*scaleFactor), Mathf.CeilToInt(parentScale.y*scaleFactor), 0)
        {
            filterMode = textureFilterMode,
            anisoLevel = 0,
            depth = (int) Depth
        };

        //get camera
        Camera cam = GetComponent<Camera>();
        cam.orthographicSize = parentScale.y/2;

        cam.targetTexture = tex;

        GetComponentInChildren<MeshRenderer>().material.mainTexture = tex;
    }

    void Update()
    {
        //clears the render texture we created
        if (firstPass)
        {
            Graphics.SetRenderTarget(tex);
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
