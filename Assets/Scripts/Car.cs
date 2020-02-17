using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public NeuralNet neuralNet;
    public List<AxleInfo> axleInfos; // the information about each individual axle
    private Rigidbody rigidbody;

    public float throttle, steering;
    void Start()
    {
        neuralNet = gameObject.AddComponent<NeuralNet>();
        rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        //if (collider.transform.childCount == 0)
        //{
        //    return;
        //}

        //Transform visualWheel = collider.transform.GetChild(0);

        //Vector3 position;
        //Quaternion rotation;
        //collider.GetWorldPose(out position, out rotation);

        //visualWheel.transform.position = position;
        //visualWheel.transform.rotation = rotation;
    }

    void FixedUpdate()
    {
        float motor = throttle * Input.GetAxis("Vertical");
        float turn = steering * Input.GetAxis("Horizontal");

        foreach (AxleInfo axleInfo in axleInfos)
        {
            if (axleInfo.steering)
            {
                axleInfo.leftWheel.steerAngle = turn;
                axleInfo.rightWheel.steerAngle = turn;
            }
            if (axleInfo.motor)
            {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }

    }
}

[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor; // is this wheel attached to motor?
    public bool steering; // does this wheel apply steer angle?
}