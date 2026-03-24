using System;
using UnityEngine;
using Fusion;


public abstract class State : NetworkBehaviour
{
    protected StateMachine stateMachine;
    public abstract void Enter();
    public abstract void Exit();
    public abstract void Process();
}

