using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour {

	public GameObject FallingBockPrefa;
	public float TimeBetweenSpawns = 1; //1 second

	public Vector2 SpawnSizeMinMax;
	public float SpawnAngleMax;

	float nextSpawnTime;
	Vector2 screenHalfSizeWorldUnits;

	// Use this for initialization
	void Start () {
		screenHalfSizeWorldUnits = new Vector2 (Camera.main.aspect * Camera.main.orthographicSize, 
			Camera.main.orthographicSize);
	}
	
	// Update is called once per frame
	void Update () {

		if (Time.time > nextSpawnTime) {
			nextSpawnTime = Time.time + TimeBetweenSpawns;

			float spawnSize = Random.Range (SpawnSizeMinMax.x, SpawnSizeMinMax.y);
			float spawnAngle = Random.Range (-SpawnAngleMax, SpawnAngleMax);
			Vector2 spawnPosition = new Vector2 (Random.Range (-screenHalfSizeWorldUnits.x, screenHalfSizeWorldUnits.x),
				screenHalfSizeWorldUnits.y + .5f);
			
			GameObject newBlock = Instantiate (FallingBockPrefa, spawnPosition, 
				Quaternion.Euler(Vector3.forward * spawnAngle));
			
			newBlock.transform.localScale = Vector2.one * spawnSize;
		}
	}
}
