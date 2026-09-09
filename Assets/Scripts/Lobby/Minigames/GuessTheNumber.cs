using DG.Tweening;
using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Fusion.NetworkBehaviour;


public class GuessTheNumber : NetworkBehaviour
{
    public static GuessTheNumber instance;
    int _numberUpTo = 10;
    public List<int> numbersAlreadyChosen = new List<int>();
    [SerializeField] GameObject _lockScreen;
    [SerializeField] GameObject _chooseANumberScreen;
    [SerializeField] GameObject _chosenNumbersScreen;
    [SerializeField] GameObject _resultScreen;
    [SerializeField] GameObject _rulesShowcaseScreen;
    [SerializeField] RulesSelectedScreen _rulesShowcaseStuff;
    [SerializeField] CanvasGroup _cg;
    [SerializeField] TMP_Text _wasTheNumberGuessed;
    [SerializeField] TMP_Text _resultYorN;
    [SerializeField] TMP_Text _winningNumberTxt;
    public Grid grid;
    Sequence _sequenceChosenNumbers;
    Sequence _sequenceNoOneGuessed;
    Sequence _sequenceSomeoneGuessed;
    RectTransform _rtResultYorN;
    Dictionary<PlayerRef, int> _playerChoices = new();
    [SerializeField] string[] _randomNames;


    [Networked] public int numberChosen { get; set; }
    [Networked] int _roundUsedNumberA { get; set; }
    [Networked] int _roundUsedNumberB { get; set; }

    ChangeDetector _changeDetector;
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }
    void Start()
    {
        _rtResultYorN = _resultYorN.gameObject.GetComponent<RectTransform>();
    }
    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        if (HasStateAuthority)
        {
            _roundUsedNumberA = -1;
            _roundUsedNumberB = -1;
        }
    }

    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(_roundUsedNumberA):
                case nameof(_roundUsedNumberB):
                    RefreshButtonsState();
                    break;
            }
        }
    }


    public void RemoveNumberFromList(List<int> list, int numberChosen)
    {
        list.Remove(numberChosen);
    }

    public List<int> GenerateNumbersList(int HowManyNumbers, bool StartsFrom0 = false)
    {
        List<int> newList = new List<int>();
        for (int i = 0; i < HowManyNumbers; i++)
        {
            newList.Add(i + 1);
        }

        return newList;
    }

    public void StartGame()
    {
        _playerChoices.Clear();

        if (_chooseANumberScreen != null)
            _chooseANumberScreen.SetActive(true);

        if (_chosenNumbersScreen != null)
            _chosenNumbersScreen.SetActive(false);

        if (_lockScreen != null)
            _lockScreen.SetActive(false);

        grid.GenerateGridWithNumbers(_numberUpTo);

        foreach (var button in grid.buttons)
        {
            ButtonsGuessTheNumber localButton = button;

            localButton.buttonComponent.onClick.RemoveAllListeners();
            localButton.buttonComponent.onClick.AddListener(() =>
            {
                OnLocalButtonPressed(localButton);
            });
        }

        RefreshButtonsState();

       

        if (HasStateAuthority)
        {
            numberChosen = ChooseRandomNumber(_numberUpTo);
                RPC_ChangeTxt(numberChosen);

        }
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]

    void RPC_ChangeTxt(int number)
    {
        _winningNumberTxt.text = $"El número correcto era: {number}!";
    }



    int ChooseRandomNumber(int upTo)
    {
        return Random.Range(1, upTo + 1);
    }

    void StartNewRound(int usedA, int usedB)
    {
        if (BasicSpawner.instance != null && BasicSpawner.instance.localPlayer != null)
            BasicSpawner.instance.localPlayer.ForceReadyState(false);

        _playerChoices.Clear();

        grid.RemoveButtonsByNumber(usedA, usedB);
        grid.gameObject.SetActive(true);

        if (HasStateAuthority)
        {
            _roundUsedNumberA = -1;
            _roundUsedNumberB = -1;
        }

        _lockScreen.SetActive(false);
        _chooseANumberScreen.SetActive(true);
        _chosenNumbersScreen.SetActive(false);
        _resultScreen.SetActive(false);

        RefreshButtonsState();
    }


    public void PlayerReady()
    {
        BasicSpawner.instance.localPlayer.ForceReadyState(true);
        Debug.Log("Ready player: " + Runner.LocalPlayer.PlayerId);
    }

    public void CoverScreenToDisableButtons()
    {
        _lockScreen.SetActive(true);
    }
    public void BothPlayersReady()
    {

        ResetRevealVisuals();

        if (_chooseANumberScreen != null)
            _chooseANumberScreen.SetActive(false);

        bool txtSaysYes = false;
        float timeBetweenSpins = 0.3f;
        float timeBetweenSpinsHalf = timeBetweenSpins / 2f;
        float timeFaded = 0.5f;

        _sequenceChosenNumbers?.Kill();
        _sequenceChosenNumbers = DOTween.Sequence();
        _sequenceChosenNumbers.AppendInterval(0.5f);
        _sequenceChosenNumbers.Append(_cg.DOFade(0, timeFaded));
        _sequenceChosenNumbers.AppendCallback(() =>
        {
            if (_chosenNumbersScreen != null)
                _chosenNumbersScreen.SetActive(true);

            _resultScreen.SetActive(true);
            _wasTheNumberGuessed.alpha = 0;
            _wasTheNumberGuessed.text = "Fue el número elegido adivinado?";
        });
        _sequenceChosenNumbers.Append(_cg.DOFade(1, timeFaded));



        _sequenceChosenNumbers.Append(_wasTheNumberGuessed.DOFade(1, 0.3f));

        Sequence repeatBlock = DOTween.Sequence();
        repeatBlock.Append(_rtResultYorN.DOScaleX(-1, timeBetweenSpins));
        repeatBlock.Append(_rtResultYorN.DOScaleX(0, timeBetweenSpinsHalf));
        repeatBlock.AppendCallback(ChangeText);
        repeatBlock.Append(_rtResultYorN.DOScaleX(1, timeBetweenSpinsHalf));
        repeatBlock.SetLoops(3, LoopType.Restart);

        _sequenceChosenNumbers.Append(repeatBlock);
        _sequenceChosenNumbers.AppendCallback(() =>
        {
            if (!HasStateAuthority)
                return;

            List<int> numbers = _playerChoices.Values.ToList();

            if (DidAnyoneGuessTheNumber(numbers, numberChosen))
                RPC_SomeoneGuessed();
            else
                RPC_NoOneGuessed(_roundUsedNumberA, _roundUsedNumberB);
        });

        void ChangeText()
        {
            txtSaysYes = !txtSaysYes;
            _resultYorN.text = txtSaysYes ? "Si" : "No";
        }
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_NoOneGuessed(int usedA, int usedB)
    {
        ResetRevealVisuals();

        _chosenNumbersScreen.SetActive(true);
        _resultYorN.text = "No";
        _wasTheNumberGuessed.text = "";

        _sequenceNoOneGuessed = DOTween.Sequence();

        string textTOUse = "Nadie adivinó, vamos de nuevo!";
        float txtSpeed = 0.05f;

        _sequenceNoOneGuessed.Append(_wasTheNumberGuessed.DOText(textTOUse, textTOUse.Length * txtSpeed));
        _sequenceNoOneGuessed.AppendInterval(2);
        _sequenceNoOneGuessed.AppendCallback(() =>
        {
            StartNewRound(usedA, usedB);
        });
        _sequenceNoOneGuessed.AppendCallback(() =>
        {
            _resultScreen.SetActive(false);
        });
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]

    void RPC_SomeoneGuessed()
    {
        

        _resultYorN.text = "Si!";
        _wasTheNumberGuessed.text = "";
        _sequenceSomeoneGuessed = DOTween.Sequence();
        string textTOUse = "Alguien adivinó el número!";
        float txtSpeed = 0.05f;
        _sequenceSomeoneGuessed.Append(_resultYorN.DOScale(1.5f, 0.3f).SetEase(Ease.OutQuad));
        _sequenceSomeoneGuessed.AppendInterval(1);
        _sequenceSomeoneGuessed.Append(_wasTheNumberGuessed.DOText(textTOUse, textTOUse.Length * txtSpeed));
        _sequenceSomeoneGuessed.AppendInterval(1.5f);
        string textTOUse2 = "El ganador fue...";
        _sequenceSomeoneGuessed.AppendCallback(() =>
        {
            _wasTheNumberGuessed.text = "";
        });
        _sequenceSomeoneGuessed.Append(_wasTheNumberGuessed.DOText(textTOUse2, textTOUse.Length * txtSpeed));
        float timeBetweenRandomNames = 0.5f;
        int currentName = 0;
        Sequence repeatBlock = DOTween.Sequence();
        for (int i = 0; i < 50; i++)
        {
            repeatBlock.AppendCallback(() =>
            {
                currentName++;
                if (currentName >= _randomNames.Length)
                    currentName = 0;

                _resultYorN.text = _randomNames[currentName];
            });
            repeatBlock.AppendInterval(timeBetweenRandomNames);
            timeBetweenRandomNames /= 2f;
        }

        _sequenceSomeoneGuessed.Append(repeatBlock);
        if(HasStateAuthority)
        {
            _sequenceSomeoneGuessed.AppendCallback(() =>
            {
                foreach (var kvp in _playerChoices)
                {
                    bool won = kvp.Value == numberChosen;
                    RPC_ShowResult(kvp.Key, won);
                }
            }
            );
        }

        //Acá poner de alguna manera quein gano, comparando jugadores locales y eso

    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_ShowResult(PlayerRef playerTOCheck, bool didWin)
    {
        _sequenceSomeoneGuessed?.Kill();

        if (playerTOCheck == Runner.LocalPlayer)
        {
            if(didWin)
            {
                //MSGGanaste! y confite!
                Sequence TXTAnimation = DOTween.Sequence();
                float txtSpeedAnim = 0.5f;
                _resultYorN.text = "Vos!";
                _resultYorN.color = Color.blue;
                TXTAnimation.Append(_resultYorN.DOScale(1.3f, txtSpeedAnim));
                TXTAnimation.Append(_resultYorN.DOScale(1, txtSpeedAnim));
                TXTAnimation.SetLoops(-1, LoopType.Restart);
                
            }
            else
            {
                _resultYorN.text = "El rival...";
                _resultYorN.color = Color.red;
                //MSGPerdiste
            }
        }

        
        StartCoroutine(ShowRulesToWinner(playerTOCheck));
    }

    IEnumerator ShowRulesToWinner(PlayerRef winnerPlayer)
    {
        yield return new WaitForSeconds(1f);
        _winningNumberTxt.gameObject.SetActive(true);

        yield return new WaitForSeconds(2.2f);
        _chosenNumbersScreen.SetActive(false);
        if (HasStateAuthority)
            RPC_OpenWinnerRulesScreen(winnerPlayer);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_OpenWinnerRulesScreen(PlayerRef winnerRef)
    {
        PlayerControll winnerPlayer = GetPlayerControllByPlayerRef(winnerRef);

        MatchConfig config = LevelOptions.instance.CreateMatchConfig((LevelOptions.BackgroundType)winnerPlayer.selectedBackgroundIndex, (LevelOptions.Gamemode)winnerPlayer.selectedGamemodeIndex, winnerPlayer);
        LevelOptions.instance.matchConfig = config;

        LobbyReadyManager.Instance.DisplayRules(winnerPlayer);

    }

    PlayerControll GetPlayerControllByPlayerRef(PlayerRef playerRef)
    {
        foreach (var player in BasicSpawner.instance.playersControlls)
        {
            if (player.Object != null && player.Object.InputAuthority == playerRef)
                return player;
        }

        return null;
    }

    bool DidAnyoneGuessTheNumber(List<int> numbers, int numberToGuess)
    {
        foreach (var n in numbers)
        {
            if (n == numberToGuess)
                return true;
        }

        return false;
    }

    void RefreshButtonsState()
    {
        

        foreach (var button in grid.buttons)
        {
            if (button == null)
                continue;

            bool used = button.number == _roundUsedNumberA || button.number == _roundUsedNumberB;
            button.SetInteractable(!used);
        }
    }


    void OnLocalButtonPressed(ButtonsGuessTheNumber button)
    {
       

        if (!button.buttonComponent.interactable)
            return;

        RPC_SubmitChoice(button.number);

        button.SelectThis();
        CoverScreenToDisableButtons();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
    void RPC_SubmitChoice(int number, RpcInfo info = default)
    {

        Debug.Log($"RPC_SubmitChoice llegó. Source: {info.Source}, Number: {number}");

        if (_playerChoices.ContainsKey(info.Source))
            return;

        if (_roundUsedNumberA == number || _roundUsedNumberB == number)
            return;

        _playerChoices[info.Source] = number;

        if (_roundUsedNumberA == -1)
            _roundUsedNumberA = number;
        else if (_roundUsedNumberB == -1)
            _roundUsedNumberB = number;

        if (_playerChoices.Count >= 2)
        {
            RPC_BeginRevealPhase();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    void RPC_BeginRevealPhase()
    {
        BothPlayersReady();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    void ResetRevealVisuals()
    {
        _sequenceChosenNumbers?.Kill();
        _sequenceNoOneGuessed?.Kill();
        _sequenceSomeoneGuessed?.Kill();
        _resultYorN.text = "";
        _resultYorN.color = Color.white;
        _resultYorN.alpha = 1f;
        _resultYorN.rectTransform.localScale = Vector3.one;

        _wasTheNumberGuessed.text = "";
        _wasTheNumberGuessed.alpha = 1f;


    }
}
