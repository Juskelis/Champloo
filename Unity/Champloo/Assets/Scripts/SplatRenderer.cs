using UnityEngine;
using System.Collections;

public class SplatRenderer : MonoBehaviour {
    [SerializeField]
    private FilterMode textureFilterMode = FilterMode.Point;

    private enum Levels { Zero = 0, Sixteen = 16, TwentyFour = 24 }
    [SerializeField]
    private Levels Depth = Levels.TwentyFour;

    private RenderTexture tex;

    private Camera cam;

    private MeshRenderer child;

    void Start()
    {
        child = GetComponentInChildren<MeshRenderer>();

        Vector3 parentScale = transform.parent.localScale;

        //make render texture that fits the object
        if (parentScale.x < parentScale.y)
            tex = new RenderTexture(Mathf.CeilToInt(parentScale.x) * 50, Mathf.CeilToInt(parentScale.y) * 100, 0);
        else
            tex = new RenderTexture(Mathf.CeilToInt(parentScale.x) * 100, Mathf.CeilToInt(parentScale.y) * 50, 0);

        //tex.Create();

        tex.DiscardContents();

        tex.filterMode = FilterMode.Point;
        tex.anisoLevel = 0;

        tex.depth = (int)Depth;

        //get camera
        cam = GetComponent<Camera>();
        cam.orthographicSize = Mathf.Min(parentScale.x, parentScale.y) / 2;

        cam.targetTexture = tex;

        child.material.mainTexture = tex;

        tex.Create();

        tex.DiscardContents(true, true);
    }
}
