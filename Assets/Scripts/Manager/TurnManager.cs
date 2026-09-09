using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using System.Linq;

public class TurnManager : NetworkBehaviour
{
    ChangeDetector _changeDetector;

    public static TurnManager instance;

    public food_charac currentCharacter;
    public List<food_charac> vegetables = new List<food_charac>();
    public List<food_charac> fruits = new List<food_charac>();
    public List<PlayerControll> players = new List<PlayerControll>();

    [Networked] public int CurPlayerTurn { get; set; }
    [Networked] public bool isAbleToContinue { get; set; }

    [Networked, OnChangedRender(nameof(OnCurrentCharacterObjectChanged))]
    NetworkObject CurrentCharacterObject { get; set; }

    public int currentAmountOfBulletsOnScreen;
    float _counterForTurnChange;
    float _counterForTurnChangeGeneral;
    public float timeToWaitToChange;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public override void Spawned()
    {
        base.Spawned();
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        ResolveCurrentCharacterFromNetwork();
    }

    public override void Render()
    {
        ResolveCurrentCharacterFromNetwork();

        if (_changeDetector == null)
            return;

        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(CurPlayerTurn):
                    HUDManager.instance.RPC_TurnChange(CurPlayerTurn);
                    break;

                case nameof(isAbleToContinue):
                    break;
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        ResolveCurrentCharacterFromNetwork();

        if (!HasStateAuthority)
            return;

        CheckIfCanContinue(
            currentAmountOfBulletsOnScreen,
            GetRigidBodysSpeed(
                vegetables
                    .Concat(fruits)
                    .Where(x => x != null)
                    .Select(x => x.GetComponent<Rigidbody2D>())
                    .Where(x => x != null)
                    .ToList()
            )
        );
    }

    void OnCurrentCharacterObjectChanged()
    {
        ResolveCurrentCharacterFromNetwork();
    }

    void ResolveCurrentCharacterFromNetwork()
    {
        food_charac resolvedCharacter = null;

        if (CurrentCharacterObject != null)
            resolvedCharacter = CurrentCharacterObject.GetComponent<food_charac>();

        if (currentCharacter == resolvedCharacter)
            return;

        currentCharacter = resolvedCharacter;

        if (currentCharacter != null && Mira.instance != null)
            Mira.instance.ChangePJ(currentCharacter.gameObject);
    }

    void RefreshPlayersCache()
    {
        if (BasicSpawner.instance == null)
            return;

        players = BasicSpawner.instance.GetPlayersControlls()
            .Where(player => player != null)
            .OrderBy(player => player.playerID)
            .ToList();
    }

    void SetCurrentCharacter(food_charac nextCharacter)
    {
        currentCharacter = nextCharacter;

        if (nextCharacter != null)
            CurrentCharacterObject = nextCharacter.GetComponent<NetworkObject>();
        else
            CurrentCharacterObject = null;

        if (currentCharacter != null && Mira.instance != null)
            Mira.instance.ChangePJ(currentCharacter.gameObject);
    }

    public void RPCAdvanceTurn()
    {
        if (!HasStateAuthority)
            return;

        lifeObserverManager.instance.UpdateLifes();

        if (vegetables.Count <= 0)
            return;

        if (fruits.Count <= 0)
            return;

        RefreshPlayersCache();

        if (CurPlayerTurn == 0)
        {
            if (vegetables.Count > 0)
            {
                vegetables.Add(vegetables[0]);
                vegetables.RemoveAt(0);
            }

            SetCurrentCharacter(fruits[0]);
        }
        else
        {
            if (fruits.Count > 0)
            {
                fruits.Add(fruits[0]);
                fruits.RemoveAt(0);
            }

            SetCurrentCharacter(vegetables[0]);
        }

        StartCoroutine(TurnFinished());
    }

    IEnumerator TurnFinished()
    {
        yield return new WaitForSeconds(2f);

        HUDManager.instance.RPC_UpdateHealthBars();
        lifeObserverManager.instance.UpdateCharactersLifes();

        yield return new WaitForSeconds(1f);

        RefreshPlayersCache();

        CurPlayerTurn = CurPlayerTurn + 1 > 1 ? 0 : 1;

        foreach (var player in players)
        {
            if (player != null)
                player.itsMyTurn = false;
        }

        if (players.Count > CurPlayerTurn && players[CurPlayerTurn] != null)
        {
            players[CurPlayerTurn].itsMyTurn = true;

            if (currentCharacter != null)
                players[CurPlayerTurn].ResetAimState(currentCharacter.transform.position);
        }
    }

    void CheckIfCanContinue(int amountBull, float speeds)
    {
        if (!isAbleToContinue)
            return;

        _counterForTurnChangeGeneral += Runner.DeltaTime;

        if (amountBull <= 0 && speeds <= 2f && speeds >= -2f)
        {
            _counterForTurnChange += Runner.DeltaTime;

            if (_counterForTurnChange >= timeToWaitToChange)
            {
                RPCAdvanceTurn();

                _counterForTurnChangeGeneral = 0f;
                _counterForTurnChange = 0f;
                isAbleToContinue = false;
            }
        }
        else
        {
            _counterForTurnChange = 0f;
        }

        if (_counterForTurnChangeGeneral >= 20f)
        {
            RPCAdvanceTurn();

            _counterForTurnChangeGeneral = 0f;
            _counterForTurnChange = 0f;
            isAbleToContinue = false;
        }
    }

    float GetRigidBodysSpeed(List<Rigidbody2D> rigidsToCheck)
    {
        float totalSpeed = 0f;

        foreach (var rigid in rigidsToCheck)
        {
            if (rigid == null)
                continue;

            totalSpeed += rigid.velocity.magnitude;
        }

        return totalSpeed;
    }

    public void BeginGame()
    {
        if (!HasStateAuthority)
            return;

        RefreshPlayersCache();

        if (vegetables.Count <= 0)
            return;

        if (players.Count <= 0)
            return;

        foreach (var player in players)
        {
            if (player != null)
                player.itsMyTurn = false;
        }

        CurPlayerTurn = 0;
        SetCurrentCharacter(vegetables[0]);

        if (players.Count > CurPlayerTurn && players[CurPlayerTurn] != null)
        {
            players[CurPlayerTurn].itsMyTurn = true;
            players[CurPlayerTurn].ResetAimState(currentCharacter.transform.position);
        }

        HUDManager.instance.RPC_TurnChange(CurPlayerTurn);
    }
}