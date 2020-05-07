using Assets.Scripts.Interfaces;
using Assets.Scripts.Managers;
using UnityEngine;

public class CarIndividual : MonoBehaviour, IIndividual
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
    private float[] neuralNetInput;
    private float[] neuralNetOutput;
    private bool endedSimulation = false;
    private bool netInitialized = false;
    private float angleStride;
    [HideInInspector] public Vector3 lastPosition;
    private new Rigidbody rigidbody;

    [HideInInspector] public NeuralNet neuralNet;

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
            stats.trackID = populationManager.GetTrack().name;
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
            steering = neuralNetOutput[0];  //Input.GetAxis("Horizontal");
            throttle = neuralNetOutput[1];  //Input.GetAxis("Vertical");
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
                float inversedNormalizedDistance = (hits[i].distance / length);
                neuralNetInput[i] = inversedNormalizedDistance;
                Debug.DrawRay(transform.position, hits[i].point - transform.position, Color.green);
            }
            else
            {
                neuralNetInput[i] = 1F;
                Debug.DrawRay(transform.position, sensesDirections[i] * length, Color.blue);
            }
        }
        neuralNetInput[DNA.INPUT_COUNT - 1] = throttle;
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
        return new CarIndividualData(neuralNet.dna, neuralNet.dna.fitness);
    }

    #region Interface

    public void SetPopulationManager(PopulationManager populationManager)
    {
        this.populationManager = populationManager;
    }

    public DNA ProvideDNA()
    {
        return this.neuralNet.dna;
    }

    public void SetDNA(DNA dna)
    {
        neuralNet = new NeuralNet(dna);
        directionsCount = DNA.INPUT_COUNT - 1;
        sensesDirections = new Vector3[directionsCount];
        neuralNetInput = new float[DNA.INPUT_COUNT];
        angleStride = 180F / (directionsCount + 1);
        netInitialized = true;
    }

    public SimulationStats ProvideStats()
    {
        return stats;
    }

    #endregion Interface
}