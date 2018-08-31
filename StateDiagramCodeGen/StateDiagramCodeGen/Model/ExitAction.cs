namespace StateDiagramCodeGen.Model
{
    public sealed class ExitAction
    {
        public string StateName { get; }
        public string GuardName { get; }
        public string ActionName { get; }

        public ExitAction(
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
