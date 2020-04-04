[System.Serializable]
public class SimulationStats
{
    public float averageThrottle;
    public float time;
    public float distance;
    public string trackID;

    public SimulationStats(float averageThrottle, float time, float distance, string track)
    {
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
        return this.time < other.time;
    }
}