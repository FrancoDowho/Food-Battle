using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchBeginnerManager : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Spawned()
    {
        base.Spawned();
        BasicSpawner.instance.EnableGameControls();

        if (!HasStateAuthority)
            return;
        Debug.Log("El startSeLLAMA");
        StartCoroutine(CorroutineCheckToStart());
    }

    IEnumerator CorroutineCheckToStart()
    {
        while (!BasicSpawner.instance.CheckIfThereIsEnoughPlayersToStart())
        {
            yield return new WaitForSeconds(0.5f);
            Debug.Log("Esperando jugadores para arrancar el juego...");
        }
        Debug.Log("DALE ARRANCA");
        StartGameFlow();
    }

    void StartGameFlow()
    {
        EventManager.TriggerEvent(EventManager.EventsType.Event_ResetDefaultGamemode);
        //int mapToChose = UnityEngine.Random.Range(0, levelSelectorManager.instance.maps.Length);
        MatchConfig configuration = LevelOptions.instance.matchConfig;
        levelSelectorManager.instance.SpawnMap(configuration.background);
        TurnManager.instance.BeginGame();

        if (configuration.gamemode == LevelOptions.Gamemode.DoubleDMG)
        {
            FlyWeightGameVariables.instance.dmgMultiplier = 2f;
        }
        if (configuration.gamemode == LevelOptions.Gamemode.Sensible)
        {
            EventManager.TriggerEvent(EventManager.EventsType.Event_SetSensibleGamemode);
        }
          
    }
    
   

    // Update is called once per frame
    void Update()
    {
        
        
    }
}
