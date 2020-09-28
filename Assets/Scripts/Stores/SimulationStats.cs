using Assets.Scripts.CustomBehaviour;
using System;
using UnityEngine;

[System.Serializable]
public class SimulationStats
{
    public float averageThrottle;
    public float distance;
    public float time;
    public float lastThrottle;
    public Track track;

    private int cycles;
    private Vector3 lastPosition;
    private float throttleSum;

    private SimulationStats(Track track)
    {
        this.track = track;
        cycles = 0;
        throttleSum = 0F;
    }

    public SimulationStats(float averageThrottle, float time, float distance, Track track) : this(track)
    {
        this.averageThrottle = averageThrottle;
        this.time = time;
        this.distance = distance;
        this.lastThrottle = 0;
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

        distance = throttle > 0 ? distance + Vector3.Distance(position, lastPosition) : distance - Vector3.Distance(position, lastPosition);

        distance = distance > track.Length() ? track.Length() : distance;
        lastThrottle = throttle;
        lastPosition = position;

        time += Time.deltaTime;
        cycles++;
        throttleSum += throttle;
        averageThrottle = throttleSum / cycles;
        averageThrottle = averageThrottle < 0 ? 0 : averageThrottle;

        lastPosition = position;
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
        return "Track: " + track +
             "\nAverage throttle: " + averageThrottle +
             "\nTime: " + time +
             "\nDistance: " + distance;
    }
}