using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConsistentTextSize : MonoBehaviour
{
    [SerializeField]
    private Text[] toSync;
	
	// Update is called once per frame
	void Update ()
	{
	    int minSize = int.MaxValue;
	    foreach (Text text in toSync)
	    {
	        text.resizeTextForBestFit = true;
            text.cachedTextGenerator.Invalidate();
	        Vector2 size = (text.transform as RectTransform).rect.size;
	        TextGenerationSettings tempSettings = text.GetGenerationSettings(size);
	        tempSettings.scaleFactor = 1;
	        if (!text.cachedTextGenerator.Populate(text.text, tempSettings))
	            Debug.LogError("Failed to generate fit size");
            minSize = Mathf.Min(
                minSize,
                text.cachedTextGenerator.fontSizeUsedForBestFit);
	    }
	    foreach (Text text in toSync)
	    {
            //for some reason the size is sometimes one too high
	        text.fontSize = Mathf.Max(minSize - 1, 1);
	        text.resizeTextForBestFit = false;
	    }
    }
}
