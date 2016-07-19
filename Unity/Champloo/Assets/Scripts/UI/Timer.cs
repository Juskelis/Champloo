using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Timer : MonoBehaviour {
    [SerializeField]
    private float levelSeconds = 90f;

    [SerializeField] private Text displayText;

    private float timer;
    public float TimeLeft { get { return timer; } }

    private bool timerDone = false;
    public bool TimerDone { get { return timerDone; } }

    public UnityEvent OnTimeout;

    void Start()
    {
        timerDone = false;
        timer = levelSeconds;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if(!timerDone && timer <= 0f)
        {
            Timeout();
            timerDone = true;
        }

        if (displayText != null)
        {
            displayText.text = timer > 0f ? ((int)timer).ToString(CultureInfo.InvariantCulture) : "--";
        }
    }

    void Timeout()
    {
        OnTimeout.Invoke();
    }
}
