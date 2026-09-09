using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using DG.Tweening;

public class ButtonsGuessTheNumber : MonoBehaviour
{
    // Start is called before the first frame update
    public int number;
    public TMP_Text txt;
    public Button buttonComponent;
    public Vector3 _ogScale;
    RectTransform _rt;
    private void Awake()
    {
        _rt = GetComponent<RectTransform>();
            _ogScale = _rt.localScale;
    }
    void Start()
    {
        
    }

    public void Set(int numberS)
    {
        number = numberS;
        txt.text = numberS.ToString();
        buttonComponent.interactable = true;
        _rt.localScale = _ogScale;
    }

    public void SelectThis()
    {
        buttonComponent.interactable = false;
        _rt.DOScale(0.8f, 0.8f).SetEase(Ease.OutElastic);
        //Aniamcion

    }
    public void SetInteractable(bool state)
    {
        buttonComponent.interactable = state;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
