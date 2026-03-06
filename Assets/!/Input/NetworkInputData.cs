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
}
