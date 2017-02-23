using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float WalkSpeed = 2;
	public float RunSpeed = 6;

	public float TurnSmoothTime = 0.2f; //The time taken by the smoothDampAngle from the current value to the target value
	float _turnSmoothVelocity;

	public float SpeedSmoothTime = 0.1f;
	float _speedSmoothVelocity;
	float _currentSpeed;

	Animator _animator;

	Transform _cameraTransform;

	// Use this for initialization
	void Start () {
		_animator = GetComponent<Animator> ();
		_cameraTransform = Camera.main.transform;
	}
	
	// Update is called once per frame
	void Update () {
		//For the keyboard input
		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		//Turn the input vector to a direction
		Vector2 inputDir = input.normalized;

		//Calculate the rotation when the inputDir is not (0, 0)
		if (inputDir != Vector2.zero) {
			//Set character rotation
			var targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg +
				_cameraTransform.eulerAngles.y;
			//Smoothing the rotation
			targetRotation = Mathf.SmoothDampAngle (transform.eulerAngles.y, //current angle
				targetRotation, ref _turnSmoothVelocity, TurnSmoothTime); 
			transform.eulerAngles = Vector3.up * targetRotation;// up (rot around the y axis)
		}

		bool running = Input.GetKey (KeyCode.LeftShift);
		float targetSpeed = (running ? RunSpeed : WalkSpeed) * inputDir.magnitude; //the speed should be 0 if no key is kept down (inputDir.magnitude == 0)
		//Smoothing speed
		_currentSpeed = Mathf.SmoothDamp (_currentSpeed, targetSpeed, ref _speedSmoothVelocity, 
			SpeedSmoothTime);
		//To move the character
		transform.Translate(transform.forward * _currentSpeed * Time.deltaTime, Space.World); //move to the front in the world space
	
		float animationSpeedPercent = (running ? 1 : .5f) * inputDir.magnitude;
		_animator.SetFloat ("SpeedPercent", animationSpeedPercent,
			SpeedSmoothTime, Time.deltaTime); //Smoothing the animation - already built-in the animator
	}
}
