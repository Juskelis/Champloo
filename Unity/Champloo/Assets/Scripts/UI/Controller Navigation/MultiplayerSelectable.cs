using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class MultiplayerSelectable : MonoBehaviour
{
    [Tooltip("Where player indicators are held (ie Horizontal/Vertical Layout Groups)")]
    public LayoutGroup playerContainerPrefab;

    public bool addToNeighbors = false;

    private LayoutGroup playerContainer;

    void Start()
    {
        playerContainer = Instantiate(playerContainerPrefab);
        playerContainer.transform.SetParent(transform, false);

        if (addToNeighbors)
        {
            Selectable us = GetComponent<Selectable>();
            if (us.FindSelectableOnDown().GetComponent<MultiplayerSelectable>() == null)
            {
                Utility.CopyComponent(this, us.FindSelectableOnDown().gameObject);
            }
            if (us.FindSelectableOnUp().GetComponent<MultiplayerSelectable>() == null)
            {
                Utility.CopyComponent(this, us.FindSelectableOnUp().gameObject);
            }
            if (us.FindSelectableOnLeft().GetComponent<MultiplayerSelectable>() == null)
            {
                Utility.CopyComponent(this, us.FindSelectableOnLeft().gameObject);
            }
            if (us.FindSelectableOnRight().GetComponent<MultiplayerSelectable>() == null)
            {
                Utility.CopyComponent(this, us.FindSelectableOnRight().gameObject);
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
