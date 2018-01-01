using System;
using UnityEngine;

public class WeaponSpecialListener : MonoBehaviour {
    
    [SerializeField]
    private Transform buildupEffect;

    [SerializeField]
    private Transform fireEffect;

    private Weapon weapon;
    private Color playerColor;

    void Awake()
    {
        GetComponentInParent<LocalEventDispatcher>().AddListener<WeaponSpecialTimingEvent>(OnSpecial);
        GetComponentInParent<LocalEventDispatcher>().AddListener<DeathEvent>(OnDeath);
        weapon = GetComponentInChildren<Weapon>();
    }

    void Start()
    {
        playerColor = GetComponentInParent<Player>().PlayerColor;
    }

    private void OnDeath(object sender, EventArgs args)
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            Transform temp = transform.GetChild(i);
            if (temp.name.Contains(buildupEffect.name) || temp.name.Contains(fireEffect.name))
            {
                Destroy(temp.gameObject);
            }
        }

        for (int i = weapon.transform.childCount - 1; i >= 0; i--)
        {
            Transform temp = weapon.transform.GetChild(i);
            if (temp.name.Contains(buildupEffect.name) || temp.name.Contains(fireEffect.name))
            {
                Destroy(temp.gameObject);
            }
        }
    }

    private void OnSpecial(object sender, EventArgs args)
    {
        WeaponSpecialTimingEvent e = (WeaponSpecialTimingEvent)args;
        if (e.Timing == TimingState.WARMUP)
        {
            Transform t = Instantiate(buildupEffect, transform.position, weapon.transform.rotation);
            t.SetParent(weapon.transform, true);
            SetColor(t);
        }
        else if (e.Timing == TimingState.IN_PROGRESS)
        {
            Transform t = Instantiate(fireEffect, transform.position, weapon.transform.rotation);
            t.SetParent(transform, true);
            SetColor(t);
        }
    }

    private void SetColor(Transform t)
    {
        ParticleSystem[] systems = t.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem system in systems)
        {
            ParticleSystem.MainModule main = system.main;
            ParticleSystem.MinMaxGradient startColor = main.startColor;
            GradientColorKey[] colorKeys;
            switch (system.main.startColor.mode)
            {
                case ParticleSystemGradientMode.Color:
                    startColor = playerColor;
                    break;
                case ParticleSystemGradientMode.Gradient:
                    colorKeys = startColor.gradient.colorKeys;
                    for (var i = 0; i < colorKeys.Length; i++)
                    {
                        colorKeys[i].color *= playerColor;
                    }
                    startColor.gradient.SetKeys(colorKeys, startColor.gradient.alphaKeys);
                    break;
                case ParticleSystemGradientMode.TwoColors:
                    startColor.colorMax *= playerColor;
                    startColor.colorMin *= playerColor;
                    break;
                case ParticleSystemGradientMode.TwoGradients:
                    colorKeys = startColor.gradientMax.colorKeys;
                    for (var i = 0; i < colorKeys.Length; i++)
                    {
                        colorKeys[i].color *= playerColor;
                    }
                    startColor.gradientMax.SetKeys(colorKeys, startColor.gradientMax.alphaKeys);
                    colorKeys = startColor.gradientMin.colorKeys;
                    for (var i = 0; i < colorKeys.Length; i++)
                    {
                        colorKeys[i].color *= playerColor;
                    }
                    startColor.gradientMin.SetKeys(colorKeys, startColor.gradientMax.alphaKeys);
                    break;
            }
            main.startColor = startColor;
        }
    }
}
