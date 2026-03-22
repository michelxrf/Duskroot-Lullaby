using Fusion;
using UnityEngine;

/// <summary>
/// Allow the input data to be synced through the network
/// </summary>
public struct NetworkInputData : INetworkInput
{
    public Vector2 Move;
    public bool Aim;
    public bool Walk;
    public bool Attack;
    public bool Skill1;
    public bool Skill2;
    public bool Interact;
}
