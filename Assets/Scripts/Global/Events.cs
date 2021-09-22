using Assets.Scripts.CustomBehaviour;
using GibFrame.Patterns;

public static class Events
{
    public static Event ForceSimulationEvent = new Event();
    public static Event ClearLogEvent = new Event();
    public static Event<ISimulatingOrganism, bool> IndividualEndedSimulationEvent = new Event<ISimulatingOrganism, bool>();

    public static class Queries
    {
        public static Query<Track> TrackQuery = new Query<Track>();
    }
}