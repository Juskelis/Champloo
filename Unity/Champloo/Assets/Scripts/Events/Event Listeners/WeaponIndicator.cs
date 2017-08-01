using System;
using UnityEngine;

public class WeaponIndicator : MonoBehaviour
{
    [SerializeField] private bool inverseIndicator = false;

    private int playerNumber;
    public int PlayerNumber
    {
        get
        {
            return playerNumber; 
        }
        set
        {
            if (!isOnPlayer)
            {
                playerNumber = value;
            }
        }
    }
    public bool isOnPlayer = false;

    void Awake()
    {
        if (isOnPlayer)
        {
            playerNumber = GetComponentInParent<Player>().PlayerNumber;
        }
    }
    
    void Start()
    {
        if (isOnPlayer)
        {
            GetComponentInParent<LocalEventDispatcher>().AddListener<WeaponInHandEvent>(OnWeaponHand);
        }
        else
        {
            EventDispatcher.Instance.AddListener<WeaponInHandEvent>(OnWeaponHand);
        }

        if (inverseIndicator)
        {
            gameObject.SetActive(false);
        }
    }

    void OnWeaponHand(object sender, EventArgs args)
    {
        WeaponInHandEvent e = (WeaponInHandEvent)args;

        if (isOnPlayer || e.Target.PlayerNumber == PlayerNumber)
        {
            gameObject.SetActive(e.Target.InHand != inverseIndicator);
        }
    }
}
