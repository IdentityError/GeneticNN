using System;
using UnityEngine;

public class CarIndividual : MonoBehaviour
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

    [Header("Live Values")]
    public float steering;
    public float throttle;

    [Header("Debug")]
    public float averageThrottle = 0F;
    public float timeTravelled = 0F;
    public float distanceTravelled = 0F;

    private int updateCycles = 0;
    private float currentThrottleSum = 0F;

    private float ackermanAngleRight;
    private float ackermanAngleLeft;
    private int directionsCount;
    private Vector3[] sensesDirections;
    private float[] neuralNetInput;
    private float[] neuralNetOutput;
    private bool endedSimulation = false;
    private bool netInitialized = false;
    private float angleStride;
    private Vector3 lastPosition;

    [HideInInspector] public NeuralNet neuralNet;
    [HideInInspector]
    public GeneticManager manager;

    public float fitness;
    public float breedingProbability;

    private void Start()
    {
        lastPosition = transform.position;

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
            UpdateStats();
            UpdateFitness();
        }
    }

    private void ManageWheels()
    {
        steering = neuralNetOutput[0];//Input.GetAxis("Horizontal"); //TODO NN
        throttle = neuralNetOutput[1];//Input.GetAxis("Vertical");

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
        for (int i = 0; i < directionsCount; i++)
        {
            float currentAngle = angleStride * (i + 1);
            currentAngle -= transform.rotation.eulerAngles.y;
            sensesDirections[i] = new Vector3(Mathf.Cos(currentAngle * TMath.DegToRad), 0F, Mathf.Sin(currentAngle * TMath.DegToRad));

            sensesDirections[i].Normalize();

            LayerMask ignore = ~LayerMask.GetMask("CarIndividual");
            if (Physics.Raycast(transform.position, sensesDirections[i], out hits[i], length, ignore))
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
        }
        neuralNetInput[neuralNet.dna.topology.inputCount - 1] = throttle;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!endedSimulation)
        {
            endedSimulation = true;
            throttle = 0;
            steering = 0;
            manager?.DecrementPopulationAliveCount();
        }
    }

    private void UpdateFitness()
    {
        fitness += 0.0012F * (2F * (float)Math.Exp(3F * (throttle - 1F)) * (1.5F * distanceTravelled + (0.075F * timeTravelled)));
    }

    private void UpdateStats()
    {
        distanceTravelled += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;
        timeTravelled += Time.deltaTime;

        updateCycles++;
        currentThrottleSum += throttle;
        averageThrottle = currentThrottleSum / updateCycles;
    }
}