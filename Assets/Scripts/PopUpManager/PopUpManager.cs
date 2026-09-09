using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;
public class PopUpManager : MonoBehaviour
{
    public static PopUpManager instance;
    public List<Button> buttons = new List<Button>();
    [HideInInspector] public List<TMP_Text> texts = new List<TMP_Text>();
    public TMP_Text title;
    public TMP_Text description;
    [SerializeField] GameObject _parentObject;
    [SerializeField] GameObject _buttonsParentObj;
    [SerializeField] float _scaleFrom = 0.4f;
    Sequence OpenPopUPAnims;
    CanvasGroup _cg;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        foreach (var item in buttons)
        {
            item.onClick.RemoveAllListeners();
        }
    }
    private void Start()
    {
        foreach (var item in buttons)
        {
            texts.Add(item.gameObject.GetComponentInChildren<TMP_Text>());
        }
        _cg = _parentObject.GetComponent<CanvasGroup>();
    }
    public void ClosePopUp()
    {
        title.text = "Estás seguro?";
        description.text = "Estás seguro?";
        description.color = Color.white;
        foreach (var item in buttons)
        {
            item.onClick.RemoveAllListeners();
            item.gameObject.transform.SetParent(_parentObject.gameObject.transform);
            item.gameObject.SetActive(false);
        }
        _parentObject.SetActive(false);

    }

    public PopUpManager SetAmountOfButtons(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            buttons[i].gameObject.SetActive(true);
            buttons[i].gameObject.transform.SetParent(_buttonsParentObj.transform);

        }
        return this;
    }

    public PopUpManager ActivePopUp()
    {
        ClosePopUp();
        _parentObject.SetActive(true);
        OpenPopUPAnims.Kill();
        OpenPopUPAnims = DOTween.Sequence();
        _cg.alpha = 0;
        float timeToOpen = 0.2f;
        OpenPopUPAnims.Append(_parentObject.transform.DOScale(_scaleFrom, 0));
        OpenPopUPAnims.Append(_parentObject.transform.DOScale(1, timeToOpen).SetEase(Ease.OutQuad));
        OpenPopUPAnims.Join(_cg.DOFade(1, timeToOpen));


       


        return this;
    }

    public PopUpManager SetButtonText(int number, string txt)
    {
        texts[number].text = txt;
        return this;

    }

    public PopUpManager SetTitle(string txt)
    {
        title.text = txt;
        return this;

    }

    public PopUpManager SetDesc(string txt)
    {
        description.text = txt;
        return this;

    }
    public PopUpManager SetDescColor(Color colorTOApply)
    {
        description.color = colorTOApply;
        return this;
    }

    public PopUpManager SetButtonAction(int buttonNumber,Action actionToDo)
    {
        //Tabas acá
        buttons[buttonNumber].onClick.AddListener(() => actionToDo());
        return this;
    }

    /// <summary>
    /// Settea 2 botones, uno con el texto no y otro con el texto si
    /// </summary>
    public PopUpManager DefaultButtonsText()
    {
        SetAmountOfButtons(2);

        texts[0].text = "No";
        texts[1].text = "Si";
        return this;
    }
}
