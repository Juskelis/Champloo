using UnityEngine;
using System.Collections.Generic;

public class SmashCamera : MonoBehaviour
{
    private Camera cam;

    private Transform[] toFollow;

    [SerializeField]
    private Transform bottomLeft;
    [SerializeField]
    private Transform topRight;

    private Vector3 size;

    private Vector3 zoomOut;
    private Vector3 zoomIn;

    [SerializeField]
    private float panSpeed = 0.6f;

    [SerializeField]
    private float zoomOutSpeed = 0.6f;
    [SerializeField]
    private float zoomInSpeed = 0.8f;

    [SerializeField]
    private float zoomOutBoundary = 0.6f;

    [SerializeField]
    private float zoomInBoundary = 0.4f;

    [SerializeField]
    private float zoomMin = 0.1f;
    [SerializeField]
    private float zoomMax = 10f;

    [SerializeField]
    private AnimationCurve zoomCurve;

    private Vector3 center;
    private Vector3 maxDist;

    private List<Transform> targetTransforms;
    private List<float> medianCollection;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = GetComponentInChildren<Camera>();
        bottomLeft.SetParent(null, true);
        topRight.SetParent(null, true);
    }

    void Start()
    {
        size = topRight.position - bottomLeft.position;

        zoomMax = Mathf.Max(zoomMax, size.x, size.y*cam.aspect);

        zoomIn = zoomInBoundary * size;
        zoomOut = zoomOutBoundary * size;

        center = Vector3.zero;

        targetTransforms = new List<Transform>();
        medianCollection = new List<float>();
    }

    void LateUpdate()
    {
        CameraTarget[] targets = FindObjectsOfType<CameraTarget>();
        targetTransforms.Clear();
        foreach(CameraTarget target in targets)
        {
            targetTransforms.Add(target.transform);
        }
        toFollow = targetTransforms.ToArray();

        Vector2 newCenter = Vector2.zero;

        maxDist = Vector2.zero;

        for (int i = 0; i < toFollow.Length; i++)
        {
            newCenter.x += toFollow[i].position.x;
            newCenter.y += toFollow[i].position.y;
        }
        newCenter = newCenter / (toFollow.Length > 0 ? toFollow.Length : 1);
        for (int i = 0; i < toFollow.Length; i++)
        {
            maxDist.x = Mathf.Max(maxDist.x, Mathf.Abs(toFollow[i].position.x - newCenter.x));
            maxDist.y = Mathf.Max(maxDist.y, Mathf.Abs(toFollow[i].position.y - newCenter.y));
        }

        if (maxDist.x >= zoomOut.x || maxDist.y >= zoomOut.y)
        {
            //zoom out
            if (maxDist.x >= zoomOut.x/2)
            {
                size.x = Median(
                    zoomMin,
                    zoomMax,
                    (Mathf.Pow(zoomOutSpeed, 3) * (maxDist.x - zoomOut.x/2))/2 + size.x
                );
                size.y = size.x/cam.aspect;
            }
            else if (maxDist.y >= zoomOut.y/2)
            {
                size.y = Median(
                    zoomMin/cam.aspect,
                    zoomMax/cam.aspect,
                    (Mathf.Pow(zoomOutSpeed, 3) * (maxDist.y - zoomOut.y/2))/2 + size.y
                );
                size.x = size.y*cam.aspect;
            }
        }
        else if (maxDist.x <= zoomIn.x && maxDist.y <= zoomIn.y)
        {
            if (maxDist.x/(zoomIn.x/2) >= maxDist.y/(zoomIn.y/2))
            {
                size.x = Median(
                    zoomMin,
                    zoomMax,
                    (Mathf.Pow(zoomInSpeed, 3) * (maxDist.x - zoomIn.x/2))/2 + size.x
                );
                size.y = size.x/cam.aspect;
            }
            else
            {
                size.y = Median(
                    zoomMin/cam.aspect,
                    zoomMax/cam.aspect,
                    (Mathf.Pow(zoomInSpeed, 3) * (maxDist.y - zoomIn.y/2))/2 + size.y
                );
                size.x = size.y*cam.aspect;
            }
        }

        zoomOut = zoomOutBoundary*size;
        zoomIn = zoomInBoundary*size;

        //center = newCenter;
        center.x = cubic_lerp(center.x, newCenter.x, panSpeed);
        center.y = cubic_lerp(center.y, newCenter.y, panSpeed);
        center.z = transform.position.z;

        //clamp position to within bounds
        center.x = Mathf.Clamp(center.x, bottomLeft.position.x + size.x / 2, topRight.position.x - size.x / 2);
        center.y = Mathf.Clamp(center.y, bottomLeft.position.y + size.y / 2, topRight.position.y - size.y / 2);


        //zoom camera to bounds
        transform.position = center;

        Vector3 camBottomLeft = cam.ScreenToWorldPoint(Vector3.zero);
        Vector3 camTopRight = cam.ScreenToWorldPoint(new Vector3(cam.pixelWidth, cam.pixelHeight, 0));
        Vector3 camSize = camBottomLeft - camTopRight;

        cam.orthographicSize *= size.magnitude/camSize.magnitude;
    }

    // Update is called once per frame
    void OnDrawGizmosSelected () {
	    Gizmos.color = Color.white;
        Gizmos.DrawWireCube(center, size);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center, zoomOut);

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(center, zoomIn);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube((bottomLeft.position + topRight.position)/2f, (topRight.position - bottomLeft.position));
    }

    public float Median(params float[] values)
    {
        int middleIndex = values.Length/2;
        medianCollection.Clear();
        medianCollection.AddRange(values);
        medianCollection.Sort();
        return values.Length%2 != 0 ?
            medianCollection[middleIndex] :
            (medianCollection[middleIndex] + medianCollection[middleIndex - 1])/2f;
    }

    public float cubic_lerp(float start, float end, float speed)
    {
        return speed*speed*speed*(end - start)/2 + start;
    }
}
