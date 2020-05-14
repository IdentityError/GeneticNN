using Assets.Scripts.Interfaces;
using Assets.Scripts.NeuralNet;
using Assets.Scripts.TWEANN;
using System.Collections.Generic;
using UnityEngine;

public class CarIndividual : MonoBehaviour, ISimulatingIndividual
{
    [Header("Specifications")]
    public Wheel[] wheels;
    public bool manualControl;

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

    [Header("Debug")]
    public float steering;
    public float throttle;
    public SimulationStats stats;

    private int updateCycles = 0;
    private float currentThrottleSum = 0F;
    private float ackermanAngleRight;
    private float ackermanAngleLeft;
    private int directionsCount;
    private Vector3[] sensesDirections;
    private double[] neuralNetInput;
    private double[] neuralNetOutput;
    private bool endedSimulation = false;
    private bool netInitialized = false;
    private float angleStride;
    [HideInInspector] public Vector3 lastPosition;
    private new Rigidbody rigidbody;

    private NeuralNetwork neuralNet;
    private Species species;
    private double fitness;
    private double pickProbability;
    private PopulationManager populationManager;

    private void Start()
    {
        lastPosition = transform.position;
        rigidbody = GetComponent<Rigidbody>();

        foreach (Wheel w in wheels)
        {
            w.motorPower = this.motorPower;
            w.steeringPower = this.steeringPower;
        }

        if (populationManager != null)
        {
            stats.trackID = populationManager.GetTrack().GetId();
        }
    }

    private void Update()
    {
        if (!endedSimulation && netInitialized)
        {
            if (!manualControl)
            {
                Sense();
                neuralNetOutput = neuralNet.FeedForward(neuralNetInput);
            }
            ManageWheels();
            UpdateStats();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!endedSimulation)
        {
            EndIndividualSimulation();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("FinishLine"))
        {
            EndIndividualSimulation();
        }
    }

    private void ManageWheels()
    {
        if (manualControl)
        {
            steering = Input.GetAxis("Horizontal");
            throttle = Input.GetAxis("Vertical");
        }
        else
        {
            steering = (float)neuralNetOutput[0];
            throttle = (float)neuralNetOutput[1];
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
        List<double> netInputs = new List<double>();
        for (int i = 0; i < directionsCount; i++)
        {
            float currentAngle = angleStride * (i + 1);
            currentAngle -= transform.rotation.eulerAngles.y;
            sensesDirections[i] = new Vector3(Mathf.Cos(currentAngle * TMath.DegToRad), 0F, Mathf.Sin(currentAngle * TMath.DegToRad));

            sensesDirections[i].Normalize();

            if (Physics.Raycast(transform.position, sensesDirections[i], out hits[i], length, LayerMask.GetMask("Obstacles")))
            {
                double inversedNormalizedDistance = (hits[i].distance / length);
                neuralNetInput[i] = inversedNormalizedDistance;
                Debug.DrawRay(transform.position, hits[i].point - transform.position, Color.green);
            }
            else
            {
                neuralNetInput[i] = 1F;
                Debug.DrawRay(transform.position, sensesDirections[i] * length, Color.blue);
            }
        }
        neuralNetInput[neuralNet.GetGenotype().InputCount - 1] = throttle;
    }

    private void UpdateStats()
    {
        stats.distance += Vector3.Distance(transform.position, lastPosition);
        lastPosition = transform.position;
        stats.time += Time.deltaTime;

        updateCycles++;
        currentThrottleSum += throttle;
        stats.averageThrottle = currentThrottleSum / updateCycles;
    }

    public void EndIndividualSimulation()
    {
        if (endedSimulation) return;
        endedSimulation = true;
        throttle = 0;
        steering = 0;
        populationManager?.IndividualEndedSimulation(this);
    }

    public CarIndividualData GetIndividualData()
    {
        return new CarIndividualData(neuralNet.GetGenotype(), ProvideFitness());
    }

    #region Interface

    public void SetPopulationManager(PopulationManager populationManager)
    {
        this.populationManager = populationManager;
    }

    public NeuralNetwork ProvideNeuralNet()
    {
        return this.neuralNet;
    }

    public void SetNeuralNet(NeuralNetwork net)
    {
        neuralNet = net;
        Genotype gen = neuralNet.GetGenotype();
        directionsCount = gen.InputCount - 1;
        sensesDirections = new Vector3[directionsCount];
        neuralNetInput = new double[gen.InputCount];
        angleStride = 180F / (directionsCount + 1);
        netInitialized = true;
    }

    public SimulationStats ProvideSimulationStats()
    {
        return stats;
    }

    public double ProvideFitness()
    {
        return this.fitness;
    }

    public void SetFitness(double fitness)
    {
        this.fitness = fitness;
    }

    public double ProvidePickProbability()
    {
        return pickProbability;
    }

    public void SetPickProbability(double pickProbability)
    {
        this.pickProbability = pickProbability;
    }

    public void SetSimulationStats(SimulationStats stats)
    {
        this.stats = stats;
    }

    public Species ProvideSpecies()
    {
        return species;
    }

    #endregion Interface
}