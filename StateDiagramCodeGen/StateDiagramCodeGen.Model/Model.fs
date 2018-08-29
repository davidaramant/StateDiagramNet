namespace StateDiagramCodeGen.PlantUMLModel
open System.Collections.Generic

type InternalTransition = 
    { 
    StateName : string
    EventName : string
    GuardName : string
    ActionName : string 
    }

type EntryAction = 
    { 
    StateName : string
    GuardName : string
    ActionName : string 
    }

type ExitAction = 
    { 
    StateName : string
    GuardName : string
    ActionName : string 
    }
    
type ExternalTransition = 
    { 
    Source : string
    Destination : string
    EventName : string
    GuardName : string
    ActionName : string
    }
    
type State =
    {
    ShortName : string
    LongName : string
    EntryActions : List<EntryAction>
    ExitActions : List<ExitAction>
    InternalTransitions : List<InternalTransition>
    ExternalsTransitions : List<ExternalTransition>
    ChildStates : List<State>
    }