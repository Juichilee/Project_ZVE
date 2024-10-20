using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    // The unique ID for the implemented state (Refer to StateTypes.cs).
    // Helps register the instantiated state with state machine state cache
    // to optimize state retrieval.
    public abstract Enum stateType { get; }

    // Empty functions that mirror the MonoBehavior class defined for each state.
    // You can override each function in a state class that implements this.
    public virtual void Enter()
    {

    }
    
    public virtual void Execute()
    {
        
    }
    
    public virtual void FixedExecute()
    {

    }
    
    public virtual void Exit()
    {

    }
    
    public virtual void OnAnimatorMove()
    {

    }
}