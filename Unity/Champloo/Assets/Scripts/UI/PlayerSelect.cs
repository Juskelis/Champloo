using System;
using UnityEngine;
using System.Collections;
using GamepadInput;
using UnityEngine.Events;

public class PlayerSelect : MonoBehaviour {

    [Range(1,4)]
    public int PlayerIndex;

    [Serializable]
    private class Stage
    {
        public string name;
        
        public UnityEvent onEnter;
        
        public UnityEvent onExit;
    }

    [SerializeField]
    private GamePad.Button next;

    [SerializeField]
    private GamePad.Button previous;

    [SerializeField]
    private Stage[] stages;

    private int currentStage = 0;

    private CanvasGroup ourGroup;

    private void Start()
    {
        ourGroup = GetComponentInParent<CanvasGroup>();
    }

    private void Update()
    {
        if (!ourGroup.interactable) return;

        GamePad.Index playerIndex = InputController.ConvertToIndex(PlayerIndex);

        int previousStage = currentStage;
        if (GamePad.GetButtonDown(next, playerIndex))
        {
            currentStage = Mathf.Min(currentStage + 1, stages.Length - 1);
            stages[currentStage].onEnter.Invoke();
        }

        if (GamePad.GetButtonDown(previous, playerIndex))
        {
            currentStage = Mathf.Max(currentStage - 1, 0);
            stages[previousStage].onExit.Invoke();
        }
    }
}
