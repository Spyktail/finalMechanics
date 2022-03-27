using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Jumping : MonoBehaviour
{
	public float jumpHeight = 1.2f;
	public float gravity = -15.0f;
	public float jumpTimeout = 0.50f;
	public float fallTimeout = 0.15f;
    private float jumpTimeoutDelta;
	private float fallTimeoutDelta;
	public bool isGrounded = true;
	public float groundedOffset = -0.14f;
	public float groundedRadius;
	public LayerMask groundLayers;

        [Header("MidAir Jump")]
        public bool isMidairJumping;
        public float midairJumpHeight;
        private bool isAirborne;
        public int extraJumps;


        public float verticalVelocity;
		private float terminalVelocity = 53.0f;
    
		private CharacterController _controller;
        private PlayerInput _playerInput;
		private TPSInput _input;
    // Start is called before the first frame update
    void Start()
    {
        _controller = GetComponent<CharacterController>();
		_input = GetComponent<TPSInput>();
		_playerInput = GetComponent<PlayerInput>();
		jumpTimeoutDelta = jumpTimeout;
		fallTimeoutDelta = fallTimeout;
    }

    // Update is called once per frame
    void Update()
    {
        Jumps();
		GroundedCheck();
    }

    private void GroundedCheck()
		{
			// set sphere position, with offset
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z);
			isGrounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
		}

		private void Jumps()
		{
			if (isGrounded)
			{
				// reset the fall timeout timer
				fallTimeoutDelta = fallTimeout;
                isAirborne = false;
                extraJumps = 1;

				// stop our velocity dropping infinitely when grounded
				if (verticalVelocity < 0.0f)
				{
					verticalVelocity = -2f;
				}

				// Jump
				if (_input.jump && jumpTimeoutDelta <= 0.0f)
				{
					// the square root of H * -2 * G = how much velocity needed to reach desired height
					verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
                    isAirborne = true;
                    //midairJump = true;
                    _input.jump = false;
				}

				// jump timeout
				if (jumpTimeoutDelta >= 0.0f)
				{
					jumpTimeoutDelta -= Time.deltaTime;
				}
			}
			else
			{
				// reset the jump timeout timer
				jumpTimeoutDelta = jumpTimeout;

				// fall timeout
				if (fallTimeoutDelta >= 0.0f)
				{
					fallTimeoutDelta -= Time.deltaTime;
				}
				
			}

			if (verticalVelocity < terminalVelocity)
			{
				verticalVelocity += gravity * Time.deltaTime;
			}
            if (extraJumps <= 0)
            {
                isMidairJumping = false;
                _input.jump = false;
            }

            if (!isGrounded)
            {
                isAirborne = true;
            }
            if (isAirborne)
            {
                isMidairJumping = true;
            
            }
            else
            {
                isMidairJumping = false;
            }
            if (isAirborne && isMidairJumping && _input.jump)
            {
                verticalVelocity += midairJumpHeight;
                _input.jump = false;
                isMidairJumping = false;
                extraJumps--;
            }
            
		}



        private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (isGrounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;
			
			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - groundedOffset, transform.position.z), groundedRadius);
		}
}

