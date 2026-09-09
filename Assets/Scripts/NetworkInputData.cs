using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public const byte MOUSEBUTTON0 = 0;

    public NetworkButtons buttons;
    public Vector2 aimDirection;
    public int BombTimer;
    public int WeaponSelected;
}