﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GamepadInput;

public class NameCarousel : MonoBehaviour
{
    [HideInInspector]
    public int playerNumber;

    [HideInInspector]
    public Rewired.Player p;
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
    private string[] valuesOnCarousel;

    [SerializeField]
    private Text renderTo;

    private int currentIndex = 0;

    private float timer = 0;

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
        renderTo.text = valuesOnCarousel[currentIndex];
    }
}
