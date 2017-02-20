using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviourScript : MonoBehaviour {

	float speed = 7;
	int coinCount;
	Vector3 velocity;

	Rigidbody myRigidBody;

	// Use this for initialization
	void Start () {
		myRigidBody = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 input = new Vector3 (Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
		Vector3 direction = input.normalized;
		velocity = direction * speed;
	}

	void FixedUpdate(){
		myRigidBody.position += velocity * Time.fixedDeltaTime;
	}

	void OnTriggerEnter(Collider triggerCollider){
		//print (string.Format("triggerCollider.gameObject.name = {0}", triggerCollider.gameObject.name));
		if(triggerCollider.tag.Equals("Coin", System.StringComparison.OrdinalIgnoreCase)){
			Destroy (triggerCollider.gameObject);
			coinCount++;
		}
	}
}
