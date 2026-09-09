using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerControll : NetworkBehaviour
{
    ChangeDetector _changeDetector;

    bool _startedCharging;
    
    [SerializeField] float _maximumHoldSeconds;
    float _holdedSeconds;
    [HideInInspector] public int WeaponEquip;


    [Networked] int _bombSeconds { get; set; }
    [Networked] public bool itsMyTurn { get; set; }

    [Networked] public int playerID { get; set; }
    [Networked] public int playerNameIndex { get; set; }
    [Networked] public int playerpfpIndex { get; set; }
    float multiplierTest;

    //Lobby stuff
    [Networked] public bool isReady { get; private set; }
    [Networked] public int selectedBackgroundIndex { get; set; }
    [Networked] public int selectedGamemodeIndex { get; set; }

    float _visualHoldedSeconds;

    [Networked] public Vector2 aimDirection { get; set; }
    public override void Spawned()
    {
        base.Spawned();
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        if (playerID == 1)
            //itsMyTurn = true;

        _bombSeconds = 3;
        //TurnManager.instance.players.Add(this); PASARLO DE ALGUNA MANERA AL OTRO
        BasicSpawner.instance?.RegisterPlayer(this);

        if (Object.HasInputAuthority)
        {
            BasicSpawner.instance.localPlayer = this;
        }

        ProfilePicManager.instance?.UpdateUI();
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        BasicSpawner.instance?.UnregisterPlayer(this);
    }

    public override void FixedUpdateNetwork()
    {
        if (!GetInput(out NetworkInputData data))
            return;

        if (!itsMyTurn)
            return;

        if (data.aimDirection.sqrMagnitude > 0.0001f)
            aimDirection = data.aimDirection.normalized;

        if (data.BombTimer == 0)
            _bombSeconds = 3;
        else if (data.BombTimer != 0 && WeaponEquip == 1)
            _bombSeconds = data.BombTimer;

        WeaponEquip = data.WeaponSelected;

        if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0) && _holdedSeconds < _maximumHoldSeconds)
        {
            _startedCharging = true;
            _holdedSeconds += Runner.DeltaTime;

            if (HasStateAuthority && TurnManager.instance != null && TurnManager.instance.currentCharacter != null)
                TurnManager.instance.currentCharacter.ChangeAnim(food_charac.AnimState.PreparaLanzamiento);
        }
        else if (!data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0) && _startedCharging)
        {
            Shoot(_holdedSeconds);

            if (HasStateAuthority && TurnManager.instance != null && TurnManager.instance.currentCharacter != null)
                TurnManager.instance.currentCharacter.ChangeAnim(food_charac.AnimState.Lanza);

            if (TurnManager.instance != null)
                TurnManager.instance.isAbleToContinue = true;

            itsMyTurn = false;
        }

        HUDManager.instance.SelectWeapon(WeaponEquip);

        if (WeaponEquip == 1)
            HUDManager.instance.ChangedBombTimer(_bombSeconds);
        else
            HUDManager.instance.HideBombTimer();
    }


    void Shoot(float holdedTime)
    {
        _startedCharging = false;

        float _maximumForce = 30f;
        float _minimumForce = 3f;

        float _progress = holdedTime / _maximumHoldSeconds;
        float _force = Mathf.Clamp(_progress * _maximumForce, _minimumForce, _maximumForce);

        _holdedSeconds = 0f;
        _visualHoldedSeconds = 0f;

        if (Mira.instance != null)
            Mira.instance.ChargeShoot(0f);

        if (!HasStateAuthority)
            return;

        if (TurnManager.instance == null || TurnManager.instance.currentCharacter == null)
            return;

        Vector3 _shootDirection = new Vector3(aimDirection.x, aimDirection.y, 0f);

        if (_shootDirection.sqrMagnitude <= 0.0001f)
            return;

        _shootDirection.Normalize();

        TurnManager.instance.currentCharacter.Shoot(
            _shootDirection,
            _force,
            Flyweight_WeaponsPre.instance.weaponsPrefs[WeaponEquip],
            _bombSeconds
        );
    }

    public void ResetAimState(Vector3 newOrigin)
    {
        _startedCharging = false;
        _holdedSeconds = 0f;
        _visualHoldedSeconds = 0f;

        aimDirection = Vector2.zero;

        if (Mira.instance != null)
            Mira.instance.ChargeShoot(0f);
    }

    public void ForceReadyState(bool State)
    {
        if (!Object.HasInputAuthority)
            return;

        int backgroundIndex = (int)LevelOptions.instance.GetCurrentBackground();
        int gamemodeIndex = (int)LevelOptions.instance.GetCurrentGamemode();

        RPC_RequestSetReady(State, backgroundIndex, gamemodeIndex);
    }
    public void ChangeReadyState()
    {
        if (!Object.HasInputAuthority)
            return;

        int backgroundIndex = (int)LevelOptions.instance.GetCurrentBackground();
        int gamemodeIndex = (int)LevelOptions.instance.GetCurrentGamemode();

        RPC_RequestToggleReady(backgroundIndex, gamemodeIndex);
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void RPC_RequestToggleReady(int backgroundIndex, int gamemodeIndex)
    {
        isReady = !isReady;
        LobbyReadyManager.Instance?.CheckPlayersReady(BasicSpawner.instance.playersControlls);
    }


    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void RPC_RequestSetReady(bool state, int backgroundIndex, int gamemodeIndex)
    {
        isReady = state;
        selectedBackgroundIndex = backgroundIndex;
        selectedGamemodeIndex = gamemodeIndex;
        LobbyReadyManager.Instance?.CheckPlayersReady(BasicSpawner.instance.playersControlls);
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(playerID):


                    break;
                /*case nameof(itsMyTurn):
                    if (Mira.instance != null)
                        Mira.instance.isActive = itsMyTurn;


                    break;*/

                case nameof(_bombSeconds):

                    if (HUDManager.instance != null)
                        HUDManager.instance.ChangedBombTimer(_bombSeconds);
                    break;

                case nameof(playerpfpIndex):

                    ProfilePicManager.instance?.UpdateUI();
                    break;

                case nameof(playerNameIndex):

                    ProfilePicManager.instance?.UpdateUI();
                    break;

            }
        }
    }

    void Update()
    {
        if (!Object || !Object.HasInputAuthority)
            return;

        if (!itsMyTurn)
        {
            _visualHoldedSeconds = 0f;
            if (Mira.instance != null)
                Mira.instance.ChargeShoot(0f);
            return;
        }

        if (Input.GetMouseButton(0))
        {
            _visualHoldedSeconds += Time.deltaTime;
            _visualHoldedSeconds = Mathf.Min(_visualHoldedSeconds, _maximumHoldSeconds);

            if (Mira.instance != null)
                Mira.instance.ChargeShoot(_visualHoldedSeconds / _maximumHoldSeconds);
        }
        else if (_visualHoldedSeconds > 0f)
        {
            _visualHoldedSeconds = 0f;

            if (Mira.instance != null)
                Mira.instance.ChargeShoot(0f);
        }
    }

    public void RequestRandomAvatar()
    {
        if (!Object.HasInputAuthority)
            return;

        int randomAvatarIndex = Random.Range(0, ProfilePicManager.instance.allPossibleImages.Length);

        RPC_RequestSetAvatar(randomAvatarIndex);
    }
    public void RequestRandomName()
    {
        if (!Object.HasInputAuthority)
            return;

        int randomNameIndex = Random.Range(0, ProfilePicManager.instance.allPossibleNames.Length);

        RPC_RequestSetName(randomNameIndex);
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void RPC_RequestSetAvatar(int avatarIndex)
    {
        playerpfpIndex = avatarIndex;
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    void RPC_RequestSetName(int nameIndex)
    {
        playerNameIndex = nameIndex;
    }
}
