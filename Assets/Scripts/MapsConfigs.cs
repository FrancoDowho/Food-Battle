using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct MapsConfigs
{
    public LevelOptions.BackgroundType background;
    public GameObject[] maps;
    //Any other configuration that we want to add for the maps, like gamemode/gravity or something else

}
