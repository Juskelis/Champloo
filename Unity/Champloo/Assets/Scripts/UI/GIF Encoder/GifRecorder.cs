using UnityEngine;
using System;
using System.IO;
using GifEncoder;

public class GifRecorder : MonoBehaviour
{
	public Camera renderCamera;
    public Camera copyCamera;
	public float lengthInSeconds = 2;
	public bool customResolution = false;
	public Vector2 GifResolution = Vector2.zero;
	
	private bool rendering = false;
	public bool Rendering
	{
		get { return rendering; }
	}
	
	private RenderTexture renderTexture;
	private AnimatedGifEncoder gifEncoder;
	
	private float timespent = 0;
	
	private string currentPath = "";

	// Use this for initialization
	void Start ()
	{
        renderCamera.CopyFrom(copyCamera);

		// 320/width = x/height
		int width = (int)GifResolution.x;
		int height = (int)GifResolution.y;
		if(!customResolution)
			height = (int)(Screen.height * ((float)width)/Screen.width);
		
		renderTexture = new RenderTexture(width, height, 24);
		renderCamera.enabled = false;
		renderCamera.targetTexture = renderTexture;

        #if UNITY_EDITOR
        currentPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/" + Application.productName + "/Recordings/";
        #else
        currentPath = Application.dataPath + "/Recordings/";
        #endif

        Directory.CreateDirectory(currentPath);
    }
	
	public void BeginRendering()
	{
		if(rendering)
		{
			Debug.Log("Recording Error: attempt to begin recording while already recording");
			return;
        }

		gifEncoder = new AnimatedGifEncoder(currentPath + "output_" + DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + ".gif");
        
		gifEncoder.SetDelay(1000/60);
		//Invoke("Finish", lengthInSeconds);
		timespent = 0;
		rendering = true;
	}

    void CopyCamera()
    {
        renderCamera.transform.position = copyCamera.transform.position;
        renderCamera.orthographicSize = copyCamera.orthographicSize;
    }
	
	// Update is called once per frame
	void Update () {
		if(rendering)
		{
            CopyCamera();

			renderCamera.Render();
			
			RenderTexture.active = renderTexture;
			Texture2D frameTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
			frameTexture.ReadPixels(new Rect(0,0,renderTexture.width, renderTexture.height), 0, 0);
			
			gifEncoder.AddFrame(frameTexture);
			
			UnityEngine.Object.Destroy(frameTexture);
			
			gifEncoder.SetDelay((int)(Time.deltaTime*1000));
			
			timespent += Time.deltaTime;
			if(timespent >= lengthInSeconds)
				Finish();
		}
	}
	
	void Finish()
	{
		Debug.Log("Finished Writing To " + currentPath);
		gifEncoder.Finish();
		rendering = false;
	}
}
