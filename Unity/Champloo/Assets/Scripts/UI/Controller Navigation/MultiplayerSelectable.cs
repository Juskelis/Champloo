using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class MultiplayerSelectable : MonoBehaviour
{
    [Tooltip("Where player indicators are held (ie Horizontal/Vertical Layout Groups)")]
    public LayoutGroup playerContainerPrefab;

    private LayoutGroup playerContainer;

    void Start()
    {
        playerContainer = Instantiate(playerContainerPrefab);
        playerContainer.transform.SetParent(transform, false);
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
