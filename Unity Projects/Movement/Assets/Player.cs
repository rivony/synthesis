using UnityEngine;

public class Player : MonoBehaviour {

	float speed = 10;

	// Update is called once per frame
	void Update () {
		Vector3 input = new Vector3 (Input.GetAxisRaw ("Horizontal"), 0, Input.GetAxisRaw ("Vertical"));
		Vector3 direction = input.normalized;
		Vector3 velocity = direction * speed;
		Vector3 moveAmount = velocity * Time.deltaTime;
		//print(string.Format("Move Amount: {0}", moveAmount));
		//transform.position += moveAmount;
		transform.Translate(moveAmount);
		//print(string.Format("Input vector: {0}", input));
	}
}
