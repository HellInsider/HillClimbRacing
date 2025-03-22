using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCar : MonoBehaviour
{
    [SerializeField] public float engineForce;
    [SerializeField] public float maxSpeed;
    [SerializeField] public float rotationSpeed;
    public WheelJoint2D[] wheelJoints; 
    public Transform centerOfMass;
    public bool checkfuel = true;
    private Rigidbody2D rb;
    private float moveInput;
    //private float torqueInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (centerOfMass != null)
        {
            rb.centerOfMass = centerOfMass.localPosition;
        }
    }

    void Update()
    {
        if (checkfuel) moveInput = Input.GetAxis("Horizontal");
        else moveInput = 0;
        // torqueInput = Input.GetAxis("Vertical");
    }
    void FixedUpdate()
    {
        foreach (var wheelJoint in wheelJoints)
        {
            if (wheelJoint != null)
            {
                JointMotor2D motor = wheelJoint.motor;
                motor.motorSpeed = moveInput * engineForce;
                motor.maxMotorTorque = 10000;
                wheelJoint.motor = motor;
                wheelJoint.useMotor = moveInput != 0;
            }
        }
        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        /*if (torqueInput != 0)
        {
            rb.AddTorque(torqueInput * rotationSpeed);
        }*/
    }
    public float GetMoveInput()
    {
        return moveInput;
    }

    public float GetCurrentSpeed()
    {
        return rb.velocity.magnitude;
    }

    public void StopCar()
    {
        foreach (var wheelJoint in wheelJoints)
        {
            if (wheelJoint != null)
            {
                JointMotor2D motor = wheelJoint.motor;
                motor.motorSpeed = 0;
                motor.maxMotorTorque = 0;
                wheelJoint.motor = motor;
                wheelJoint.useMotor = false;
                checkfuel = false;
            }
        }
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }
}
