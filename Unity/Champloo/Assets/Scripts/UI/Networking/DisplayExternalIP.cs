using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DisplayExternalIP : MonoBehaviour {
    Text t;

    [SerializeField]
    GetExternalIP requester;

    void Awake()
    {
        t = GetComponent<Text>();
    }

    void Start()
    {
        requester.RequestIP(s => { t.text = s; });
    }
}
