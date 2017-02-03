using UnityEngine;
using System.Collections;

public class GetExternalIP : MonoBehaviour
{
    private bool _ready = false;
    public bool Ready { get { return _ready; } }

    [SerializeField]
    private string externalIP_Site = "https://icanhazip.com/";

    private string _IP = null;

    public delegate void Callback(string IP);

    private Callback ongoingCallback = null;

    public void RequestIP(Callback onComplete)
    {
        if (_IP != null) onComplete(_IP);
        if (ongoingCallback != null) ongoingCallback += onComplete;
        else StartCoroutine(GetIP(onComplete));
    }

    private IEnumerator GetIP(Callback onComplete)
    {
        ongoingCallback = onComplete;
        WWW request = new WWW(externalIP_Site);
        yield return request;
        _IP = request.text;
        _ready = true;
        ongoingCallback(_IP);
        ongoingCallback = null;
    }
}
