namespace StateDiagramCodeGen.PlantUMLModel

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
    
type EventTransition = 
    { 
    Source : string
    Destination : string
    EventName : string
    GuardName : string
    ActionName : string
    }
