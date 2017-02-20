using UnityEngine;

public class FallingBlock : MonoBehaviour {
	public float Speed = 7;
	// Update is called once per frame
	void Update () {
		transform.Translate (Vector3.down * Time.deltaTime * Speed);
	}
}
