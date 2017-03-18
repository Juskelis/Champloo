using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ExtraLargeTextSize : MonoBehaviour
{
    private Text t;

    [SerializeField]
    private int fontSize;

    [SerializeField]
    private bool dynamic = true;

    void Awake()
    {
        t = GetComponent<Text>();
        if (!dynamic) t.fontSize = fontSize;
    }

    void Update()
    {
        if (dynamic) t.fontSize = fontSize;
    }
}
