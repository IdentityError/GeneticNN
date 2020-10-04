using Assets.Scripts.CustomBehaviour;
using Assets.Scripts.MachineLearning.TWEANN;
using Assets.Scripts.TUtils.ObjectPooling;
using Assets.Scripts.TUtils.Utils;
using System;
using UnityEngine;

public class CarIndividual : MonoBehaviour, ISimulatingOrganism, IPooledObject
{
    [HideInInspector] public Vector3 lastPosition;
    [Header("Senses")]
    [Tooltip("Length of the raycasted senses")]
    public float length;
    public bool manualControl;
    public float motorPower;
    [Tooltip("Length from left to right axes")]
    public float rearTrack;
    public SimulationStats stats;
    [Header("Debug")]
    public float steering;
    public float steeringPower;
    public float throttle;
    [Tooltip("Radius of the turn")]
    public float turnRadius;
    [Tooltip("Length from rear to front axes")]
    public float wheelBase;
    [Header("Specifications")]
    public Wheel[] wheels;
    private float ackermanAngleLeft;
    private float ackermanAngleRight;
    private float angleStride;
    private int directionsCount;
    [SerializeField] private bool endedSimulation = false;
    [SerializeField] private double fitness = 0;
    [SerializeField] private double rawFitness = 0;
    [SerializeField] private bool netInitialized = false;
    private NeuralNetwork neuralNet;
    private double[] neuralNetInput;
    private double[] neuralNetOutput;
    private PopulationManager populationManager;
    private new Rigidbody rigidbody;
    private Vector3[] sensesDirections;

    private float prob;

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
            stats.track = populationManager.GetTrack();
            populationManager.SubscribeToForceEnded(OnForceEnded);
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
            endedSimulation = true;
            SetRawFitness(EvaluateFitnessFunction());
            populationManager?.IndividualEndedSimulation(this, false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("FinishLine"))
        {
            endedSimulation = true;
            SetRawFitness(EvaluateFitnessFunction());
            populationManager?.IndividualEndedSimulation(this, throttle > 0);
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
                Debug.DrawRay(transform.position, sensesDirections[i] * length, Color.red);
            }
        }
        neuralNetInput[neuralNet.GetGenotype().InputCount - 1] = throttle;
    }

    private void UpdateStats()
    {
        stats.Update(throttle, transform.position);
    }

    private void ResetParameters()
    {
        endedSimulation = true;
        netInitialized = false;

        lastPosition = transform.position;

        throttle = 0;
        steering = 0;
        fitness = 0;
        rawFitness = 0;

        stats.Reset();
    }

    private void OnForceEnded()
    {
        if (IsSimulating())
        {
            endedSimulation = true;
        }
        SetRawFitness(EvaluateFitnessFunction());
    }

    #region Interface

    public double EvaluateFitnessFunction()
    {
        float length = populationManager.GetTrack().Length();
        double fitness = 3D * (stats.distance) * Math.Pow(stats.averageThrottle + 1D, stats.distance / length);
        if (fitness > 0) return fitness;
        else return 0;
    }

    public bool IsSimulating()
    {
        return !endedSimulation;
    }

    public void OnObjectSpawn()
    {
        ResetParameters();
    }

    public double ProvideAdjustedFitness()
    {
        return this.fitness;
    }

    public NeuralNetwork ProvideNeuralNet()
    {
        return this.neuralNet;
    }

    public SimulationStats ProvideSimulationStats()
    {
        return stats;
    }

    public void AdjustFitness(double fitness)
    {
        this.fitness = fitness;
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
        endedSimulation = false;
    }

    public void SetPopulationManager(PopulationManager populationManager)
    {
        this.populationManager = populationManager;
    }

    public double ProvideRawFitness()
    {
        return rawFitness;
    }

    public void SetRawFitness(double rawFitness)
    {
        this.rawFitness = rawFitness;
    }

    public float ProvideSelectProbability()
    {
        return prob;
    }

    public void SetSelectProbability(float prob)
    {
        this.prob = prob;
    }

    #endregion Interface
}