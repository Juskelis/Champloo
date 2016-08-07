using System;
using UnityEngine;
using System.Collections;
using GamepadInput;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerSelect : MonoBehaviour {

    public bool Working { get { return currentStage > 0; } }

    public bool Finished { get { return currentStage == stages.Length - 1; } }

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
    private GamePad.Button nextButton;

    [SerializeField]
    private GamePad.Button previousButton;

    [SerializeField]
    private GamePad.Button readyUpButton;

    [SerializeField]
    private Stage[] stages;

    [SerializeField]
    private Text playerName;

    [SerializeField]
    private Text playerPrefab;

    [SerializeField]
    private Text playerWeapon;

    [SerializeField]
    private Text playerShield;

    private int currentStage = 0;

    private CanvasGroup ourGroup;

    private void Start()
    {
        ourGroup = GetComponentInParent<CanvasGroup>();
        stages[currentStage].onEnter.Invoke();
    }

    private void Update()
    {
        if (!ourGroup.interactable) return;

        GamePad.Index playerIndex = InputController.ConvertToIndex(PlayerIndex);

        if (GamePad.GetButtonDown(nextButton, playerIndex))
        {
            currentStage = Mathf.Min(currentStage + 1, stages.Length - 2);
            stages[currentStage].onEnter.Invoke();
        }

        if (GamePad.GetButtonDown(previousButton, playerIndex))
        {
            int previousStage = currentStage;
            currentStage = Mathf.Max(currentStage - 1, 0);
            stages[previousStage].onExit.Invoke();
        }

        if (GamePad.GetButtonDown(readyUpButton, playerIndex))
        {
            int previousStage = currentStage;
            currentStage = stages.Length - 1;
            stages[currentStage].onEnter.Invoke();
        }
    }

    public void SavePlayerSelection()
    {
        PlayerSettings settings = FindObjectOfType<PlayerSettings>();
        settings.SetPlayerName(PlayerIndex, playerName.text);

        settings.SetWeapon(PlayerIndex, "Prefabs/Weapon/Sword");
        settings.SetShield(PlayerIndex, "Prefabs/Shield/Shield");
        /*
        settings.SetWeapon(PlayerIndex, playerWeapon.text);
        settings.SetShield(PlayerIndex, playerShield.text);
        */

        settings.SetPlayerPrefab(PlayerIndex, "Prefabs/Player/Player");
    }
}
