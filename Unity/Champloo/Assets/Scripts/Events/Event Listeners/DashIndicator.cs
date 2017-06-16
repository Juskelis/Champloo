using System;
using System.Collections.Generic;
using UnityEngine;

public class DashIndicator : MonoBehaviour
{
    [SerializeField]
    private Transform singleIndicator;

    [SerializeField]
    private float spacing = 1f;

    private OnDash dashState;

    private List<Transform> indicators;
    
    void Start()
    {
        GetComponentInParent<LocalEventDispatcher>().AddListener<MovementStateChangedEvent>(OnChange);
        dashState = GetComponentInParent<OnDash>();
        if (dashState == null)
        {
            Debug.LogError("DashIndicator could not find OnDash component");
        }

        //create all indicators
        indicators = new List<Transform>();
        for (int i = 0; i < dashState.DashLimit; i++)
        {
            Transform toAdd = Instantiate(singleIndicator);
            Vector3 pos = toAdd.position;
            pos.x = ((dashState.DashLimit*spacing)/2f)*(i-Mathf.Floor(dashState.DashLimit/2f));
            toAdd.position = pos;
            toAdd.SetParent(transform, false);
            indicators.Add(toAdd);
        }
    }

    void OnDestroy()
    {
        LocalEventDispatcher dispatcher = GetComponentInParent<LocalEventDispatcher>();
        if (dispatcher != null)
        {
            dispatcher.RemoveListener<MovementStateChangedEvent>(OnChange);
        }
    }

    private void OnChange(object sender, EventArgs args)
    {
        for (var i = 0; i < indicators.Count; i++)
        {
            indicators[i].gameObject.SetActive(i < dashState.DashesRemaining);
        }
    }
}
