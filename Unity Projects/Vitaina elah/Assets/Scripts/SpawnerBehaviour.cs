using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerBehaviour : MonoBehaviour {

	//public
	public GameObject FallingBlockPrefab;
	//scale
	public float SpawnSizeMin;
	public float SpawnSizeMax;
	//rotation
	public float SpawnAngleMax;
	//time
	public float SecondsBetweenSpawnMax;
	public float SecondsBetweenSpawnMin;

	//private
	Vector2 screenHalfSizeInWorldUnits;
	float nextSpawnTime;


	void Start(){
		screenHalfSizeInWorldUnits = new Vector2 (Camera.main.aspect * Camera.main.orthographicSize,
			Camera.main.orthographicSize);
	}

	// Update is called once per frame
	void Update () {
		if (Time.time > nextSpawnTime) {
			//time
			var secondsBetweenSpawns = Mathf.Lerp(SecondsBetweenSpawnMin,  SecondsBetweenSpawnMax, Difficulty.GetDifficultyPercent());
			nextSpawnTime = Time.time + secondsBetweenSpawns;
			//position
			var spawnSize = Random.Range (SpawnSizeMin, SpawnSizeMax);
			Vector2 spawnPosition = new Vector2 (Random.Range ( - screenHalfSizeInWorldUnits.x, screenHalfSizeInWorldUnits.x),
				screenHalfSizeInWorldUnits.y + spawnSize);
			//rotation
			var spawnAngle = Random.Range (-SpawnAngleMax, SpawnAngleMax);
			//instantiation
			var newFallingBlock = Instantiate (FallingBlockPrefab, spawnPosition, Quaternion.Euler(new Vector3(0, 0, 1) * spawnAngle));
			//scale
			newFallingBlock.transform.localScale = new Vector2 (1, 1) * spawnSize; //Vector2 (1, 1): vector one
		}
	}
}
