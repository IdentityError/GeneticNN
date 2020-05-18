[System.Serializable]
public class SimulationStats
{
    public float averageThrottle;
    public float lastThrottle;
    public float time;
    public float distance;
    public string trackID;

    public SimulationStats(float averageThrottle, float time, float distance, float lastThrottle, string track)
    {
        this.lastThrottle = lastThrottle;
        this.averageThrottle = averageThrottle;
        this.time = time;
        this.distance = distance;
        this.trackID = track;
    }

    public override string ToString()
    {
        return "Track: " + trackID +
             "\nAverage throttle: " + averageThrottle +
             "\nTime: " + time +
             "\nDistance: " + distance;
    }

    public bool BetterThan(SimulationStats other)
    {
        if (other == null)
        {
            return true;
        }
        return this.time < other.time && this.averageThrottle > other.averageThrottle;
    }
}