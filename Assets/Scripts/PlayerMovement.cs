using UnityEngine;

using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Component for Player.
/// Handles user input. Makes Player gameobject roll, fly and absorb.
/// </summary>
public class PlayerMovement : MonoBehaviour
{
    public bool isFlying = false;
    public bool isAbsorbing = false;
    private float maxFlyHeight = 1f;

    private float speed = 12f;
    private float takeoffForce = 12f;
    private Rigidbody rb;
    private float horizontalInput;
    private float verticalInput;
    private bool jumpInput;
    private bool absorbInput;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");   // WASD to move
        verticalInput = Input.GetAxis("Vertical");
        jumpInput = Input.GetKey(KeyCode.Space);         // SPACE to jump
        absorbInput = Input.GetKey(KeyCode.E);           // "E" to absorb cubes
        
    }

    private void FixedUpdate()
    {
        Move();

        isFlying = (jumpInput) ? true : false;
        if (isFlying) // pressing Spacebar
        {
            Fly();
        }

        isAbsorbing = (absorbInput) ? true : false;

    }

    private void Move()
    {
        Vector3 moveVersor = new Vector3(horizontalInput, 0, verticalInput).normalized;
        Vector3 moveVector = moveVersor * speed;

        //rb.AddTorque(moveVector); // moveVector are the axis around witch the torque has effect
        rb.AddForce(moveVector);
    }

    private void Fly()
    {
        Vector3 upwardForce = Vector3.up * takeoffForce;

        if (transform.position.y > maxFlyHeight) // dont fly higher than threshold height
        {
            rb.velocity = new Vector3(rb.velocity.x, 0.0f, rb.velocity.z); // kill y inertia
            upwardForce = Physics.gravity;
        }

        rb.AddForce(upwardForce);
    }
}
