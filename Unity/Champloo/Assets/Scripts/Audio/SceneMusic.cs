using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SceneMusic : MonoBehaviour
{
    AudioSource source;

    private float startLevel;
    private float endLevel;
    private float startTime;
    private float fadeTime;
    
    public float defaultFadeTime = 1f;

    private bool fading = false;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (fading)
        {
            source.volume = Mathf.Lerp(startLevel, endLevel, (Time.time - startTime)/fadeTime);
        }
    }

    public void FadeTo(float level, float time)
    {
        startLevel = source.volume;
        endLevel = level;
        startTime = Time.time;
        fadeTime = Mathf.Max(time, Time.fixedDeltaTime);
        fading = true;
    }

    public void FadeTo(float level)
    {
        startLevel = source.volume;
        endLevel = level;
        startTime = Time.time;
        fadeTime = defaultFadeTime;
        fading = true;
    }

    public void SetTo(float level)
    {
        source.volume = level;
        fading = false;
    }
}
