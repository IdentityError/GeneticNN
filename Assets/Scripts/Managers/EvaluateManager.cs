using UnityEngine;
using UnityEngine.UI;

public class EvaluateManager : Manager
{
    private CarIndividualData fittestData;
    private CarIndividual fittest = null;

    private void Start()
    {
        fittestData = TSaveManager.GetInstance().LoadPersistentData(TSaveManager.CHOSEN_ONE).GetData<CarIndividualData>();
        StartSimulation();

        uiManager?.UpdateTrackLength(CalculateTrackLength());
        uiManager?.DrawNetUI(fittestData.GetDNA().topology);
        uiManager?.AppendToLog("Evaluating the current chosen one");
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