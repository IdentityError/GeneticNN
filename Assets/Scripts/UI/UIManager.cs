using System.Collections;
using System.Collections.Generic;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            ClearLog();
        }
    }

    public void DrawNetUI(DNA.DnaTopology topology)
    {
        uiNetBuilder?.DrawNetUI(topology);
    }

    public void UpdateGenerationCount(int count)
    {
        generationTxt.text = count.ToString() + "° generation";
    }

    public void UpdateTrackLength(float length)
    {
        trackLengthText.text = "Stimated average track length: " + length.ToString("0.0");
    }

    public void UpdateSavedBestStats(TrackBest best)
    {
        savedBestTrackStats.text = "Current track best stats\n\nFitness: " + best.individualData.GetFitness() + "\n" + best.stats.ToString();
    }

    public void AppendToLog(string text)
    {
        logText.text += "- " + text + "\n";
        scroll.velocity = new Vector2(0f, 200f);
    }

    public void ClearLog()
    {
        logText.text = "";
        scroll.velocity = new Vector2(0f, -5000f);
    }
}