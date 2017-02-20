using UnityEngine;

public class Chaser : MonoBehaviour {

	public float Speed = 7;

	public Transform targetTransform;
	// Update is called once per frame
	void Update () {
		Vector3 displacementFromTarget = targetTransform.position - transform.position;
		//print (string.Format("Chaser targetTransform.position = ", targetTransform.position));
		Vector3 directionToTarget = displacementFromTarget.normalized;
		Vector3 velocity = directionToTarget * Speed;

		float distanceToTarget = displacementFromTarget.magnitude;

		if (distanceToTarget > 1.5f) {
			transform.Translate (velocity * Time.deltaTime);
		}
	}
}
