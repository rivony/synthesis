using UnityEngine;

public class Spawner : MonoBehaviour {

	public GameObject FallingBlockPrefab;
	public float secondsBetweenSpawns = 1; //one second

	float nextSpawnTime;
	Vector2 screenHalfSizeWorldUnits;
	float angle;

	// Use this for initialization
	void Start () {
		screenHalfSizeWorldUnits = new Vector2 (Camera.main.aspect * Camera.main.orthographicSize,
			Camera.main.orthographicSize);
		angle = Mathf.Atan2 (screenHalfSizeWorldUnits.x, screenHalfSizeWorldUnits.y) * Mathf.PI / 180 ;
		print (string.Format("screenHalfSizeWorldUnits.y = {0}, screenHalfSizeWorldUnits.x = {1}", 
			screenHalfSizeWorldUnits.y, screenHalfSizeWorldUnits.x));
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > nextSpawnTime) {
			nextSpawnTime = Time.time + secondsBetweenSpawns;
			//position
			float random_x = Random.Range (-screenHalfSizeWorldUnits.x, screenHalfSizeWorldUnits.x);
			Vector2 spawnPosition = new Vector2 (random_x, screenHalfSizeWorldUnits.y + .5f); //.5f = half the siwe of the object
			//rotation
			Vector3 spawnRotation = new Vector3(0,0,1) * Random.Range(0, 60);
			Instantiate (FallingBlockPrefab, spawnPosition, Quaternion.Euler(spawnRotation))
				.transform.localScale = new Vector2(1,1) * Random.Range( .5f, 2); //scale
		}
	}
}
