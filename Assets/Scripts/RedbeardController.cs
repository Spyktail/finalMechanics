using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class RedbeardController : MonoBehaviour
{
    public float RedSpeed;
    private Rigidbody rb;
    private PlayerInput playerInput;
    public Vector3 movement;
    private float moveX;
    private float moveY;
    private float lookX, lookY;
    public float lookSpeedHoriz = 2.0f;
    public float lookSpeedVert = 2.0f;

    private float yaw = 0.0f;
    private float pitch = 0.0f;
    private Vector3 camRotation;

    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnLook(InputValue lookValue)
    {
        Vector2 lookVector = lookValue.Get<Vector2>();
        lookX = lookVector.x;
        lookY = lookVector.y;
    }

    // Update is called once per frame
    void Update()
    {
        yaw += lookSpeedHoriz * lookX;
        pitch -= lookSpeedVert * lookY;
        transform.eulerAngles = new Vector3(Mathf.Clamp(pitch,-30.0f, 30.0f), yaw, 0.0f);
        //movement = playerInput.OnMove();
    }

    private void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();

        moveX = movementVector.x;
        moveY = movementVector.y;
    }

    
    void FixedUpdate()
    {
        movement = new Vector3(moveX, 0.0f, moveY);

        rb.AddForce(movement * RedSpeed * Time.fixedDeltaTime);

    }



}

