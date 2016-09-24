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

	    for (int i = 0; i < maxScore; i++)
	    {
	        GameObject g = new GameObject(i.ToString());
	        Image im = g.AddComponent<Image>();
	        im.sprite = emptyScoreIndicator;
	        im.preserveAspect = true;
	        g.transform.SetParent(indicatorParent, false);
            scores.Add(im);
        }


        PlayerSettings settings = PlayerSettings.GetSettingsFor(playerNumber);
	    playerNameText.text = settings != null ? settings.Name : "Name";

	    /*
	    PlayerSettings settings = FindObjectOfType<PlayerSettings>();
        if(settings == null) print("settings is null!");
	    string playerName = settings.GetPlayerName(playerNumber + 1);
	    playerNameText.text = playerName;
        */
	    //playerNameText.text = FindObjectOfType<PlayerSettings>().GetPlayerName(playerNumber + 1);
	}
	
	// Update is called once per frame
	void Update ()
	{
	    score = Score.Scores[playerNumber];

	    for (int i = 0; i < maxScore; i++)
	    {
	        scores[i].sprite = i < score ? filledScoreIndicator : emptyScoreIndicator;
	    }
	}
}
