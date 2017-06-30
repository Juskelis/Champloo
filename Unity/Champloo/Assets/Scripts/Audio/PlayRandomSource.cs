using UnityEngine;

public class PlayRandomSource : MonoBehaviour
{
    private AudioSource[] sources;
    private bool looping = false;

    private void Start()
    {
        sources = GetComponents<AudioSource>();
    }

    private void Update()
    {
        if (looping && !Playing)
        {
            Play();
        } 
    }

    public bool Playing { get { return PlayCount > 0; } }

    public int PlayCount
    {
        get
        {
            int ret = 0;
            foreach (AudioSource source in sources)
            {
                if (source.isPlaying) ret++;
            }
            return ret;
        }
    }

    /// <summary>
    /// Plays a random one of the audio sources once on this gameobject
    /// </summary>
    public void Play()
    {
        if (sources.Length <= 0 || (looping && Playing)) return;
        AudioSource toPlay = sources[Random.Range(0, sources.Length)];
        toPlay.loop = false;
        toPlay.Play();
    }

    public void PlayLooped()
    {
        Play();
        looping = true;
    }

    /// <summary>
    /// Stops all sounds on this gameobject
    /// </summary>
    public void Stop()
    {
        looping = false;
        foreach (AudioSource source in sources)
        {
            source.Stop();
        }
    }
}
