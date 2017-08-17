using System;
using UnityEngine;

public class ProjectileHitParticles : MonoBehaviour {

    [SerializeField]
    GameObject effect;

    void Start()
    {
        EventDispatcher.Instance.AddListener<ProjectileHitEvent>(OnProjectileHit);
    }

    void OnProjectileHit(object sender, EventArgs args)
    {
        Projectile proj = (Projectile)sender;
        ProjectileHitEvent projectileHitEvent = (ProjectileHitEvent) args;
        if (projectileHitEvent.HitPlayer == null
            || proj.OurPlayer.PlayerNumber != projectileHitEvent.HitPlayer.PlayerNumber)
        {
            Instantiate(effect, proj.transform.position, proj.transform.rotation);
        }
    }
}
