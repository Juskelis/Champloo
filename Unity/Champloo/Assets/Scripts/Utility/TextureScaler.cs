using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class TextureScaler : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
	    float scaleDim = Mathf.Min(transform.localScale.x, transform.localScale.y);
        GetComponent<Renderer>().material.mainTextureScale = new Vector2(
            transform.localScale.x/scaleDim,
            transform.localScale.y/scaleDim
        );
	}
}
