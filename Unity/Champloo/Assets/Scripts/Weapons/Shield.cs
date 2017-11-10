using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shield : MonoBehaviour {

    [SerializeField]
    private int MaxHits = 3;
    
    [SerializeField]
    private float timeToGainShield = 2f;

    [SerializeField]
    private SpriteRenderer visuals;

    [SerializeField]
    private bool canBlink = false;

    [SerializeField]
    [Range(0.0001f, float.MaxValue)]
    [Delayed]
    private float blinkTime = 1f;

    public bool CanActivate { get; private set; }

    public bool Up { get; private set; }

    private int hitsLeft;
    private float rechargeStartTime = 0f;
    private float rechargeStepTime = 0f;

    private float spriteMaxScale;
    private float maxAlpha;

    private List<SpriteRenderer> shieldLevels;

    private void Start()
    {
        CanActivate = true;
        hitsLeft = MaxHits;
        rechargeStepTime = timeToGainShield/MaxHits;

        spriteMaxScale = visuals.transform.localScale.x;
        
        maxAlpha = visuals.color.a;
        Color color = GetComponentInParent<Player>().PlayerColor;
        color.a = maxAlpha;

        shieldLevels = new List<SpriteRenderer>();
        SpriteRenderer parent = visuals;
        shieldLevels.Add(parent);
        parent.color = color;
        for (int i = 1; i < MaxHits; i++)
        {
            SpriteRenderer spawned = Instantiate(parent, parent.transform);
            spawned.transform.localScale = Vector3.one * (MaxHits - i)/MaxHits;
            spawned.color = color;
            shieldLevels.Add(spawned);
            parent = spawned;
        }
        shieldLevels.Reverse();

    }

    public void Reset()
    {
        Up = false;
        hitsLeft = MaxHits;
    }

    private void Update()
    {
        if (!Up)
        {
            bool couldRechargeThisFrame = hitsLeft < MaxHits;

            if (Time.time - rechargeStartTime >= rechargeStepTime)
            {
                hitsLeft = Mathf.Min(hitsLeft + 1, MaxHits);
                rechargeStartTime = Time.time;
            }

            if (hitsLeft == MaxHits)
            {
                Color color = GetComponentInParent<Player>().PlayerColor;
                color.a = maxAlpha;
                visuals.color = color;
                if (couldRechargeThisFrame)
                {
                    EventDispatcher.Instance.FireEvent(this, new ShieldRechargeEvent { OurShield = this });
                    GetComponentInParent<LocalEventDispatcher>().FireEvent(this, new ShieldRechargeEvent { OurShield = this });
                }
                CanActivate = true;
            }
        }
        else if(hitsLeft == 1 && canBlink)
        {
            //blink
            float percent = (Time.time%blinkTime)/blinkTime;
            Color c = visuals.color;
            c.a = maxAlpha*(1 - percent);
            visuals.color = c;
        }

        for (int i = 0; i < MaxHits; i++)
        {
            shieldLevels[i].enabled = i < hitsLeft && Up;
        }
    }

    public void ActivateShield()
    {
        if (!CanActivate) return;

        CanActivate = false;
        Up = true;
    }

    public void DeactivateShield()
    {
        Up = false;
        rechargeStartTime = Time.time;
    }

    public bool TakeHit(Weapon w)
    {
        if (!Up) return false;
        if (w != null && w.IsSpecialAttacking && w is Axe) return false;

        hitsLeft--;
        if (hitsLeft == 0)
        {
            DeactivateShield();
            EventDispatcher.Instance.FireEvent(this, new ShieldBreakEvent {OurShield = this});
            GetComponentInParent<LocalEventDispatcher>().FireEvent(this, new ShieldBreakEvent {OurShield = this});
        }
        return hitsLeft >= 0;
    }
}
