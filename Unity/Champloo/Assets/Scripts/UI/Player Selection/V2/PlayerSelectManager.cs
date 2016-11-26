using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerSelectManager : MonoBehaviour {

    void Start()
    {
    }

    public void PlayerSelected(MultiplayerSelectable selectable, MultiplayerUIController controller)
    {
        selectable.GetComponent<PlayerOption>();
    }
}
