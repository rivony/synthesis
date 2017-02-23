using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float WalkSpeed = 2;
	public float RunSpeed = 6;
	public float Gravity = -12;
	float _velocityOnYaxis;

	public float JumpHeight = 1;
	[Range(0,1)]
	public float AirControlPercent;

	public float TurnSmoothTime = 0.2f; //The time taken by the smoothDampAngle from the current value to the target value
	float _turnSmoothVelocity;

	public float SpeedSmoothTime = 0.1f;
	float _speedSmoothVelocity;
	float _currentSpeed;

	Animator _animator;

	Transform _cameraTransform;
	CharacterController _controller;

	// Use this for initialization
	void Start () {
		_animator = GetComponent<Animator> ();
		_cameraTransform = Camera.main.transform;
		_controller = GetComponent<CharacterController> ();
	}
	
	// Update is called once per frame
	void Update () {
		//For the keyboard input
		Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		//Turn the input vector to a direction
		Vector2 inputDir = input.normalized;
		bool running = Input.GetKey (KeyCode.LeftShift);
		Move (inputDir, running);

		if (Input.GetKeyDown(KeyCode.Space)) {
			Jump ();
		}

		//animator
		float animationSpeedPercent = running ? _currentSpeed/RunSpeed : _currentSpeed/WalkSpeed * .5f;
		_animator.SetFloat ("SpeedPercent", animationSpeedPercent,
			SpeedSmoothTime, Time.deltaTime); //Smoothing the animation - already built-in the animator

	}


	void Move (Vector2 inputDir, bool running){
		//Calculate the rotation when the inputDir is not (0, 0)
		if (inputDir != Vector2.zero) {
			//Set character rotation
			var targetRotation = Mathf.Atan2(inputDir.x, inputDir.y) * Mathf.Rad2Deg +
				_cameraTransform.eulerAngles.y;
			//Smoothing the rotation
			targetRotation = Mathf.SmoothDampAngle (transform.eulerAngles.y, //current angle
				targetRotation, ref _turnSmoothVelocity, GetModifiedSmoothTime(TurnSmoothTime)); 
			transform.eulerAngles = Vector3.up * targetRotation;// up (rot around the y axis)
		}


		float targetSpeed = (running ? RunSpeed : WalkSpeed) * inputDir.magnitude; //the speed should be 0 if no key is kept down (inputDir.magnitude == 0)
		//Smoothing speed
		_currentSpeed = Mathf.SmoothDamp (_currentSpeed, targetSpeed, ref _speedSmoothVelocity, 
			GetModifiedSmoothTime(SpeedSmoothTime));

		_velocityOnYaxis += Time.deltaTime * Gravity;

		//To move the character
		Vector3 velocity = transform.forward * _currentSpeed + Vector3.up * _velocityOnYaxis;
		_controller.Move (velocity * Time.deltaTime);
		_currentSpeed = new Vector2 (_controller.velocity.x, _controller.velocity.z) //the velocity is on the plane, we are setting the Y axis separately
			.magnitude; // it is the speed not the velocity: magnitude

		//Reset the velocityOnYaxis when the character is on the ground
		if (_controller.isGrounded) {
			_velocityOnYaxis = 0;
		}


	}

	void Jump(){
		if (_controller.isGrounded) {
			float jumpVelocity = Mathf.Sqrt (-2 * Gravity * JumpHeight);
			_velocityOnYaxis = jumpVelocity;
		}
	}

	float GetModifiedSmoothTime(float smoothTime){
		if (_controller.isGrounded) {
			return smoothTime;
		}
		if (AirControlPercent == 0) {
			return float.MaxValue;
		}
		return smoothTime / AirControlPercent; //Less control for the player when the character is in the Air (if controlPercent is for eg. )
	}
}
