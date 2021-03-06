﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitListener : MonoBehaviour
{
    [SerializeField]
    private Transform toSpawn;

	// Use this for initialization
	void Start () {
		EventDispatcher.Instance.AddListener<HitEvent>(OnHit);
	}

    private void OnHit(object sender, EventArgs args)
    {
        HitEvent hit = (HitEvent) args;

        Vector3 dirToAttacker = (hit.Attacker.CenterOfSprite - hit.Hit.CenterOfSprite).normalized;
        Instantiate(toSpawn, hit.Hit.CenterOfSprite,
            Quaternion.AngleAxis(Utility.Vector2AsAngle(dirToAttacker), transform.forward));
    }
}
