using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashIndicator : MonoBehaviour
{
    [SerializeField]
    private Image singleIndicator;

    [SerializeField]
    private Sprite availableSprite;

    [SerializeField]
    private Sprite unavailableSprite;

    [SerializeField]
    private Transform container;

    private OnDash dashState;

    private List<Image> indicators;

    private int playerNumber;
    
    void Start()
    {
        if (!container) container = transform;

        playerNumber = GetComponentInParent<PlayerScoreCard>().playerNumber;

        EventDispatcher.Instance.AddListener<MovementStateChangedEvent>(OnChange);

        foreach (Player player in FindObjectsOfType<Player>())
        {
            if (player.PlayerNumber == playerNumber)
            {
                dashState = player.GetComponentInChildren<OnDash>();
            }
        }
        if (dashState == null)
        {
            Debug.LogError("DashIndicator could not find OnDash component");
        }

        //create all indicators
        indicators = new List<Image>();
        for (int i = 0; i < dashState.DashLimit; i++)
        {
            Image toAdd = Instantiate(singleIndicator);
            toAdd.transform.SetParent(container, false);
            indicators.Add(toAdd);
        }
    }

    private void OnChange(object sender, EventArgs args)
    {
        for (var i = 0; i < indicators.Count; i++)
        {
            indicators[i].sprite = (i < dashState.DashesRemaining) ? availableSprite : unavailableSprite;
        }
    }
}
