using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class CustomEvents : MonoBehaviour {

    public KeyCode toPress;

    public UnityEvent toExecute;
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(toPress)) toExecute.Invoke();
	}
}
