using UnityEngine;

public class PlayRandomSource : MonoBehaviour
{
    private AudioSource[] sources;

    private void Start()
    {
        sources = GetComponents<AudioSource>();
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
        if (sources.Length <= 0) return;
        AudioSource toPlay = sources[Random.Range(0, sources.Length)];
        toPlay.loop = false;
        toPlay.Play();
    }

    /// <summary>
    /// Stops all sounds on this gameobject
    /// </summary>
    public void Stop()
    {
        foreach (AudioSource source in sources)
        {
            source.Stop();
        }
    }
}
