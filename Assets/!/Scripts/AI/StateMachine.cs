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

        EnemySetup enemySetup = GetComponent<EnemySetup>();

        if (enemySetup.IsInitialized())
        {
            ChangeState(initialState);
        }
        else
        {
            enemySetup.OnInit += () => ChangeState(initialState);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!HasStateAuthority) return;

        if(!isChangingStates)
            currentState.Process();
    }

    private void OnDisable()
    {
        State[] states = GetComponents<State>();
        foreach (var state in states)
        {
            state.enabled = false;
        }
    }

    private void OnEnable()
    {
        State[] states = GetComponents<State>();
        foreach (var state in states)
        {
            state.enabled = true;
        }
    }
}
