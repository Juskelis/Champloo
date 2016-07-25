using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GamepadInput;

public class Carousel : MonoBehaviour {
    [Range(1,4)]
    public int playerNumber;

    [SerializeField]
    private GamePad.Axis navigate;

    [SerializeField]
    private GamePad.Button next;

    [SerializeField]
    private GamePad.Button previous;

    [SerializeField]
    private string[] valuesOnCarousel;

    [SerializeField]
    private Text renderTo;

    int currentIndex = 0;

    public void Update()
    {
        GamePad.Index playerIndex = InputController.ConvertToIndex(playerNumber);

        float axis = GamePad.GetAxis(navigate, playerIndex).x;

        if (GamePad.GetButtonDown(next, playerIndex))
        {
            currentIndex = (currentIndex + 1) % (valuesOnCarousel.Length);
        }

        if (GamePad.GetButtonDown(previous, playerIndex))
        {
            currentIndex--;
            if (currentIndex < 0) currentIndex = valuesOnCarousel.Length + currentIndex;
        }


        renderTo.text = valuesOnCarousel[currentIndex];
    }
}
