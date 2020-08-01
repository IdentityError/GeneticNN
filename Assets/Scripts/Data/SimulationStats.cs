using System;
using UnityEngine;

[System.Serializable]
public class SimulationStats
{
    public float averageThrottle;
    public float distance;
    public float time;
    public string trackID;

    private int cycles;
    private Vector3 lastPosition;
    private float throttleSum;

    public SimulationStats(string track)
    {
        this.trackID = track;
        cycles = 0;
        throttleSum = 0F;
    }

    public SimulationStats(float averageThrottle, float time, float distance, string track) : this(track)
    {
        this.averageThrottle = averageThrottle;
        this.time = time;
        this.distance = distance;
    }

    /// <summary>
    ///   Compare two different simulation stats
    /// </summary>
    /// <param name="other"> </param>
    /// <returns> </returns>
    [Obsolete]
    public bool BetterThan(SimulationStats other)
    {
        if (other == null)
        {
            return true;
        }
        return this.distance > other.distance && this.averageThrottle > other.averageThrottle;
    }

    /// <summary>
    ///   Resets the stats values
    /// </summary>
    public void Reset()
    {
        averageThrottle = 0;
        time = 0;
        distance = 0;
    }

    /// <summary>
    ///   Override of the ToString function
    /// </summary>
    /// <returns> </returns>
    public override string ToString()
    {
        return "Track: " + trackID +
             "\nAverage throttle: " + averageThrottle +
             "\nTime: " + time +
             "\nDistance: " + distance;
    }

    /// <summary>
    ///   Update the simulation stats, <b> Call this function every update cycle </b>
    /// </summary>
    /// <param name="throttle"> </param>
    /// <param name="position"> </param>
    public void Update(float throttle, Vector3 position)
    {
        if (cycles == 0)
        {
            lastPosition = position;
        }

        if (throttle > 0)
        {
            distance += Vector3.Distance(position, lastPosition);
        }
        lastPosition = position;

        time += Time.deltaTime;
        cycles++;
        throttleSum += throttle;
        averageThrottle = throttleSum / cycles;
        averageThrottle = averageThrottle < 0 ? 0 : averageThrottle;

        lastPosition = position;
    }
}