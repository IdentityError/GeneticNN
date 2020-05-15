using Assets.Scripts.TWEANN;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private UIManager UIManager = null;
    private PopulationManager populationManager = null;

    private void Start()
    {
        UIManager = FindObjectOfType<UIManager>();
        populationManager = FindObjectOfType<PopulationManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            populationManager.ForceSimulationEnd();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            UIManager?.ClearLog();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
        }
    }
}