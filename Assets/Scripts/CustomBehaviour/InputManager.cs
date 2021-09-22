using UnityEngine;

public class InputManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Events.ForceSimulationEvent.Broadcast();
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Events.ClearLogEvent.Broadcast();
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
        }
    }
}