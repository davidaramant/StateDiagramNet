namespace StateDiagramCodeGen.Model
{
    public sealed class EventTransition
    {
        public string Source { get; }
        public string Destination { get; }
        public string EventName { get; }
        public string ActionName { get; }
        public string GuardName { get; }

        public EventTransition(
            string source, 
            string destination, 
            string eventName = "", 
            string actionName = "", 
            string guardName = "")
        {
            Source = source;
            Destination = destination;
            EventName = eventName;
            ActionName = actionName;
            GuardName = guardName;
        }
    }
}
