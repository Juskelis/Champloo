using UnityEngine;

public class Parallaxer : MonoBehaviour {

    private Vector3 previousCamPosition;

	// Use this for initialization
	void Start () {
        previousCamPosition = Camera.main.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        //find farthest one and invert all based on that one
        float maxZ = float.MinValue;
        for(int i = 0; i < transform.childCount; i++)
        {
            maxZ = Mathf.Max(maxZ, transform.GetChild(i).transform.position.z);
        }

		for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            float parallax = (Camera.main.transform.position.x - previousCamPosition.x) * (1-child.position.z/maxZ);

            Vector3 newPosition = child.position;
            newPosition.x += parallax;
            child.position = newPosition;
        }
        previousCamPosition = Camera.main.transform.position;
	}
}
