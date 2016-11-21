using UnityEngine;
using System.Collections;
using UnityEngine.UI;


//[RequireComponent(typeof(Selectable))]
public class MultiplayerSelectable : Selectable
{
    [Tooltip("Where player indicators are held (ie Horizontal/Vertical Layout Groups)")]
    public LayoutGroup playerContainerPrefab;

    public bool addToNeighbors = false;

    private LayoutGroup playerContainer;

    void Start()
    protected override void Start()
    {
        playerContainer = Instantiate(playerContainerPrefab);
        playerContainer.transform.SetParent(transform, false);
        base.Start();

        if (addToNeighbors)
        {
            //Selectable us = GetComponent<Selectable>();
            if (FindSelectableOnDown().GetComponent<MultiplayerSelectable>() == null)
            {
                Utility.CopyComponent(this, FindSelectableOnDown().gameObject);
            }
            if (FindSelectableOnUp().GetComponent<MultiplayerSelectable>() == null)
            {
                Utility.CopyComponent(this, FindSelectableOnUp().gameObject);
            }
            if (FindSelectableOnLeft().GetComponent<MultiplayerSelectable>() == null)
            {
                Utility.CopyComponent(this, FindSelectableOnLeft().gameObject);
            }
            if (FindSelectableOnRight().GetComponent<MultiplayerSelectable>() == null)
            {
                Utility.CopyComponent(this, FindSelectableOnRight().gameObject);
            }
        }
    }

    public void AddPlayerIndicator(Transform r)
    {
        int sortR = r.GetComponent<MultiplayerUIController>().ControllerNumber;

        int sortIndex;
        MultiplayerUIController[] children = transform.GetComponentsInChildren<MultiplayerUIController>();
        for(sortIndex = 0; sortIndex < children.Length && sortR < children[sortIndex].ControllerNumber; sortIndex++)
        { }

        r.SetParent(playerContainer.transform, false);
        r.SetSiblingIndex(sortIndex);
    }
}
