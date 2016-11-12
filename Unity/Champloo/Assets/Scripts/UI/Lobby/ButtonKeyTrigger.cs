using UnityEngine;
using Rewired;
using System.Collections;
using UnityEngine.UI;

public class ButtonKeyTrigger : MonoBehaviour
{
    public string actionName;
    [Tooltip("presents as 'Press [actionName button] to [description]")]
    public string description;
    private Button us;

	// Use this for initialization
	void Start ()
	{
	    us = GetComponent<Button>();
	    string actionNameButton =
	        Rewired.ReInput.players.GetPlayer(0).controllers.maps.GetFirstElementMapWithAction(actionName, true).elementIdentifierName;
	    GetComponentInChildren<Text>().text = "Press " + actionNameButton + " to " + description;
	}
	
	// Update is called once per frame
	void Update () {
	    if (Rewired.ReInput.players.GetPlayer(0).GetButtonDown(actionName))
	    {
	        us.onClick.Invoke();
	    }
	}
}
