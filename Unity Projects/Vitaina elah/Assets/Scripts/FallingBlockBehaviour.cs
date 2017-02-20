using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingBlockBehaviour : MonoBehaviour {

	//public
	public float SpeedMin;
	public float SpeedMax;

	float speed;
	float visibleHeightThreshold;

	void Start(){
		speed = Mathf.Lerp (SpeedMin, SpeedMax, Difficulty.GetDifficultyPercent ());
		visibleHeightThreshold = -Camera.main.orthographicSize - transform.localScale.y;
	}

	// Update is called once per frame
	void Update () {
		transform.Translate (new Vector3 (0f, -1f, 0f) * speed * Time.deltaTime, Space.Self); //Vector3 (0f, -1f, 0f): down

		if (transform.position.y < visibleHeightThreshold) {
			Destroy (gameObject);
		}
	}
}
