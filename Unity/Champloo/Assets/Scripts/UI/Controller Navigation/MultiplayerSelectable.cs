using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Events;

[System.Serializable]
public class MultiplayerSelectableEvent : UnityEvent<MultiplayerSelectable, MultiplayerUIController> {}

//[RequireComponent(typeof(Selectable))]
public class MultiplayerSelectable : Selectable
{
    [Tooltip("Where player indicators are held (ie Horizontal/Vertical Layout Groups)")]
    public LayoutGroup playerContainerPrefab;

    public bool addToNeighbors = false;

    //public Action OnClick;
    public MultiplayerSelectableEvent OnClick;

    private LayoutGroup playerContainer;
    
    public int ControllersSelecting()
    {
        int ret = 0;
        foreach (var controller in playerContainer.GetComponentsInChildren<MultiplayerUIController>())
        {
            if (controller.hasSelected) ret++;
        }
        return ret;
    }

    public bool HasControllerSelected(MultiplayerUIController controller)
    {
        if (!controller.hasSelected) return false;

        foreach (var child in playerContainer.GetComponentsInChildren<MultiplayerUIController>())
        {
            if (child == controller) return true;
        }
        return false;
    }

    protected override void Start()
    {
        base.Start();

        playerContainer = GetComponentInChildren<LayoutGroup>();
        if (playerContainer == null)
        {
            //print("Error: MultiplayerSelectable needs a child LayoutGroup");
            Debug.LogError("MultiplayerSelectable needs a child LayoutGroup");
        }


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

    protected override void OnDestroy()
    {
        base.OnDestroy();
        DestroyImmediate(playerContainer);
    }

    public void AddPlayerIndicator(Transform r)
    {
        int sortR = r.GetComponent<MultiplayerUIController>().ControllerNumber;

        int sortIndex;
        MultiplayerUIController[] children = transform.GetComponentsInChildren<MultiplayerUIController>();
        for(sortIndex = 0; sortIndex < children.Length && sortR > children[sortIndex].ControllerNumber; sortIndex++)
        { }

        r.SetParent(playerContainer.transform, false);
        r.SetSiblingIndex(sortIndex);
    }

    public void OnClickCallback(MultiplayerSelectable selectable, MultiplayerUIController controller)
    {
        print("Clicked with " + controller.ControllerNumber);
    }
}
