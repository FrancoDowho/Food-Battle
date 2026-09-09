using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Fusion;

public class LevelOptions : MonoBehaviour
{
    public static LevelOptions instance;
    public Sprite[] bgOptions;
    public RuleData[] rules;
    public BackgroundType bgSelected;
    public Gamemode gmSelected;
    public Dictionary<PlayerRef, PlayerGameOptions> playerOptions = new();
    public MatchConfig matchConfig;


    public enum BackgroundType
    {
        Default,
        Teatro,
        Moon
        // Agregá más si hacés crecer el array
    }

    public enum Gamemode
    {
        Default,
        DoubleDMG,
        Sensible,
        
        // Agregá más si hacés crecer el array
    }
    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        //GUARDAR EN ALGUN LADO LADO LAS OPCIONES DE LOS JUGADORES UNA VEZ QUE AMBOS ESTEN LISTOS
    }

    public LevelOptions.BackgroundType GetCurrentBackground()
    {
        return bgSelected;
    }

    public LevelOptions.Gamemode GetCurrentGamemode()
    {
        return gmSelected;
    }

    public MatchConfig CreateMatchConfig(BackgroundType bg, Gamemode gamemode, PlayerControll winner)
    {
        MatchConfig config = new MatchConfig();
        config.background = bg;
        config.gamemode = gamemode;
        config.playerWinnerLobby = winner;
        return config;

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
