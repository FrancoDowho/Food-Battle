using Fusion;
using Fusion.Sockets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class LobbyReadyManager : NetworkBehaviour
{
    int _minimumPlayerReady = 2;
    public Action ReadyMethod = delegate { };
    public static LobbyReadyManager Instance { get; private set; }
    [SerializeField] GameObject _lobbyVisual;
    [SerializeField] RulesSelectedScreen _rulesSelectedScreen;
    Sequence _goToGameSequence;
    [SerializeField] Image _fadeImage;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    
    public void CheckPlayersReady(List<PlayerControll> playersToCheck)
    {
        int playersReady = 0;
        foreach (var player in playersToCheck)
        {
            if (player.isReady)
                playersReady++;
            
        }
        if (playersReady >= _minimumPlayerReady && playersReady >= playersToCheck.Count)
        {
            //Empieza el juego
            Debug.Log("Listos");
            RPC_OnBothPlayersReady();
        }


        //FaltaLaLógica
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_OnBothPlayersReady()
    {
        ReadyMethod?.Invoke();
    }


    // Start is called before the first frame update
    void Start()
    {                                   
        
    }
    public void ConnectedToLobby()
    {
        _lobbyVisual.SetActive(true);
    }


    public void DisplayRules(PlayerControll winnerPlayer)
    {
        Debug.Log($"DisplayRules -> winnerPlayer null? {winnerPlayer == null}");
        Debug.Log($"DisplayRules -> LevelOptions.instance null? {LevelOptions.instance == null}");

        if (LevelOptions.instance != null)
        {
            Debug.Log($"DisplayRules -> rules null? {LevelOptions.instance.rules == null}");
            Debug.Log($"DisplayRules -> rules length: {LevelOptions.instance.rules.Length}");
        }

        _rulesSelectedScreen.gameObject.SetActive(true);
        RuleData ruleData = LevelOptions.instance.rules[winnerPlayer.selectedGamemodeIndex];
        _rulesSelectedScreen.SetRules(LevelOptions.instance.bgOptions[winnerPlayer.selectedBackgroundIndex],
           ruleData.ruleSprite
            ,ruleData.ruleName
            , ruleData.description);

        ReadyMethod = BeginMatch;
    }

    public void BeginMatch()
    {
        _goToGameSequence?.Kill();
        _goToGameSequence = DOTween.Sequence();
        _goToGameSequence.Append(_fadeImage.DOFade(1, 2f));
        if (!Runner.IsSceneAuthority)
            return;
        _goToGameSequence.AppendCallback(() => {
            Runner.LoadScene(SceneRef.FromIndex(1), LoadSceneMode.Additive);
        });
        

    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            PlayerControll winnerPlayer = BasicSpawner.instance.playersControlls[0];
            MatchConfig config = LevelOptions.instance.CreateMatchConfig((LevelOptions.BackgroundType)2, (LevelOptions.Gamemode)2, winnerPlayer);
            LevelOptions.instance.matchConfig = config;

            BeginMatch();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]

    public void RPC_AmountOfPlayers(int players)
    {
        StartCoroutine(SendEvent(players));
    }

    IEnumerator SendEvent(int players)
    {
        yield return new WaitForSeconds(0.5f);
        EventManager.TriggerEvent(EventManager.EventsType.Event_PlayersJoined, players);

    }
}
