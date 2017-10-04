using UnityEngine;
using System.Collections;
using Prototype.NetworkLobby;
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

    public MultiplayerSelectableEvent OnHover;

    private LayoutGroup playerContainer;
    
    /// <summary>
    /// Get the number of players selecting this controller
    ///     takes into account networked players as well
    /// </summary>
    /// <returns></returns>
    public int ControllersSelecting()
    {
        int ret = 0;
        foreach (var controller in playerContainer.GetComponentsInChildren<MultiplayerUIController>())
        {
            if (controller.hasSelected) ret++;
        }

        if (LobbyManager.s_Singleton != null)
        {
            //potentially have network players who have voted for this
            LobbyPlayer player;
            foreach (var netPlayer in LobbyManager.s_Singleton.lobbySlots)
            {
                player = (LobbyPlayer) netPlayer;
                if (player && player.selectedSelectable == ToString())
                {
                    ret++;
                }
            }
        }

        return ret;
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
        
        if (playerContainer.transform.GetComponent<GridLayoutGroup>() != null)
        {
            r.SetParent(playerContainer.transform.GetChild(sortR), false);
        }
        else
        {
            r.SetParent(playerContainer.transform, false);
            //r.SetSiblingIndex(sortIndex);
        }

        OnHover.Invoke(this, r.GetComponent<MultiplayerUIController>());
    }

    public void OnClickCallback(MultiplayerSelectable selectable, MultiplayerUIController controller)
    {
        print("Clicked with " + controller.ControllerNumber);
    }

    private string HeirarchyPath()
    {
        Transform t = transform;
        string path = t.name;
        while (t.parent != null)
        {
            t = t.parent;
            path = t.name + "." + path;
        }
        return path;
    }

    public override string ToString()
    {
        return "MultiplayerSelectable_" + gameObject.tag + "_" + HeirarchyPath();
    }
}
