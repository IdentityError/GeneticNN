using System.Collections.Generic;

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

    public TrackBest GetFittestTrackBest()
    {
        float max = 0;
        TrackBest current = null;
        for (int i = 0; i < tracksStats.Count; i++)
        {
            if (tracksStats[i].individualData.GetFitness() > max)
            {
                current = tracksStats[i];
                max = tracksStats[i].individualData.GetFitness();
            }
        }
        return current;
    }
}