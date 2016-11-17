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
        r.SetParent(playerContainer.transform, false);

        //ordering
    }
}
