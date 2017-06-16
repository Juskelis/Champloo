using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class TextSetter : MonoBehaviour
{
    Text t;

    void Awake()
    {
        t = GetComponent<Text>();
    }

    public void SetText(string text)
    {
        t.text = text;
    }
}
