using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine<TStateType, TStateInterface>
    where TStateInterface : BaseState
    where TStateType : Enum
{
    public TStateInterface currentState { get; private set; }
    private Dictionary<TStateType, TStateInterface> stateCache;

    public StateMachine()
    {
        stateCache = new Dictionary<TStateType, TStateInterface>();
    }

    // Registers a state with its corresponding state type to the state cache.
    public void RegisterState(TStateType stateType, TStateInterface state)
    {
        if (stateCache.ContainsKey(stateType))
        {
            throw new ArgumentException($"State {stateType} is already registered.");
        }
        stateCache[stateType] = state;
    }

    // Changes the current state to the specified state type.
    public void ChangeState(TStateType newStateType)
    {
        if (!stateCache.TryGetValue(newStateType, out TStateInterface newState))
        {
            throw new ArgumentException($"State {newStateType} is not registered.");
        }

        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    // Calls the Execute method of the current state.
    public void Update()
    {
        currentState?.Execute();
    }

    // Calls the FixedExecute method of the current state.
    public void FixedUpdate()
    {
        currentState?.FixedExecute();
    }

    public void OnAnimatorMove()
    {
        currentState?.OnAnimatorMove();
    }
}
