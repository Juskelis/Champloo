using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEditor;

public class SmashCamera : MonoBehaviour
{
    private Camera cam;
    private Renderer ren;

    [SerializeField]
    private Transform[] toFollow;

    [SerializeField]
    private Transform bottomLeft;
    [SerializeField]
    private Transform topRight;

    private Vector3 screenSpaceTopRight;

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

    void Start()
    {
        cam = GetComponent<Camera>();
        ren = GetComponent<Renderer>();

        screenSpaceTopRight = new Vector3(cam.pixelWidth, cam.pixelHeight, 0);

        bottomLeft.position = cam.ScreenToWorldPoint(Vector3.zero);
        topRight.position = cam.ScreenToWorldPoint(screenSpaceTopRight);

        size = topRight.position - bottomLeft.position;

        zoomIn = zoomInBoundary * size;
        zoomOut = zoomOutBoundary * size;

        center = Vector3.zero;

        for (int i = 0; i < toFollow.Length; i++)
        {
            center.x += toFollow[i].position.x;
            center.y += toFollow[i].position.y;
        }
        center = center / (toFollow.Length > 0 ? toFollow.Length : 1);
    }

    void LateUpdate()
    {
        Vector2 newCenter = Vector2.zero;

        maxDist = Vector2.zero;

        for (int i = 0; i < toFollow.Length; i++)
        {
            newCenter.x += toFollow[i].position.x;
            newCenter.y += toFollow[i].position.y;

            maxDist.x = Mathf.Max(maxDist.x, Mathf.Abs(toFollow[i].position.x - center.x));
            maxDist.y = Mathf.Max(maxDist.y, Mathf.Abs(toFollow[i].position.y - center.y));
        }
        newCenter = newCenter / (toFollow.Length > 0 ? toFollow.Length : 1);

        if (maxDist.x > zoomOut.x / 2 && maxDist.y > zoomOut.y / 2)
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
            if (maxDist.y >= zoomOut.y/2)
            {
                size.y = Median(
                    zoomMin/cam.aspect,
                    zoomMax/cam.aspect,
                    (Mathf.Pow(zoomOutSpeed, 3) * (maxDist.y - zoomOut.y/2))/2 + size.y
                );
                size.x = size.y*cam.aspect;
            }
        }
        else if (maxDist.x < zoomIn.x && maxDist.y < zoomIn.y)
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


        //zoom camera to bounds
        transform.position = center;

        bottomLeft.position = cam.ScreenToWorldPoint(Vector3.zero);
        topRight.position = cam.ScreenToWorldPoint(screenSpaceTopRight);
        Vector3 camSize = bottomLeft.position - topRight.position;

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
    }

    public float Median(params float[] values)
    {
        int half = values.Length / 2;
        var sorted = values.OrderBy(n => n);
        if (values.Length % 2 == 0)
        {
            return (sorted.ElementAt(half) + sorted.ElementAt(half - 1)) / 2f;
        }
        else
        {
            return sorted.ElementAt(half);
        }
    }

    public float cubic_lerp(float start, float end, float speed)
    {
        return speed*speed*speed*(end - start)/2 + start;
    }
}
