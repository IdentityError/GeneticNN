using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrackBest
{
    public SimulationStats stats;
    public CarIndividualData individualData;

    public TrackBest(SimulationStats stats, CarIndividualData individualData)
    {
        this.stats = stats;
        this.individualData = individualData;
    }

    public bool BetterThan(TrackBest other)
    {
        if (stats.trackID != other.stats.trackID)
        {
            return false;
        }
        return stats.BetterThan(other.stats);
    }

    public override string ToString()
    {
        return stats.ToString();
    }
}