[System.Serializable]
public class TrackBest
{
    public SimulationStats stats;
    public CarIndividualData individualData;

    public TrackBest()
    {
        stats = null;
        individualData = null;
    }

    public TrackBest(SimulationStats stats, CarIndividualData individualData)
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