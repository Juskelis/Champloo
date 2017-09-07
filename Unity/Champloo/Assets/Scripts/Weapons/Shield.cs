using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour {

    [SerializeField]
    private int MaxHits = 3;

    [SerializeField]
    private float timeToLoseShield = 2f;
    [SerializeField]
    private float timeToGainShield = 2f;

    private float grow;

    [SerializeField]
    private SpriteRenderer visuals;

    private float spriteMaxScale;

    private float percentLeft = 1;

    private float percentPerHit;

    private bool canActivate = true;
    public bool CanActivate {  get { return canActivate; } }

    private bool up = false;
    public bool Up { get { return up; } }

    public void Start()
    {
        spriteMaxScale = visuals.transform.localScale.x;
    }

    public void ActivateShield()
    {
        if (!canActivate) return;

        percentPerHit = 1/((float)MaxHits);
        up = percentLeft > 0;
        grow = 1/timeToGainShield;

        canActivate = false;
    }

    public void DeactivateShield()
    {
        up = false;
    }

    private void Update()
    {
        if (!up)
        {
            percentLeft += grow * Time.deltaTime;
        }
        else if (percentLeft <= 0)
        {
            up = false;
        }
        
        if(!canActivate && percentLeft >= 1f)
        {
            canActivate = true;
        }

        percentLeft = Mathf.Clamp01(percentLeft);

        visuals.transform.localScale = Vector3.one*spriteMaxScale*percentLeft;
        visuals.enabled = up;
    }

    public bool TakeHit(Weapon w)
    {
        if (!Up) return false;
        if (w != null && w.IsSpecialAttacking && w is Axe) return false;

        percentLeft -= percentPerHit;
        return percentLeft >= 0;
    }
}
