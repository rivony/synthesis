using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeGame : MonoBehaviour {

	int waitTime;
	float roundStartTime = 0;
	float roundStartDelayTime = 3;
	bool roundStarted = false;

	// Use this for initialization
	void Start () {
		print ("Press the spacebar once you think the alloted time is up.");
		Invoke ("SetNewRandomTime", roundStartDelayTime);
	}

	string GetMessage (float error)
	{
		if (error < .15f) {
			return "Outstanding!";
		}
		if (error < .75f) {
			return "Exceeds Expectations.";
		}
		if (error < 1.25f) {
			return "Acceptable.";
		}
		if (error < 1.75f) {
			return "Poor.";
		}
		return "Dreadful.";
	}

	void InputReceived ()
	{
		roundStarted = false;

		float playerWaitTime = Time.time - roundStartTime;
		float error = Mathf.Abs (waitTime - playerWaitTime);
		string message = GetMessage (error);
		print (string.Format ("You waited for {0} seconds. That's {1} seconds off. {2}", playerWaitTime, error, message));

		Invoke ("SetNewRandomTime", roundStartDelayTime);
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space) && roundStarted){
			InputReceived ();
		}
	}

	void SetNewRandomTime(){
		waitTime = Random.Range (5, 21);
		roundStartTime = Time.time; //current time
		roundStarted = true;
		print(string.Format("Wait time = {0}", waitTime));
	}
}
