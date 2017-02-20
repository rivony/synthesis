using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	float boundaryLimit;// = 3.5f;
	float halfPlayerWidth;

	public float Speed = 7;

	// Use this for initialization
	void Start () {
		halfPlayerWidth = transform.localScale.x / 2f;
		boundaryLimit = Camera.main.aspect * Camera.main.orthographicSize + halfPlayerWidth;
	}
	
	// Update is called once per frame
	void Update () {
		float inputX = Input.GetAxisRaw ("Horizontal");
		float velocity = inputX * Speed;
		transform.Translate (Vector2.right * velocity * Time.deltaTime);
		if (transform.position.x < -boundaryLimit) {
			transform.position = new Vector2(boundaryLimit, transform.position.y);
		}
		if (transform.position.x > boundaryLimit) {
			transform.position = new Vector2(-boundaryLimit, transform.position.y);
		}
	}
}
