using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {

    [SerializeField]
    private int MaxHits = 3;
    
    [SerializeField]
    private float timeToGainShield = 2f;

    [SerializeField]
    private SpriteRenderer visuals;

    public bool CanActivate { get; private set; }

    public bool Up { get; private set; }

    private int hitsLeft;
    private float rechargeStartTime = 0f;
    private float rechargeStepTime = 0f;

    private float spriteMaxScale;

    private void Start()
    {
        hitsLeft = MaxHits;
        rechargeStepTime = timeToGainShield/MaxHits;

        spriteMaxScale = visuals.transform.localScale.x;
    }

    private void Update()
    {
        if (!Up)
        {
            if (Time.time - rechargeStartTime >= rechargeStepTime)
            {
                hitsLeft = Mathf.Min(hitsLeft + 1, MaxHits);
                rechargeStartTime = Time.time;
            }

            if (hitsLeft == MaxHits)
            {
                CanActivate = true;
            }
        }

        float percentLeft = ((float)hitsLeft)/MaxHits;

        visuals.transform.localScale = Vector3.one * spriteMaxScale * percentLeft;
        visuals.enabled = Up;
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
        return hitsLeft >= 0;
    }
}
