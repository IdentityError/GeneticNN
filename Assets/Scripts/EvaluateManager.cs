using UnityEngine;
using UnityEngine.UI;

public class EvaluateManager : MonoBehaviour
{
    [Header("References")]
    public GameObject carPrefab;
    [Header("UI")]
    public Text fitnessTxt;
    public Text topologyTxt;

    private CarIndividualData fittestData;
    private CarIndividual fittest = null;

    private void Start()
    {
        fittestData = SaveManager.GetInstance().LoadPersistentData(SaveManager.FITTEST_DATA).GetData<CarIndividualData>();
        StartSimulation();

        fitnessTxt.text = "Fitness: " + fittestData.GetFitness();
        topologyTxt.text = "<b>Topology</b>\nInput count: " + fittestData.GetDNA().topology.inputCount +
                            "\nHidden layers: " + fittestData.GetDNA().topology.hiddenLayerCount +
                            "\nNeurons per hidden layer: " + fittestData.GetDNA().topology.neuronsPerHiddenLayer +
                            "\nOutput count: " + fittestData.GetDNA().topology.outputCount;
    }

    private void Update()
    {
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

        fittest = Instantiate(carPrefab, new Vector3(0F, 0.75F, 0F), Quaternion.identity).GetComponent<CarIndividual>();
        fittest.InitializeNeuralNet(fittestData.GetDNA());
    }
}