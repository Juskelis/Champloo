using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

    [SerializeField]
    private Transform objectToSpawn;

    [SerializeField]
    private float spawnDelay;

    [SerializeField]
    private float spawnWidth;

    [SerializeField]
    private float spawnHeight;

	// Use this for initialization
	void Start () {
        Invoke("Spawn", spawnDelay);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void Spawn()
    {
        Instantiate(
            objectToSpawn,
            transform.position + Vector3.right * Random.Range(-1f, 1f) * spawnWidth + Vector3.up * Random.Range(-1f, 1f) * spawnHeight,
            transform.rotation
        );
        Invoke("Spawn", spawnDelay);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, Vector3.right * spawnWidth*2 + Vector3.up * spawnHeight*2);
    }
}
