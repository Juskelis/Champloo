using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class DynamicFont : MonoBehaviour {

    private Dictionary<Text, int> originalFontSizes;

    private float originalWidth;
    private float newWidth = -1;

	// Use this for initialization
	void Awake () {
        originalWidth = Screen.width;
        originalFontSizes = new Dictionary<Text, int>();
	}
	
	// Update is called once per frame
	void OnGUI () {
        if (Screen.width != newWidth)
        {
            newWidth = Screen.width;
            float ratio = newWidth / originalWidth;

            foreach (Text t in FindObjectsOfType<Text>())
            {
                if (!originalFontSizes.ContainsKey(t))
                {
                    originalFontSizes[t] = t.fontSize;
                }
                t.fontSize = (int)(originalFontSizes[t] * ratio);
            }
        }
	}
}
