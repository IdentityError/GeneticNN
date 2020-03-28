using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TracksStats
{
    private List<TrackBest> tracksStats = null;

    public TracksStats()
    {
        if (tracksStats == null)
        {
            tracksStats = new List<TrackBest>();
        }
    }

    public void AddOrReplace(TrackBest trackBest)
    {
        if (tracksStats == null)
        {
            tracksStats = new List<TrackBest>();
        }
        for (int i = 0; i < tracksStats.Count; i++)
        {
            if (tracksStats[i].stats.trackID.Equals(trackBest.stats.trackID))
            {
                if (trackBest.BetterThan(tracksStats[i]))
                {
                    tracksStats[i] = trackBest;
                    return;
                }
            }
        }
        tracksStats.Add(trackBest);
    }

    public TrackBest Get(string trackID)
    {
        for (int i = 0; i < tracksStats.Count; i++)
        {
            if (tracksStats[i].stats.trackID.Equals(trackID))
            {
                return tracksStats[i];
            }
        }
        return null;
    }
}