using UnityEngine;
using UnityEngine.UI;

public class EvaluateManager : Manager
{
    [Header("UI")]
    public Text fitnessTxt;
    public Text topologyTxt;

    private CarIndividualData fittestData;
    private CarIndividual fittest = null;

    private void Start()
    {
        fittestData = TSaveManager.GetInstance().LoadPersistentData(TSaveManager.FITTEST_DATA).GetData<CarIndividualData>();
        StartSimulation();

        fitnessTxt.text = "Fitness: " + fittestData.GetFitness();
        topologyTxt.text = "<b>Topology</b>\nInput count: " + DNA.INPUT_COUNT +
                            "\nHidden layers: " + fittestData.GetDNA().topology.hiddenLayerCount +
                            "\nNeurons per hidden layer: " + fittestData.GetDNA().topology.neuronsPerHiddenLayer +
                            "\nOutput count: " + DNA.OUTPUT_COUNT;
    }

    private void Update()
    {
        Time.timeScale = timeScale;
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartSimulation();
        }
    }

    private void StartSimulation()
    {
        if (fittest != null)
        {
            Destroy(fittest.gameObject);
        }

        InstantiateAndInitializeIndividual(fittestData.GetDNA(), null);
    }

    protected override CarIndividual InstantiateAndInitializeIndividual(DNA dna, string name)
    {
        CarIndividual newCar = base.InstantiateAndInitializeIndividual(dna, name);
        newCar.manager = null;
        return newCar;
    }
}