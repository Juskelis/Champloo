using System;
using UnityEngine;
using System.Collections;
using GamepadInput;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerSelect : MonoBehaviour
{
    private Rewired.Player player;

    public bool Working { get { return currentStage > 0; } }

    public bool Finished { get { return currentStage == stages.Length - 1; } }

    [Range(1,4)]
    public int PlayerIndex;

    [Serializable]
    private class Stage
    {
        public string name;

        public GameObject activeObject;

        public UnityEvent onEnter;
        
        public UnityEvent onExit;
    }
    /*
    [SerializeField]
    private GamePad.Button nextButton;

    [SerializeField]
    private GamePad.Button previousButton;
    */
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
        player = Rewired.ReInput.players.GetPlayer(PlayerIndex - 1);
        //set the playernumber for all of the children
        foreach (Carousel c in GetComponentsInChildren<Carousel>())
        {
            c.playerNumber = PlayerIndex;
            c.p = player;
        }
        foreach (NameCarousel c in GetComponentsInChildren<NameCarousel>())
        {
            c.playerNumber = PlayerIndex;
            c.p = player;
        }

        foreach (Stage s in stages)
        {
            s.activeObject.SetActive(false);
        }
        ourGroup = GetComponentInParent<CanvasGroup>();
        stages[currentStage].activeObject.SetActive(true);
        stages[currentStage].onEnter.Invoke();
    }

    private void Update()
    {
        if (!ourGroup.interactable) return;

        //GamePad.Index playerIndex = InputController.ConvertToIndex(PlayerIndex);
        Rewired.Player p = Rewired.ReInput.players.GetPlayer(PlayerIndex - 1);

        int next = 0;
        //if (GamePad.GetButtonDown(nextButton, playerIndex))
        if (p.GetButtonDown("Accept"))
            next = 1;
        //else if (GamePad.GetButtonDown(previousButton, playerIndex))
        else if (p.GetButtonDown("Back"))
            next = -1;

        if(next != 0)
        {
            stages[currentStage].activeObject.SetActive(false);
            stages[currentStage].onExit.Invoke();
            currentStage = Mathf.Clamp(currentStage + next, 0, stages.Length - 1);
            stages[currentStage].activeObject.SetActive(true);
            stages[currentStage].onEnter.Invoke();
        }
        /*
        if (GamePad.GetButtonDown(nextButton, playerIndex))
        {
            stages[currentStage].activeObject.SetActive(false);
            currentStage = Mathf.Min(currentStage + 1, stages.Length - 2);
            stages[currentStage].activeObject.SetActive(true);
            stages[currentStage].onEnter.Invoke();
        }

        if (GamePad.GetButtonDown(previousButton, playerIndex))
        {
            stages[currentStage].activeObject.SetActive(false);
            stages[currentStage].onExit.Invoke();
            currentStage = Mathf.Max(currentStage - 1, 0);
            stages[currentStage].activeObject.SetActive(true);
        }
        */
    }

    public void SavePlayerSelection()
    {
        /*
        PlayerSettings settings = FindObjectOfType<PlayerSettings>();

        settings.SetPlayerName(PlayerIndex, playerName.text);
        
        if(playerWeapon != null)
            settings.SetWeapon(PlayerIndex, "Prefabs/Weapon/" + playerWeapon.text);
        else
            settings.SetWeapon(PlayerIndex, "Prefabs/Weapon/Sword");

        if (playerShield != null)
            settings.SetShield(PlayerIndex, "Prefabs/Shield/" + playerShield.text);
        else
            settings.SetShield(PlayerIndex, "Prefabs/Shield/Shield");

        if(playerPrefab != null)
            settings.SetPlayerPrefab(PlayerIndex, "Prefabs/Player/" + playerPrefab.text);
        else
            settings.SetPlayerPrefab(PlayerIndex, "Prefabs/Player/Player");
            */
    }
}
