using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI")]
    public Text generationTxt = null;
    public Text logText = null;
    public Text savedBestTrackStats = null;
    public Text trackLengthText = null;
    private ScrollRect scroll;

    public void AppendToLog(string text)
    {
        if (logText != null)
        {
            logText.text += "- " + text + "\n";
        }
        if (scroll != null)
        {
            scroll.velocity = new Vector2(0f, 1000f);
        }
    }

    public void ClearLog()
    {
        if (logText != null)
        {
            logText.text = "";
        }
        if (scroll != null)
        {
            scroll.velocity = new Vector2(0f, -5000f);
        }
    }

    public void UpdateGenerationCount(int count)
    {
        if (generationTxt != null)
        {
            generationTxt.text = count.ToString() + "° generation";
        }
    }

    public void UpdateTextBox1(string text)
    {
        if (savedBestTrackStats != null)
        {
            savedBestTrackStats.text = text;
        }
    }

    public void UpdateTrackLength(float length)
    {
        if (trackLengthText != null)
        {
            trackLengthText.text = "Stimated track length: " + length.ToString("0.0");
        }
    }

    private void Awake()
    {
        scroll = gameObject.GetComponentInChildren<ScrollRect>();
    }
}