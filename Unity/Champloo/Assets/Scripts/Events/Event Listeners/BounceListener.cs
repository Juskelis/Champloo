﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceListener : MonoBehaviour
{
    [SerializeField]
    private Transform toSpawn;

	// Use this for initialization
	void Start () {
		EventDispatcher.Instance.AddListener<BounceEvent>(OnBounce);
	}

    private void OnBounce(object sender, EventArgs args)
    {
        BounceEvent e = (BounceEvent) args;

        Vector3 pos = (e.A.transform.position + e.B.transform.position)/2;
        Vector3 dir = (e.A.transform.position - e.B.transform.position).normalized;
        Instantiate(toSpawn, pos, Quaternion.AngleAxis(Utility.Vector2AsAngle(dir), transform.forward));
    }
}