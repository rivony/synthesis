using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour {

	//public
	public float Speed = 7;
	public event System.Action OnPlayerDeath;

	//private
	float screenHalfWithInWorldUnits = 4f;

	// Use this for initialization
	void Start () {
		SetHalfScreen ();
	}

	#region Start methods

	void SetHalfScreen ()
	{
		float halfPlayerWidth = transform.localScale.x / 2f;
		screenHalfWithInWorldUnits = Camera.main.aspect * Camera.main.orthographicSize 
			+ halfPlayerWidth;
	}

	#endregion
	
	// Update is called once per frame
	void Update () {
		MoveAccordingToInput ();
		BoundariesReposition ();
	}

	#region Update methods

	void MoveAccordingToInput ()
	{
		float input = Input.GetAxisRaw ("Horizontal");
		float velocity = input * Speed;
		transform.Translate (new Vector2 (1, 0) * velocity * Time.deltaTime);
		//new Vector2 (1, 0): right
	}

	void BoundariesReposition ()
	{
		if (transform.position.x < -screenHalfWithInWorldUnits) {
			transform.position = new Vector2 (screenHalfWithInWorldUnits, transform.position.y);
		}
		if (transform.position.x > screenHalfWithInWorldUnits) {
			transform.position = new Vector2 (-screenHalfWithInWorldUnits, transform.position.y);
		}
	}

	#endregion

	void OnTriggerEnter2D(Collider2D triggerCollider){
		if (triggerCollider.tag.Equals("Falling Block")) {
			//FindObjectOfType<GameOverBehaviour> ().OnGameOver ();
			if (OnPlayerDeath != null) {
				OnPlayerDeath ();
			}
			Destroy (gameObject);
		}
	
	}
}
