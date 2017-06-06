using System.Globalization;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Timer : NetworkBehaviour {
    [SerializeField]
    private float levelSeconds = 90f;

    [SerializeField] private Text displayText;

    [SyncVar]
    private float timer;
    public float TimeLeft { get { return timer; } }

    private bool timerDone = false;
    public bool TimerDone { get { return timerDone; } }

    public UnityEvent OnTimeout;

    private bool running = true;

    void Start()
    {
        timerDone = false;
        timer = levelSeconds;
    }

    void Update()
    {
        if(running) timer -= Time.deltaTime;
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

    public void SetRunning(bool running)
    {
        this.running = running;
    }
}
