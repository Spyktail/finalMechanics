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

    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
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

