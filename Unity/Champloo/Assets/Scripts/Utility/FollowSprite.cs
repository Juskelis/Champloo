using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowSprite : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer toFollow;

    [SerializeField]
    private Vector3 offset;

	// Update is called once per frame
	void LateUpdate ()
	{
	    transform.position = toFollow.bounds.center + offset;
	}
}
