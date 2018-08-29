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
    
type Transition = 
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
    InternalTransitions : List<InternalTransition>
    EntryActions : List<EntryAction>
    ExitActions : List<ExitAction>
    Transitions : List<Transition>
    ChildStates : List<State>
    }