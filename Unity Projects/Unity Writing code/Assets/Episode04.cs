using System;
using UnityEngine;

public class Episode04 : MonoBehaviour {

	int frameCount = 0;

	// Use this for initialization
	void Start () {
		print ("Start");
		float distance = GetDistanceBetween (1, 3, 4, 5);

		print (String.Format("Distance is equal to {0}", distance));
	}
	
	// Update is called once per frame
	void Update () {
		frameCount++;
		//print (String.Format("Frame Count = {0}", frameCount));
	}

	float GetDistanceBetween(float x, float y, float another_x, float another_y){
		float dx = x - another_x;
		float dy = y - another_y;
		return Mathf.Sqrt (dx * dx + dy * dy);
	}
}
