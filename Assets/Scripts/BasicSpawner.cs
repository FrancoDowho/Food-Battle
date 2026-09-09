using Fusion;
using Fusion.Sockets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BasicSpawner : SimulationBehaviour , INetworkRunnerCallbacks
{
    NetworkRunner _runner;
    public static BasicSpawner instance;
    [SerializeField] NetworkPrefabRef _playerPrefab;
    Dictionary<PlayerRef, NetworkObject> _playersSpawned = new Dictionary<PlayerRef, NetworkObject>();
    public PlayerControll localPlayer;
    public List<PlayerControll> playersControlls = new List<PlayerControll>();
    //[Networked] private NetworkLinkedList<PlayerRef> SyncedPlayers => default;

    public Action SceneLoadDoneMethod = delegate { };

    private int _currentCount = 0;
    bool _mouseButton0;
   
    int _curTimeBomb;
    int _weaponSelected;
    int _amountOfPlayerToStartGame = 2;

    Action currentControls = delegate { };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    #region Networking 
    public void OnConnectedToServer(NetworkRunner runner)
    {
        
    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        PopUpManager.instance.ActivePopUp()
           .SetAmountOfButtons(1)
           .SetTitle("No se ha podido entrar a la sala")
           .SetDesc("Es posible que la sala esté llena o ya no esté disponible")
           .SetButtonText(0, "Volver al menu")
           .SetButtonAction(0, () =>
           {
               PopUpManager.instance.ClosePopUp();
               LeaveMatchManager.Instance.ForceReturnToMenu();

           });
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {
    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {
    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {
    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        data.buttons.Set(NetworkInputData.MOUSEBUTTON0, _mouseButton0);
        data.BombTimer = _curTimeBomb;
        data.WeaponSelected = _weaponSelected;

        if (Mira.instance != null)
            data.aimDirection = new Vector2(Mira.instance.rotation.x, Mira.instance.rotation.y);
        else
            data.aimDirection = Vector2.zero;

        input.Set(data);

        if (Input.GetMouseButtonUp(0))
            _mouseButton0 = false;
    }

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
    }

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {
       
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        LobbyReadyManager.Instance.ConnectedToLobby();

        if (runner.IsServer)
        {
            Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.PlayerCount) * 3, 1, 0);

            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            runner.SetPlayerObject(player, networkPlayerObject);

            _playersSpawned.Add(player, networkPlayerObject);

            PlayerControll playerSpawned = networkPlayerObject.GetComponent<PlayerControll>();
            playerSpawned.playerID = _playersSpawned.Count;
           // LobbyReadyManager.Instance.RPC_AmountOfPlayers(runner.ActivePlayers.Count());

            if (!playersControlls.Contains(playerSpawned))
                playersControlls.Add(playerSpawned);

            /*if(playerSpawned.HasInputAuthority)
            {
                localPlayer = playerSpawned;
            }*/



            //ACA ESTÁ TODO LO QUE ES PARA EMPEZAR EL NIVELLLLLLLLLLLLL
            /*
            if (_playersSpawned.Count == 1)
            {
                int mapToChose = UnityEngine.Random.Range(0, levelSelectorManager.instance.maps.Length);
                levelSelectorManager.instance.SpawnMap(mapToChose);
               
            }
            if (_playersSpawned.Count == _amountOfPlayerToStartGame)
            {
                TurnManager.instance.BeginGame();
                
               
                
            }*/
        }
        //ProfilePicManager.instance.ConnectedToLobby();


    }



    /*public override void Spawned()
    {
        base.Spawned();

        if (HasStateAuthority)
        {
            if (!SyncedPlayers.Contains(Object.InputAuthority))
                SyncedPlayers.Add(Object.InputAuthority);
        }

        UpdateLocalList();
    }*/



    /*private void UpdateLocalList()
    {
        playersControlls.Clear();

        foreach (var playerRef in SyncedPlayers)
        {
            var obj = Runner.GetPlayerObject(playerRef);
            if (obj != null && obj.TryGetComponent(out PlayerControll pc))
            {
                playersControlls.Add(pc);
            }
        }
    }*/

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_playersSpawned.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _playersSpawned.Remove(player);
        }

        EventManager.TriggerEvent(EventManager.EventsType.Event_PlayersLeft, _playersSpawned.Count);

    }



    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {
    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {
    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {
       /* if (!runner.IsServer)
            return;
        SceneLoadDoneMethod?.Invoke();
        SceneLoadDoneMethod = delegate { };
        Debug.Log("HOLAHOLAHOLAMELLAME");
        Debug.Log("HOLAHOLAHOLAMELLAME");
        Debug.Log("HOLAHOLAHOLAMELLAME");
        Debug.Log("HOLAHOLAHOLAMELLAME");*/
    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {
    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {
        PopUpManager.instance.ActivePopUp()
            .SetAmountOfButtons(1)
            .SetTitle("Fin de conexión")
            .SetDesc("El host ha salido de la partida")
            .SetButtonText(0, "Volver al menu")
            .SetButtonAction(0, () =>
            {
                PopUpManager.instance.ClosePopUp();
                LeaveMatchManager.Instance.ForceReturnToMenu();

            });


    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {
    }
#endregion

    async void StartGame(GameMode mode)
    {
        _runner = gameObject.AddComponent<NetworkRunner>();

        LeaveMatchManager.Instance.SetRunner(_runner);

        _runner.ProvideInput = true;
        
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);

        var sceneInfo = new NetworkSceneInfo();

        if (scene.IsValid)
         {
             sceneInfo.AddSceneRef(scene , LoadSceneMode.Additive);
         }
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "Level",
            Scene = scene,
            PlayerCount = 2,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            
        });
        //EL OG
        /*await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "Level",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });*/
    }

   
    void OnGUI()
    {
        if (_runner == null)
        {
            /*if (GUI.Button(new Rect(0, 0, 200, 40), "Host"))
            {
                StartGame(GameMode.Host);
                _amountOfPlayerToStartGame = 2;
            }
            if (GUI.Button(new Rect(0, 40, 200, 40), "Join"))
            {
                StartGame(GameMode.Client);
                
            }
            if (GUI.Button(new Rect(0, 80, 200, 40), "TestingMode"))
            {
                StartGame(GameMode.Host);
                _amountOfPlayerToStartGame = 1;


            }*/
        }
    }

    public void JoinServer(bool asHost)
    {
        if (asHost)
        {
            StartGame(GameMode.Host);
        }
        else StartGame(GameMode.Client);


    }

    public void EnableGameControls(bool Enable = true)
    {
        _mouseButton0 = false;
        _curTimeBomb = 3;
        _weaponSelected = 0;
        if (Enable)
        currentControls = InGameControls;
        else currentControls = delegate { };
    }

    private void Update()
    {
        currentControls();
        //_mouseButton0 = _mouseButton0 | Input.GetMouseButton(0);
       
    }

    public void RegisterPlayer(PlayerControll player)
    {
        if (player == null)
            return;

        if (!playersControlls.Contains(player))
            playersControlls.Add(player);
    }

    public void UnregisterPlayer(PlayerControll player)
    {
        if (player == null)
            return;

        if (playersControlls.Contains(player))
            playersControlls.Remove(player);
    }

    public List<PlayerControll> GetPlayersControlls()
    {
        return playersControlls;
    }   

    void InGameControls()
    {
        if (Input.GetMouseButtonDown(0)) _mouseButton0 |= true;

        if (Input.GetMouseButtonUp(0)) _mouseButton0 = false;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f)
        {
            if (_weaponSelected + 1 >= Flyweight_WeaponsPre.instance.weaponsPrefs.Length)
                _weaponSelected = 0;
            else _weaponSelected++;

        }
        else if (scroll < 0f)
        {
            if (_weaponSelected - 1 < 0)
                _weaponSelected = Flyweight_WeaponsPre.instance.weaponsPrefs.Length - 1;
            else _weaponSelected--;
        }




        if (Input.GetKeyDown(KeyCode.Alpha1))
            _curTimeBomb = 1;
        if (Input.GetKeyDown(KeyCode.Alpha2))
            _curTimeBomb = 2;
        if (Input.GetKeyDown(KeyCode.Alpha3))
            _curTimeBomb = 3;
        if (Input.GetKeyDown(KeyCode.Alpha4))
            _curTimeBomb = 4;
        if (Input.GetKeyDown(KeyCode.Alpha5))
            _curTimeBomb = 5;
    }

   
    public bool CheckIfThereIsEnoughPlayersToStart()
    {
        
        if (_playersSpawned.Count >= _amountOfPlayerToStartGame)
        {
           return true;
        }
        return false;
    }
    
}
