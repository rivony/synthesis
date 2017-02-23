using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {

	float _pitch;
	float _yaw;

	Vector3 _rotationSmoothVelocity;
	Vector3 _currentRotation;

	public float MouseSensitivity = 10;
	public Transform Target;
	public float DistanceFromTarget = 2;

	public Float2 PitchMinMax = new Float2(-40, 85);
	public float RotationSmoothTime = .12f;

	public bool LockCursor;

	// Use this for initialization
	void Start () {
		if (LockCursor) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {
		_yaw += Input.GetAxis ("Mouse X") * MouseSensitivity;
		_pitch -= Input.GetAxis("Mouse Y") * MouseSensitivity;

		_pitch = Mathf.Clamp (_pitch, PitchMinMax.X, PitchMinMax.Y);

		_currentRotation = Vector3.SmoothDamp (_currentRotation, new Vector3 (_pitch, _yaw),
			ref _rotationSmoothVelocity, RotationSmoothTime);
		
		transform.eulerAngles = _currentRotation;

		transform.position = Target.position - transform.forward * DistanceFromTarget;
	}
}
