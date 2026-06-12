using Fusion;
using UnityEngine;

/// <summary>
/// Allow the input data to be synced through the network
/// </summary>
public struct NetworkInputData : INetworkInput
{
    public Vector2 Move;
    public Vector2 Look;
    public bool Aim;
    public bool Walk;
    public bool Attack;
    public bool Skill1;
    public bool Skill2;
    public bool Interact;
    public bool Dash;
    public bool DebugRevive;
    public bool Emote0;
    public bool Emote1;
    public bool Emote2;
    public bool Emote3;
}
