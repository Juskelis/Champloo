using System;
using UnityEngine;
using System.Collections;
using Rewired;
using Rewired.Integration.UnityUI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MultiplayerUIController : MonoBehaviour
{
    private Rewired.Player controller;

    private EventSystem eventSystem;
    private RewiredStandaloneInputModule rewiredEventSystem;

    private MultiplayerSelectable currentlySelected;

    [SerializeField]
    private int controllerNumber = 0;

    public int ControllerNumber
    {
        get { return controllerNumber; }
        set { controllerNumber = value; }
    }

    private Vector2 prev_move = Vector2.zero;
    private int move_count = 0;

	// Use this for initialization
	void Start ()
	{
	    eventSystem = FindObjectOfType<EventSystem>();
	    rewiredEventSystem = FindObjectOfType<RewiredStandaloneInputModule>();
	    controller = Rewired.ReInput.players.GetPlayer(controllerNumber);

	    GetComponentInChildren<Text>().text = (controllerNumber+1).ToString();
	}

    void Update()
    {
        if (currentlySelected == null) return;

        
        if (controller.GetButtonDown(rewiredEventSystem.submitButton))
        {
            /*
            Button b = currentlySelected.GetComponent<Button>();
            if (b != null)
            {
                b.onClick.Invoke();
            }
            */
            currentlySelected.OnClick.Invoke(currentlySelected, this);
        }
        
        Selectable next = null;

        Vector2 raw = GetInput();
        if (Mathf.Approximately(raw.x, 0) && Mathf.Approximately(raw.y, 0))
        {
            move_count = 0;
            return;
        }

        if (raw.x > 0)
        {
            next = currentlySelected.FindSelectableOnRight();
        }
        if (raw.x < 0)
        {
            next = currentlySelected.FindSelectableOnLeft();
        }

        if (raw.y > 0)
        {
            next = currentlySelected.FindSelectableOnUp();
        }
        if (raw.y < 0)
        {
            next = currentlySelected.FindSelectableOnDown();
        }

        if (next != null) ChangeSelected(next.GetComponent<MultiplayerSelectable>());

        if (Vector2.Dot(raw, prev_move) <= 0)
        {
            move_count = 0;
        }
        move_count++;

        prev_move = raw;
    }

    private Vector2 GetInput()
    {
        Vector2 ret = Vector2.zero;
        if (controller.GetButtonDown(rewiredEventSystem.horizontalAxis))
        {
            ret.x = 1;
        }
        else if (controller.GetNegativeButtonDown(rewiredEventSystem.horizontalAxis))
        {
            ret.x = -1;
        }

        if (controller.GetButtonDown(rewiredEventSystem.verticalAxis))
        {
            ret.y = 1;
        }
        else if (controller.GetNegativeButtonDown(rewiredEventSystem.verticalAxis))
        {
            ret.y = -1;
        }

        return ret;
    }

    public void ChangeSelected(MultiplayerSelectable next)
    {
        if (next == null || next == currentlySelected) return;

        //if(currentlySelected != null) currentlySelected.OnDeselect(new BaseEventData(eventSystem));
        currentlySelected = next;
        //currentlySelected.OnSelect(new BaseEventData(eventSystem));

        /*
        if (indicator == null)
        {
            indicator = Instantiate(indicatorImage).rectTransform;
            indicator.GetComponentInChildren<Text>().text = (controllerNumber+1).ToString();
        }
        indicator.SetParent(currentlySelected.transform, false);
        indicator.SetAsFirstSibling();
        */
        /*
        transform.SetParent(currentlySelected.transform, false);
        transform.SetAsFirstSibling();
        */

        next.AddPlayerIndicator(transform);
    }
}
