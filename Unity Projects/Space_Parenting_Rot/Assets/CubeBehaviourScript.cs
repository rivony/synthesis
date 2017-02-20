using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeBehaviourScript : MonoBehaviour {
	float rotationalSpeed = 0f;
	float translationalSpeed = 0f;

	public Transform sphereTransform;
	// Use this for initialization
	void Start () {
		sphereTransform.parent = transform;
		sphereTransform.localScale = Vector3.one * 1/3;
	}
	
	// Update is called once per frame
	void Update () {
		rotationalSpeed = 180 * Time.deltaTime;
		translationalSpeed = 7 * Time.deltaTime;
		//transform.eulerAngles += Vector3.up * speed; // (0, 1, 0) * speed
		transform.Rotate(Vector3.up * rotationalSpeed, Space.World);
		transform.Translate (Vector3.forward * translationalSpeed, Space.World);

		if(Input.GetKeyDown(KeyCode.Space)){
			sphereTransform.position = Vector3.zero;
		}
	}
}
