using UnityEngine;

public class Car : MonoBehaviour
{
    [Header("Specifications")]
    public Wheel[] wheels;

    [Tooltip("Length from rear to front axes")]
    public float wheelBase;

    [Tooltip("Length from left to right axes")]
    public float rearTrack;

    [Tooltip("Radius of the turn")]
    public float turnRadius;

    public float motorPower;
    public float steeringPower;

    [Header("Senses")]
    [Tooltip("Length of the raycasted senses")]
    public float length;

    private float angleStride;

    [Header("Live Values")]
    public float steerInput;

    public float throttle;

    private float ackermanAngleRight;
    private float ackermanAngleLeft;

    private NeuralNet neuralNet;

    private int directionsCount;
    private Vector3[] sensesDirections;
    private float[] inputsVector;

    private float[] neuralNetOutput;

    private void Start()
    {
        neuralNet = GetComponent<NeuralNet>();
        neuralNet.InitializeNet();

        directionsCount = neuralNet.inputsCount - 1;
        sensesDirections = new Vector3[directionsCount];

        inputsVector = new float[neuralNet.inputsCount];

        angleStride = (180F / (directionsCount + 1));

        foreach (Wheel w in wheels)
        {
            w.motorPower = this.motorPower;
            w.steeringPower = this.steeringPower;
        }
    }

    private void Update()
    {
        Sense();
        neuralNetOutput = neuralNet.Process(inputsVector);
        ManageWheels();
    }

    private void ManageWheels()
    {
        steerInput = neuralNetOutput[0];//Input.GetAxis("Horizontal"); //TODO NN
        throttle = neuralNetOutput[1];//Input.GetAxis("Vertical");
        if (steerInput > 0)
        {
            ackermanAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * steerInput;
            ackermanAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * steerInput;
        }
        else if (steerInput < 0)
        {
            ackermanAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * steerInput;
            ackermanAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * steerInput;
        }
        else
        {
            ackermanAngleLeft = 0;
            ackermanAngleRight = 0;
        }

        foreach (Wheel w in wheels)
        {
            if (w.type == Wheel.WheelType.FRONT_L)
            {
                w.steerAngle = ackermanAngleLeft;
            }
            if (w.type == Wheel.WheelType.FRONT_R)
            {
                w.steerAngle = ackermanAngleRight;
            }
            w.throttle = this.throttle;
        }
    }

    private void Sense()
    {
        RaycastHit[] hits = new RaycastHit[directionsCount];

        for (int i = 0; i < directionsCount; i++)
        {
            float currentAngle = angleStride * (i + 1);
            currentAngle -= (transform.rotation.eulerAngles.y);
            sensesDirections[i] = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad), 0F, Mathf.Sin(currentAngle * Mathf.Deg2Rad));

            sensesDirections[i].Normalize();

            Debug.DrawRay(transform.position, sensesDirections[i] * length, Color.cyan);
            if (Physics.Raycast(transform.position, sensesDirections[i], out hits[i], length))
            {
                float inversedNormalizedDistance = 1F - (hits[i].distance / length);
                inputsVector[i] = inversedNormalizedDistance;
            }
            else
            {
                inputsVector[i] = 0.8F;
            }
        }
        inputsVector[neuralNet.inputsCount - 1] = throttle;
    }
}