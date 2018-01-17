using System;
using System.Collections.Generic;
using UnityEngine;

// Nine slice scaling using a Mesh.
// Original code by Asher Vollmer
//      https://twitter.com/AsherVo
//      http://ashervollmer.tumblr.com
// Modifications by Thomas Viktil
//      https://twitter.com/mandarinx
//      http://ma.ndar.in/

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[ExecuteInEditMode]
public class NineSlice : MonoBehaviour
{
    [SerializeField] private Sprite useSprite;
    private Vector3 scale;
    [SerializeField] private Vector2 borderWidthInPixels;
    [SerializeField] private Vector2 worldSpaceBorderWidth;

    private Vector3 oldScale;
    private Sprite oldSprite;
    private Vector2 oldBorderWidthInPixels;
    private Vector2 oldWorldBorderWidth;

    private MeshFilter filter;
    private Renderer renderer;
    private Mesh mesh;

    public void LateUpdate()
    {
        scale = transform.localScale;
        if (oldSprite == useSprite
            && oldBorderWidthInPixels == borderWidthInPixels
            && oldScale == scale
            && oldWorldBorderWidth == worldSpaceBorderWidth)
        {
            return;
        }

        if (!GetComponents())
        {
            Debug.Log("Unable to find filter/renderer");
            return;
        }

        Debug.Log("Found filter and renderer");

        CreateWorldSpaceMesh();

        oldSprite = useSprite;
        oldBorderWidthInPixels = borderWidthInPixels;
        oldScale = scale;
        oldWorldBorderWidth = worldSpaceBorderWidth;
    }

    private bool GetComponents()
    {
        filter = filter ?? gameObject.GetComponent<MeshFilter>();
        renderer = renderer ?? gameObject.GetComponent<Renderer>();
        return (filter != null) && (renderer != null);
    }

    private void CreateWorldSpaceMesh()
    {
        if (!filter || !renderer)
        {
            Debug.LogError("Lost reference to filter/renderer");
            return;
        }

        if (filter.sharedMesh != null)
        {
            DestroyImmediate(filter.sharedMesh);
        }
        if (mesh != null)
        {
            DestroyImmediate(mesh);
        }
        if (useSprite == null)
        {
            return;
        }

        mesh = new Mesh();
        
        List<Vector3> vertices = new List<Vector3>();
        List<Color> colors = new List<Color>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();
        List<int> tris = new List<int>();

        float pixelSize = 1f/useSprite.pixelsPerUnit;
        Vector2 objectSpaceBorderWidth = new Vector2(
            worldSpaceBorderWidth.x / transform.localScale.x,
            worldSpaceBorderWidth.y / transform.localScale.y);
        Vector2 uvBorderWidth = pixelSize*borderWidthInPixels;

        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                Color vertexColor = Color.black;
                Vector3 minPosition = Vector3.zero;
                Vector3 maxPosition = Vector3.zero;
                Vector3 minUV = Vector3.zero;
                Vector3 maxUV = Vector3.zero;
                switch (x)
                {
                    case 0:
                        minPosition.x = -0.5f;
                        maxPosition.x = minPosition.x + objectSpaceBorderWidth.x;
                        minUV.x = 0;
                        maxUV.x = uvBorderWidth.x;
                        break;
                    case 1:
                        vertexColor.r = 1;
                        minPosition.x = -0.5f + objectSpaceBorderWidth.x;
                        maxPosition.x = 0.5f - objectSpaceBorderWidth.x;
                        minUV.x = uvBorderWidth.x;
                        maxUV.x = 1 - uvBorderWidth.x;
                        break;
                    case 2:
                        minPosition.x = 0.5f - objectSpaceBorderWidth.x;
                        maxPosition.x = 0.5f;
                        minUV.x = 1 - uvBorderWidth.x;
                        maxUV.x = 1;
                        break;
                }
                switch (y)
                {
                    case 0:
                        minPosition.y = -0.5f;
                        maxPosition.y = minPosition.y + objectSpaceBorderWidth.y;
                        minUV.y = 0;
                        maxUV.y = uvBorderWidth.y;
                        break;
                    case 1:
                        vertexColor.g = 1;
                        minPosition.y = -0.5f + objectSpaceBorderWidth.y;
                        maxPosition.y = 0.5f - objectSpaceBorderWidth.y;
                        minUV.y = uvBorderWidth.y;
                        maxUV.y = 1 - uvBorderWidth.y;
                        break;
                    case 2:
                        minPosition.y = 0.5f - objectSpaceBorderWidth.y;
                        maxPosition.y = 0.5f;
                        minUV.y = 1 - uvBorderWidth.y;
                        maxUV.y = 1;
                        break;
                }

                vertices.Add(minPosition);
                vertices.Add(new Vector3(minPosition.x, maxPosition.y));
                vertices.Add(maxPosition);
                vertices.Add(new Vector3(maxPosition.x, minPosition.y));

                uvs.Add(minUV);
                uvs.Add(new Vector2(minUV.x, maxUV.y));
                uvs.Add(maxUV);
                uvs.Add(new Vector2(maxUV.x, minUV.y));

                normals.Add(Vector3.forward);
                normals.Add(Vector3.forward);
                normals.Add(Vector3.forward);
                normals.Add(Vector3.forward);

                colors.Add(vertexColor);
                colors.Add(vertexColor);
                colors.Add(vertexColor);
                colors.Add(vertexColor);

                tris.Add(vertices.Count - 4);
                tris.Add(vertices.Count - 3);
                tris.Add(vertices.Count - 2);
                tris.Add(vertices.Count - 2);
                tris.Add(vertices.Count - 1);
                tris.Add(vertices.Count - 4);
            }
        }


        MaterialPropertyBlock pBlock = new MaterialPropertyBlock();
        pBlock.SetTexture("_MainTex", useSprite.texture);
        pBlock.SetVector("_MinRepeatUV", uvBorderWidth);
        pBlock.SetVector("_RepeatRangeUV", Vector2.one - uvBorderWidth*2);
        renderer.SetPropertyBlock(pBlock);

        mesh.MarkDynamic();
        mesh.vertices = vertices.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.normals = normals.ToArray();
        mesh.RecalculateBounds();
        mesh.colors = colors.ToArray();

        filter.sharedMesh = mesh;
    }

    private void CreateMesh()
    {
        if (!filter || !renderer)
        {
            Debug.LogError("Lost reference to filter/renderer");
            return;
        }

        if (filter.sharedMesh != null)
        {
            DestroyImmediate(filter.sharedMesh);
        }
        if (mesh != null)
        {
            DestroyImmediate(mesh);
        }
        if (useSprite == null)
        {
            return;
        }

        MaterialPropertyBlock pBlock = new MaterialPropertyBlock();
        pBlock.SetTexture("_MainTex", useSprite.texture);
        renderer.SetPropertyBlock(pBlock);

        mesh = new Mesh();

        Vector3[] vertices = new Vector3[16];
        Color[] colors = new Color[16];
        Vector3[] normals = new Vector3[16];
        Vector2[] uvs = new Vector2[16];
        int[] tris = new int[9 * 2 * 3]; // (number of quads) * (triangles per quad) * (vertices per triangle)

        float pixelSize = 1f/useSprite.pixelsPerUnit;

        Func<int, int, int> getIndexFromPosition = (x, y) => y*4 + x;

        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                int index = getIndexFromPosition(x,y);

                Vector2 position = new Vector2(Mathf.Sign(x - 2), Mathf.Sign(y - 2))*0.5f;
                Vector2 uvPosition = position*2;
                colors[index] = Color.black;

                switch (x)
                {
                    case 0:
                        position.x = -0.5f;
                        uvPosition.x = 0;
                        break;
                    case 1:
                        position.x = -0.5f + worldSpaceBorderWidth.x / transform.localScale.x;
                        uvPosition.x = pixelSize * borderWidthInPixels.x;
                        colors[index].r = 1f;
                        break;
                    case 2:
                        position.x = 0.5f - worldSpaceBorderWidth.x / transform.localScale.x;
                        uvPosition.x = 1 - pixelSize * borderWidthInPixels.x;
                        colors[index].r = 1f;
                        break;
                    case 3:
                        position.x = 0.5f;
                        uvPosition.x = 1;
                        break;
                }
                switch (y)
                {
                    case 0:
                        position.y = -0.5f;
                        uvPosition.y = 0;
                        break;
                    case 1:
                        position.y = -0.5f + worldSpaceBorderWidth.y / transform.localScale.y;
                        uvPosition.y = pixelSize * borderWidthInPixels.y;
                        colors[index].g = 1f;
                        break;
                    case 2:
                        position.y = 0.5f - worldSpaceBorderWidth.y / transform.localScale.y;
                        uvPosition.y = 1 - pixelSize * borderWidthInPixels.y;
                        colors[index].g = 1f;
                        break;
                    case 3:
                        position.y = 0.5f;
                        uvPosition.y = 1;
                        break;
                }

                vertices[index] = position;
                uvs[index] = uvPosition;
                normals[index] = -Vector3.forward;
            }
        }

        int triangleIndex = 0;
        //loop through pairs of triangles (aka segments)
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                //triangle one
                tris[triangleIndex] = getIndexFromPosition(x, y);
                triangleIndex++;
                tris[triangleIndex] = getIndexFromPosition(x+1, y+1);
                triangleIndex++;
                tris[triangleIndex] = getIndexFromPosition(x+1, y);
                triangleIndex++;
                //triangle two
                tris[triangleIndex] = getIndexFromPosition(x, y);
                triangleIndex++;
                tris[triangleIndex] = getIndexFromPosition(x, y+1);
                triangleIndex++;
                tris[triangleIndex] = getIndexFromPosition(x+1, y+1);
                triangleIndex++;
            }
        }

        mesh.MarkDynamic();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.normals = normals;
        mesh.RecalculateBounds();

        filter.sharedMesh = mesh;
    }


    /*
    public Sprite useSprite;
    public float width;
    public float height;
    public int slicePixels = 5;

    private float oldWidth;
    private float oldHeight;
    private Sprite oldSprite;
    private MeshFilter meshF;
    private Renderer rend;
    private Mesh mesh;
    private Vector3 position;

    public void LateUpdate()
    {
        if (useSprite == oldSprite &&
            width == oldWidth &&
            height == oldHeight)
        {
            return;
        }

        CreateMesh();

        oldSprite = useSprite;
        oldWidth = width;
        oldHeight = height;
    }

    public void CreateMesh()
    {
        if (meshF == null)
        {
            meshF = GetComponent<MeshFilter>() as MeshFilter;
        }
        if (rend == null)
        {
            rend = GetComponent<Renderer>() as Renderer;
        }
        if (meshF.sharedMesh)
        {
            DestroyImmediate(meshF.sharedMesh);
        }
        if (mesh)
        {
            DestroyImmediate(mesh);
        }

        if (useSprite == null)
        {
            return;
        }

        MaterialPropertyBlock pBlock = new MaterialPropertyBlock();
        pBlock.SetTexture("_MainTex", useSprite.texture);
        rend.SetPropertyBlock(pBlock);

        float ppu = useSprite.pixelsPerUnit;
        float cornerWidth = (float)(slicePixels) / ppu;
        float cornerHeight = (float)(slicePixels) / ppu;

        mesh = new Mesh();

        Vector3[] vertices = new Vector3[16];
        Vector3[] normals = new Vector3[16];
        Vector2[] uvs = new Vector2[16];

        float[] xPValues = new float[4];
        float[] yPValues = new float[4];

        float cornerWidthP = cornerWidth / width;
        float cornerHeightP = cornerHeight / height;

        xPValues[0] = 0.0f;
        yPValues[0] = 0.0f;
        xPValues[1] = cornerWidthP;
        yPValues[1] = cornerHeightP;
        xPValues[2] = 1.0f - cornerWidthP;
        yPValues[2] = 1.0f - cornerHeightP;
        xPValues[3] = 1.0f;
        yPValues[3] = 1.0f;

        for (int x = 0; x < 4; x++)
        {
            for (int y = 0; y < 4; y++)
            {
                float xP = xPValues[x];
                float yP = yPValues[y];

                int index = OneDee(x, y);
                vertices[index] = new Vector3(
                    Mathf.Lerp(-1.0f, 1.0f, xP) * width * 0.5f,
                    Mathf.Lerp(-1.0f, 1.0f, yP) * height * 0.5f,
                    0.0f);
                uvs[index] = new Vector2((float)x / 3.0f, (float)y / 3.0f);
                normals[index] = -Vector3.forward;
            }
        }

        int[] tris = new int[9 * 2 * 3];
        int i = 0;
        for (int x = 0; x < 3; x++)
        {
            for (int y = 0; y < 3; y++)
            {
                tris[i] = OneDee(x + 1, y);
                i++;
                tris[i] = OneDee(x, y);
                i++;
                tris[i] = OneDee(x + 1, y + 1);
                i++;
                tris[i] = OneDee(x, y);
                i++;
                tris[i] = OneDee(x, y + 1);
                i++;
                tris[i] = OneDee(x + 1, y + 1);
                i++;
            }
        }

        mesh.MarkDynamic();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.normals = normals;
        mesh.RecalculateBounds();

        meshF.sharedMesh = mesh;
    }

    public static int OneDee(Vector2 coords)
    {
        return OneDee((int)coords.x, (int)coords.y);
    }

    public static int OneDee(int x, int y)
    {
        return (y * 4) + x;
    }

    public static Vector2 TwoDee(int index)
    {
        int x = index % 4;
        int y = index / 4;
        return new Vector2(x, y);
    }

    public void SetPosition(float x, float y, float z)
    {
        position.x = x;
        position.y = y;
        position.z = z;
        transform.position = position;
    }

    public void SetSize(float w, float h)
    {
        width = w;
        height = h;
    }
    */
}