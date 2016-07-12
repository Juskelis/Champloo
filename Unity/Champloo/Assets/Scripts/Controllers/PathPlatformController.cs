using UnityEngine;
using System.Collections;

public class PathPlatformController : PlatformController
{
    [SerializeField] private float speed;
    [SerializeField] private float waitTime;
    [SerializeField][Range(0,2)] private float easeAmount;
    [SerializeField] private bool cyclic;
    [SerializeField] private Vector3[] localWaypoints;
    private Vector3[] globalWaypoints;
    private int fromWaypointIndex;
    private float percentBetweenWaypoints;
    private float nextWaitTime;

    protected override void Start()
    {
        base.Start();

        globalWaypoints = new Vector3[localWaypoints.Length];
        for(int i = 0; i < localWaypoints.Length; i++)
        {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
    }

    float Ease(float x)
    {
        float a = easeAmount + 1;
        return Mathf.Pow(x, a)/(Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

    protected override Vector3 CalculatePlatformMovement()
    {
        if(Time.time < nextWaitTime) return Vector3.zero;

        fromWaypointIndex %= globalWaypoints.Length;
        int toWaypointIndex = (fromWaypointIndex + 1)%globalWaypoints.Length;
        float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex],
            globalWaypoints[toWaypointIndex]);

        percentBetweenWaypoints += Time.deltaTime*speed/distanceBetweenWaypoints;
        percentBetweenWaypoints = Mathf.Clamp01(percentBetweenWaypoints);
        float easedPercentBetweenWaypoints = Ease(percentBetweenWaypoints);

        Vector3 newPosition = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex],
            easedPercentBetweenWaypoints);

        if (percentBetweenWaypoints >= 1)
        {
            percentBetweenWaypoints = 0;
            fromWaypointIndex++;
            if (!cyclic && fromWaypointIndex >= globalWaypoints.Length - 1)
            {
                fromWaypointIndex = 0;
                System.Array.Reverse(globalWaypoints);
            }

            nextWaitTime = Time.time + waitTime;
        }

        return newPosition - transform.position;
    }

    void OnDrawGizmos()
    {
        if (localWaypoints != null)
        {
            Gizmos.color = Color.red;
            float size = 0.1f;

            for (int i = 0; i < localWaypoints.Length; i++)
            {
                Vector3 globalWaypointPosition = Application.isPlaying?globalWaypoints[i]:localWaypoints[i] + transform.position;
                //Vector3 globalWaypointPosition = transform.TransformVector(localWaypoints[i]);

                Gizmos.DrawSphere(globalWaypointPosition, size);
            }
        }
    }
}
