﻿using UnityEngine;

public class Wheel : MonoBehaviour
{
    public enum WheelType { FRONT_L, FRONT_R, REAR_L, REAR_R }

    private Rigidbody rigidbody;

    [Header("Suspension")]
    public float restLength;

    public float springTravel;
    public float springStiffness;
    public float damperStiffness;

    [Header("Wheel")]
    public float wheelRadius;

    public WheelType type;
    public float steerTime;

    private float minLength;
    private float maxLength;
    private float lastLength;
    private float springLength;
    private float springForce;
    private float damperForce;
    private float springVelocity;
    private float wheelAngle = 0;
    [HideInInspector] public float steerAngle;
    [HideInInspector] public float motorPower;
    [HideInInspector] public float steeringPower;
    [HideInInspector] public float throttle;
    private Vector3 suspensionForce;
    private Vector3 wheelVelocity;
    private float forwardForce;
    private float rightForce;

    private void Start()
    {
        rigidbody = transform.root.GetComponent<Rigidbody>();
        minLength = restLength - springTravel;
        maxLength = restLength + springTravel;
    }

    private void Update()
    {
        wheelAngle = Mathf.Lerp(wheelAngle, steerAngle, steerTime * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(Vector3.up * wheelAngle);

        Debug.DrawRay(transform.position, -transform.up * (springLength + wheelRadius), Color.green);
    }

    private void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, -transform.up, out RaycastHit hit, maxLength + wheelRadius, LayerMask.GetMask("Floor")))
        {
            lastLength = springLength;
            springLength = hit.distance - wheelRadius;
            springLength = Mathf.Clamp(springLength, minLength, maxLength);
            springVelocity = (lastLength - springLength) / Time.fixedDeltaTime;
            springForce = springStiffness * (restLength - springLength);
            damperForce = damperStiffness * springVelocity;

            wheelVelocity = transform.InverseTransformDirection(rigidbody.GetPointVelocity(hit.point));

            forwardForce = throttle * motorPower;
            rightForce = wheelVelocity.x * steeringPower;

            suspensionForce = (springForce + damperForce) * transform.up;

            rigidbody.AddForceAtPosition(suspensionForce + (forwardForce * transform.forward) + (rightForce * -transform.right), hit.point);
        }
    }
}