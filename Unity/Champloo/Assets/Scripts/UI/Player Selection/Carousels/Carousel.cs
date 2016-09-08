using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GamepadInput;

public class Carousel : MonoBehaviour {

    public Object CurrentSelection
    {
        get { return currentIndex < valuesOnCarousel.Length ? valuesOnCarousel[currentIndex] : null; }
    }

    [HideInInspector]
    public Rewired.Player p;
    
    [HideInInspector]
    public int playerNumber;
    /*
    [SerializeField]
    protected GamePad.Axis navigate;
    [SerializeField]
    protected float maxAxisTime = 0.1f;

    [SerializeField]
    protected GamePad.Button next;

    [SerializeField]
    protected GamePad.Button previous;
    */
    [SerializeField]
    protected Object[] valuesOnCarousel;

    [SerializeField]
    protected Text nameDisplay;

    private int currentIndex = 0;

    private float timer = 0;

    public void Start()
    {
        UpdateInformation();
    }

    public void Update()
    {
        //GamePad.Index playerIndex = InputController.ConvertToIndex(playerNumber);

        //float axis = GamePad.GetAxis(navigate, playerIndex).x;
        float axis = p.GetAxis("Navigate Horizontal");
        if (timer > 0) axis = 0;

        //if (GamePad.GetButtonDown(next, playerIndex) || axis > 0)
        if (p.GetButtonDown("Accept") || axis > 0)
        {
            currentIndex = (currentIndex + 1) % (valuesOnCarousel.Length);
        }

        //if (GamePad.GetButtonDown(previous, playerIndex) || axis < 0)
        if (p.GetButtonDown("Back") || axis < 0)
        {
            currentIndex--;
            if (currentIndex < 0) currentIndex = valuesOnCarousel.Length + currentIndex;
        }
        /*
        if (axis != 0)
        {
            timer = maxAxisTime;
        }

        timer -= Time.deltaTime;
        */
        UpdateInformation();
    }

    void UpdateInformation()
    {
        if (nameDisplay != null && CurrentSelection != null)
        {
            nameDisplay.text = CurrentSelection.name;
        }
    }
}
