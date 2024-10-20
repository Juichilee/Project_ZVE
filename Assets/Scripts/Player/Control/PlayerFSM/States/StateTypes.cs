/* 
    Consider each StateType enum to be a separate state machine.
    Defining items within an enum serves to add a unique state to the state machine.
    The enums items are equivalent to a unique ID for each state that
    are used to register and cache state instances within a state machine. 
 */

public enum MotionStateType
{
    Idle,
    Run,
    JumpAir
}

public enum ActionStateType
{
    Idle,
    Pickup,
    Interact,
    Attack
}