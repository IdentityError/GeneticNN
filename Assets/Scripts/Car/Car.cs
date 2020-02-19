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
    public float steering;
    public float throttle;

    [Header("Debug")]
    public bool debug;

    private float ackermanAngleRight;
    private float ackermanAngleLeft;
    private int directionsCount;
    private Vector3[] sensesDirections;
    private float[] neuralNetInput;
    private float[] neuralNetOutput;
    private bool endedSimulation = false;
    private bool netInitialized = false;

    [HideInInspector] public NeuralNet neuralNet;
    [HideInInspector]
    public GeneticManager manager;

    [HideInInspector] public float fitness;
    [HideInInspector] public float breedingProbability;

    private void Start()
    {
        foreach (Wheel w in wheels)
        {
            w.motorPower = this.motorPower;
            w.steeringPower = this.steeringPower;
        }
    }

    public void InitializeNeuralNet(DNA dna)
    {
        neuralNet = new NeuralNet(dna);
        directionsCount = neuralNet.dna.topology.inputCount - 1;
        sensesDirections = new Vector3[directionsCount];
        neuralNetInput = new float[neuralNet.dna.topology.inputCount];
        angleStride = (180F / (directionsCount + 1));

        netInitialized = true;
    }

    private void Update()
    {
        if (!endedSimulation && netInitialized)
        {
            Sense();
            neuralNetOutput = neuralNet.Process(neuralNetInput);
            ManageWheels();
        }
    }

    private void ManageWheels()
    {
        steering = neuralNetOutput[0];//Input.GetAxis("Horizontal"); //TODO NN
        throttle = neuralNetOutput[1];//Input.GetAxis("Vertical");

        if (debug)
        {
            Debug.Log("OUTPUT");
            Debug.Log("throttle: " + throttle + ", steering: " + steering);
        }

        if (steering > 0)
        {
            ackermanAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * steering;
            ackermanAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * steering;
        }
        else if (steering < 0)
        {
            ackermanAngleLeft = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius - (rearTrack / 2))) * steering;
            ackermanAngleRight = Mathf.Rad2Deg * Mathf.Atan(wheelBase / (turnRadius + (rearTrack / 2))) * steering;
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
        if (debug)
            Debug.Log("INPUT");
        for (int i = 0; i < directionsCount; i++)
        {
            float currentAngle = angleStride * (i + 1);
            currentAngle -= (transform.rotation.eulerAngles.y);
            sensesDirections[i] = new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad), 0F, Mathf.Sin(currentAngle * Mathf.Deg2Rad));

            sensesDirections[i].Normalize();

            if (Physics.Raycast(transform.position, sensesDirections[i], out hits[i], length))
            {
                float inversedNormalizedDistance = (hits[i].distance / length);
                neuralNetInput[i] = inversedNormalizedDistance;
                Debug.DrawRay(transform.position, hits[i].point - transform.position, Color.cyan);
            }
            else
            {
                neuralNetInput[i] = 1F;
                Debug.DrawRay(transform.position, sensesDirections[i] * length, Color.cyan);
            }
            if (debug)
                Debug.Log("input[" + i + "]: " + neuralNetInput[i]);
        }
        neuralNetInput[neuralNet.dna.topology.inputCount - 1] = throttle;
        if (debug)
            Debug.Log("throttle: " + throttle);
    }

    private void OnCollisionEnter(Collision collision)
    {
        endedSimulation = true;
        throttle = 0;
        steering = 0;
        manager?.DecrementPopulationAliveCount();
    }

    private void FitnessFunction()
    {
        //TODO implement a fitness function
    }
}