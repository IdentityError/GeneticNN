using UnityEngine;

public class InputManager : MonoBehaviour
{
    private TrainingManager trainingManager = null;

    private UIManager UIManager = null;

    private void Start()
    {
        UIManager = FindObjectOfType<UIManager>();
        trainingManager = FindObjectOfType<TrainingManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            trainingManager?.EndAndRestartNewSimulation();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            UIManager?.ClearLog();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            trainingManager?.SaveCurrentBest();
        }
    }
}