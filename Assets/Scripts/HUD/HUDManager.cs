using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using TMPro;


public class HUDManager : NetworkBehaviour
{

    public static HUDManager instance;

    public Slider VeggieSlider;
    public Slider FruitSlider;
    public TMP_Text txt_victory;
    public TMP_Text txt_bombTimer;
    public TMP_Text txt_Turn;
    [SerializeField] GameObject _slotSelectedImg;
    [SerializeField] GameObject[] _bagSlots;
    [SerializeField] float maxSpacing;
    [SerializeField] float minSpacing;
    float speed = 0.04f;
    [SerializeField] GameObject _EndScreen;
    [SerializeField] GameObject _EndScreenFrame;
    [SerializeField] GameObject _quienGana;

    [SerializeField] Color _winColor;
    [SerializeField] Color _looseColor;


    //ChangeDetector _changeDetector;

    
    // Start is called before the first frame update

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }


    }
    
    public override void Spawned()
    {
        base.Spawned();
        StartCoroutine(InitHUD());

        MenuManager.instance?.HideEverythingInMenu();
        HideBombTimer();

    }
    void Start()
    {
    }
    IEnumerator InitHUD()
    {
        yield return new WaitForSeconds(0.03f);
        int VeggieLife = Mathf.RoundToInt(lifeObserverManager.instance.GetCharactersLifes(TurnManager.instance.vegetables));
        int FruitLife = Mathf.RoundToInt(lifeObserverManager.instance.GetCharactersLifes(TurnManager.instance.fruits));


        InitSlider(VeggieSlider, VeggieLife);
        InitSlider(FruitSlider, FruitLife);
    }
    // Update is called once per frame
    void Update()
    {
        if (txt_bombTimer.characterSpacing >= maxSpacing)
        {
            speed *= -1;
        }
        else if(txt_bombTimer.characterSpacing <= minSpacing)
            speed *= -1;
        txt_bombTimer.characterSpacing += speed;

        
    }
    public void ChangedBombTimer(int timer)
    {
        txt_bombTimer.text = timer + " segundos";
        txt_bombTimer.CrossFadeAlpha(1, 0, true);
        txt_bombTimer.CrossFadeAlpha(0, 1f, true);
    }
    public void HideBombTimer()
    {
        txt_bombTimer.CrossFadeAlpha(0, 0.1f, true);
    }

    public void SelectWeapon(int weaponSelect)
    {
        _slotSelectedImg.transform.position = _bagSlots[weaponSelect].transform.position;

       

    }
   

    void InitSlider(Slider slider, float max)
    {
        slider.maxValue = max;
        slider.value = max;
    }

    public void RPC_TurnChange(int turnOf)
    {
        txt_Turn.gameObject.transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

        txt_Turn.gameObject.transform.parent.gameObject.SetActive(false);
        txt_Turn.gameObject.transform.parent.gameObject.SetActive(true);
        if (turnOf == 0)
        {
            if(HasStateAuthority)
            {
                txt_Turn.text = "Tu turno";
                txt_Turn.color = Color.blue;
            }
            else
            {
                txt_Turn.text = "Turno del rival";
                txt_Turn.color = Color.red;
            }
        }
        else
        {
            if (!HasStateAuthority)
            {
                txt_Turn.text = "Tu turno";
                txt_Turn.color = Color.blue;
            }
            else
            {
                txt_Turn.text = "Turno del rival";
                txt_Turn.color = Color.red;
            }
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
    public void RPC_EndMatch(bool IsHostWinner)
    {
        _EndScreen.gameObject.SetActive(true);
        StartCoroutine(EndMatchCorroutine());
        var FrameColor = _EndScreenFrame.GetComponent<Image>();
        if (IsHostWinner)
        {
            if (HasStateAuthority)
            {
                txt_victory.text = "TU GANAS";
                FrameColor.color = _winColor;

            }
            else
            {
                FrameColor.color = _looseColor;
                txt_victory.text = "DERROTA";

            }
        }
        else
        {
            if (!HasStateAuthority)
            {
                txt_victory.text = "TU GANAS";
                FrameColor.color = _winColor;
            }
            else
            {
                FrameColor.color = _looseColor;
                txt_victory.text = "DERROTA";
            }
        }
    }

    IEnumerator EndMatchCorroutine()
    {
        yield return new WaitForSeconds(3);
        PopUpManager.instance.ActivePopUp()
           .SetAmountOfButtons(1)
           .SetTitle("Fin de la partida")
           .SetDesc("El odio ha causado muchos problemas...pero nunca ha solucionado ninguno")
           .SetButtonText(0, "Volver al menu")
           .SetButtonAction(0, () =>
           {
               PopUpManager.instance.ClosePopUp();
              LeaveMatchManager.Instance.LeaveLobby();

           });

    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]

    public void RPC_UpdateHealthBars()
    {
        StartCoroutine(SlideBarChange(
           lifeObserverManager.instance.veggiesLife, HUDManager.instance.VeggieSlider));
        StartCoroutine(SlideBarChange(
            lifeObserverManager.instance.fruitLife, HUDManager.instance.FruitSlider));
    }

   

    public IEnumerator SlideBarChange(int Destiny, Slider slide)
    {
        float speed = 1.5f;
        WaitForSeconds wait = new WaitForSeconds(0.01f);

        while (slide.value > Destiny)
        {
            yield return wait;
            slide.value -= speed;

        }
        slide.value = Destiny;

    }


}
