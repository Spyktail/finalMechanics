using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class RedbeardController : MonoBehaviour
{
	public float moveSpeed;
	
	public float sprintSpeed;
		[Tooltip("How fast the character turns to face movement direction")]
		[Range(0.0f, 0.5f)]
		public float rotationSmoothTime = 0.3f;
		public float speedChangeRate = 10.0f;


        public float speedBoostDuration = 5.0f;

		
		public GameObject CinemachineCameraTarget;
		[Tooltip("How far in degrees can you move the camera up")]
		public float topClamp = 70.0f;
		[Tooltip("How far in degrees can you move the camera down")]
		public float bottomClamp = -30.0f;
		[Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
		public float cameraAngleOverride = 0.0f;
		[Tooltip("For locking the camera position on all axis")]
		public bool LockCameraPosition = false;

        
        

		// cinemachine
		private float cinemachineTargetYaw;
		private float cinemachineTargetPitch;

		// player
		private float speed;
		private float _animationBlend;
		private float targetRotation = 0.0f;
		private float rotationVelocity;
		

		private PlayerInput _playerInput;
		private Animator _animator;
		private CharacterController _controller;
		private TPSInput _input;
		public Jumping _jumping;
		private GameObject _mainCamera;

		private const float camThreshold = 0.01f;


		private bool IsCurrentDeviceMouse => _playerInput.currentControlScheme == "KeyboardMouse";

		private void Awake()
		{
			// get a reference to our main camera
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
		}

		private void Start()
		{
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<TPSInput>();
			_playerInput = GetComponent<PlayerInput>();


			
		}

		private void Update()
		{
			
			Move();

            if (_input.quit)
            {
                Application.Quit();
            }
            //OnControllerColliderHit(Wall);
		}

		private void LateUpdate()
		{
			CameraRotation();
		}

		private void CameraRotation()
		{
			// if there is an input and camera position is not fixed
			if (_input.look.sqrMagnitude >= camThreshold && !LockCameraPosition)
			{
				//Don't multiply mouse input by Time.deltaTime;
				float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
				
				cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
				cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
			}

			// clamp our rotations so our values are limited 360 degrees
			cinemachineTargetYaw = CamClamp(cinemachineTargetYaw, float.MinValue, float.MaxValue);
			cinemachineTargetPitch = CamClamp(cinemachineTargetPitch, bottomClamp, topClamp);

			// Cinemachine will follow this target
			CinemachineCameraTarget.transform.rotation = Quaternion.Euler(cinemachineTargetPitch + cameraAngleOverride, cinemachineTargetYaw, 0.0f);
		}

		private static float CamClamp(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

        
        
		private void Move()
		{
			// set target speed based on move speed, sprint speed and if sprint is pressed
			float targetSpeed = _input.sprint ? sprintSpeed : moveSpeed;

                if (_input.sprint)
                {
                    if (speedBoostDuration > 0)
                    {
                        speedBoostDuration -= Time.deltaTime;
                        if (speedBoostDuration <= 0.01f)
                        {
                            _input.sprint = false;
                            speedBoostDuration = 0.5f;
                        }
                    }
                }

			// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

			// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is no input, set the target speed to 0
			if (_input.move == Vector2.zero) 
			{
				targetSpeed = 0.0f;
			}
			// a reference to the players current horizontal velocity
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			// accelerate or decelerate to target speed
			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				// creates curved result rather than a linear one giving a more organic speed change
				// note T in Lerp is clamped, so we don't need to clamp our speed
				speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * speedChangeRate);

				// round speed to 3 decimal places
				speed = Mathf.Round(speed * 1000f) / 1000f;
			}
			else
			{
				speed = targetSpeed;
			}
			//_animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);

			// normalise input direction
			Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
			// if there is a move input rotate player when the player is moving
			if (_input.move != Vector2.zero)
			{
				targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
				float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref rotationVelocity, rotationSmoothTime);

				// rotate to face input direction relative to camera position
				transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
			}


			Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

			// move the player
			_controller.Move(targetDirection.normalized * (speed * Time.deltaTime) + new Vector3(0.0f, _jumping.verticalVelocity, 0.0f) * Time.deltaTime);
		}

}

