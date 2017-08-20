using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Net;
using UnityEngine.Networking;

public class PlayerScoreCard : MonoBehaviour
{
    private Score scoreService;

    private Text playerNameText;

    [SerializeField]
    private Transform indicatorParent;

    [SerializeField]
    private Sprite emptyScoreIndicator;

    [SerializeField]
    private Sprite filledScoreIndicator;

    [SerializeField]
    private Text scoreText;

    [SerializeField]
    private Image background;

    public int playerNumber = 0;

    private List<Image> scores = new List<Image>();

    private int maxScore = 7;
    private int score = 0;
    private int previousScore = 0;

	// Use this for initialization
	void Start ()
	{
	    scoreService = Score.instance;//FindObjectOfType<Score>();
	    maxScore = scoreService.WinScore;
        playerNameText = GetComponentInChildren<Text>();


        PlayerSettings settings = PlayerSettings.GetSettingsFor(playerNumber);
        playerNameText.text = settings != null ? settings.Name : "Name";

	    if (settings != null)
        {
            Color backgroundColor = settings.Color;
            backgroundColor.a = background.color.a;
            background.color = backgroundColor;
	    }

	    /*
        for (int i = 0; i < maxScore; i++)
	    {
	        GameObject g = new GameObject(i.ToString());
	        Image im = g.AddComponent<Image>();
	        im.sprite = emptyScoreIndicator;
	        if(settings != null) im.color = settings.Color;
	        im.preserveAspect = true;
	        g.transform.SetParent(indicatorParent, false);
            scores.Add(im);
        }
        */
	}
	
	// Update is called once per frame
	void Update ()
	{
        if(!Score.Scores.ContainsKey(playerNumber))
        {
            return;
        }
	    score = Score.Scores[playerNumber];
	    scoreText.text = score + "/" + Score.instance.WinScore;
	}
}
