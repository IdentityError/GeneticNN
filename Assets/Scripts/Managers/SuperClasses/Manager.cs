using UnityEngine;

public class Manager : MonoBehaviour
{
    [Header("Manager")]
    public Transform startPoint;
    public GameObject carIndividualPrefab;
    public float timeScale = 1F;
    protected GameObject finishLine;
    public GameObject track;

    protected UIManager uiManager;

    private void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
        finishLine = TUtilsProvider.GetInstance().GetFirstGameObjectInChildrenWithTag(track, "FinishLine", false);
    }

    /// <summary>
    ///   Instantiate and initialize the Neural Net of an individual, if null is passed as dna, it
    ///   will be created a Net with a random set of weights
    ///   <para> Return: the instantiated CarIndividual </para>
    /// </summary>
    protected virtual CarIndividual InstantiateAndInitializeIndividual(DNA dna, string name)
    {
        CarIndividual car = Instantiate(carIndividualPrefab, startPoint.localPosition, startPoint.localRotation).GetComponent<CarIndividual>();
        car.gameObject.name = name == null ? "Car" : name;
        DNA individualDna;
        if (dna == null)
        {
            individualDna = new DNA(car.predefinedTopology);
        }
        else
        {
            individualDna = new DNA(dna.topology, dna.weights);
        }
        car.InitializeNeuralNet(individualDna);
        return car;
    }

    protected float CalculateTrackLength()
    {
        float length = 0;
        foreach (Transform child in track.transform)
        {
            if (child.tag.Equals("TrackPart"))
            {
                length += child.localScale.z * 30;
            }
        }
        length -= TMath.Abs((finishLine.transform.localPosition - startPoint.localPosition).magnitude);
        return length;
    }

    public virtual void HasCompletedSimulation(CarIndividual individual)
    {
    }
}