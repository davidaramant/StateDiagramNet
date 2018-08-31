namespace StateDiagramCodeGen.Model
{
    public sealed class EntryAction
    {
        public string StateName { get; }
        public string GuardName { get; }
        public string ActionName { get; }

        public EntryAction(
            string stateName,
            string guardName,
            string actionName)
        {
            StateName = stateName;
            GuardName = guardName;
            ActionName = actionName;
        }
    }
}
