using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI")]
    public Text generationTxt = null;
    public Text savedBestTrackStats = null;
    public Text trackLengthText = null;
    public Text logText = null;

    private UINetBuilder uiNetBuilder;
    private ScrollRect scroll;

    private void Awake()
    {
        uiNetBuilder = FindObjectOfType<UINetBuilder>();
        scroll = gameObject.GetComponentInChildren<ScrollRect>();
    }

    public void DrawNetUI(DNA.DnaTopology topology)
    {
        uiNetBuilder?.DrawNetUI(topology);
    }

    public void UpdateGenerationCount(int count)
    {
        if (generationTxt != null)
        {
            generationTxt.text = count.ToString() + "° generation";
        }
    }

    public void UpdateTrackLength(float length)
    {
        if (trackLengthText != null)
        {
            trackLengthText.text = "Stimated average track length: " + length.ToString("0.0");
        }
    }

    public void UpdateSavedBestStats(TrackBest best)
    {
        if (savedBestTrackStats != null)
        {
            savedBestTrackStats.text = "Current track best stats\n\nFitness: " + best.individualData.GetFitness() + "\n" + best.stats.ToString();
        }
    }

    public void AppendToLog(string text)
    {
        if (logText != null)
        {
            logText.text += "- " + text + "\n";
        }
        if (scroll != null)
        {
            scroll.velocity = new Vector2(0f, 200f);
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
}