using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestManager : MonoBehaviour {

	public GameObject Prefab;
	int _poolSize = 3;

	// Use this for initialization
	void Start () {
		PoolManager.Instance.CreatePool (Prefab, _poolSize);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			PoolManager.Instance.ReuseObject (Prefab, Vector3.zero, Quaternion.identity);
		}
	}
}
