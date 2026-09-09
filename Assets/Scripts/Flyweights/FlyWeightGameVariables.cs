using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyWeightGameVariables : MonoBehaviour
{
    public static FlyWeightGameVariables instance;
    [Header("FallDmgVariables")]
    public float fallThreshold;
    public float fallDmgMultiplier;
    public float dmgMultiplier = 1;
    public bool isSensibleModeActive;
    // Start is called before the first frame update

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        EventManager.SubscribeToEvent(EventManager.EventsType.Event_ResetDefaultGamemode, ResetVariables);
        EventManager.SubscribeToEvent(EventManager.EventsType.Event_SetSensibleGamemode, ActivateSensibleMode);

    }
    void ActivateSensibleMode(params object[] paramaters)
    {
               isSensibleModeActive = true;
    }

    void ResetVariables(params object[] parameters)
    {
        dmgMultiplier = 1;
    }
    
}
