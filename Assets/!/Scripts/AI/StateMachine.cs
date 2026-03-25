using UnityEngine;
using Fusion;


public class StateMachine : NetworkBehaviour
{
    [SerializeField] State initialState;
    State currentState;

    public void ChangeState(State newState)
    {
        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        if (currentState != null)
            currentState.Enter();
    }

    public override void Spawned()
    {
        if (!HasStateAuthority) return;

        ChangeState(initialState);
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        currentState.Process();
    }
}
