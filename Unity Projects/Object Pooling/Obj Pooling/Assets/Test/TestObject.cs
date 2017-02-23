using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestObject : MonoBehaviour {

	float _speed = 25f;

	void Update(){
		transform.localScale += Vector3.one * Time.deltaTime * 3;
		transform.Translate (Vector3.forward * Time.deltaTime * _speed);
	}
}
