using UnityEngine;

public class FallingBlock : MonoBehaviour {

	float Speed = 7;

	// Update is called once per frame
	void Update () {
		transform.Translate (Vector3.down * Speed * Time.deltaTime, Space.Self);
	}
}
