using UnityEngine;
using Fusion;


public class StateMachine : NetworkBehaviour
{
    [SerializeField] State initialState;
    public State currentState;
    bool isChangingStates = false;

    public void ChangeState(State newState)
    {
        isChangingStates = true;

        if (currentState != null)
            currentState.Exit();

        currentState = newState;
        if (currentState != null)
            currentState.Enter();

        isChangingStates = false;
    }

    public override void Spawned()
    {
        if (!HasStateAuthority) return;

        ChangeState(initialState);
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        if(!isChangingStates)
            currentState.Process();
    }
}
