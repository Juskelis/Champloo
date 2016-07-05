using UnityEngine;
using UnityEngine.Events;

public class Timer : MonoBehaviour {
    [SerializeField]
    private float levelSeconds = 90f;

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
    }

    void Timeout()
    {
        OnTimeout.Invoke();
    }
}
