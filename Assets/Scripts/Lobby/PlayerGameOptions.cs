using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerGameOptions
{
    public LevelOptions.Gamemode mode;
    public LevelOptions.BackgroundType background;

    public PlayerGameOptions(LevelOptions.Gamemode mode, LevelOptions.BackgroundType background)
    {
        this.mode = mode;
        this.background = background;
    }
}
