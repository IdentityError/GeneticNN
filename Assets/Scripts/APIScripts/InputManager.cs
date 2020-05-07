using UnityEngine;

public class InputManager : MonoBehaviour
{
    private UIManager UIManager = null;

    private void Start()
    {
        UIManager = FindObjectOfType<UIManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
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