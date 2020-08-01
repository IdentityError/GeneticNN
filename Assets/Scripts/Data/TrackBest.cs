[System.Serializable]
public class TrackBest
{
    public SimulationStats stats;
    public IndividualData individualData;

    public TrackBest()
    {
        stats = null;
        individualData = null;
    }

    public TrackBest(SimulationStats stats, IndividualData individualData)
    {
        this.stats = stats;
        this.individualData = individualData;
    }

    public bool BetterThan(TrackBest other)
    {
        if (other is null)
        {
            return true;
        }

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