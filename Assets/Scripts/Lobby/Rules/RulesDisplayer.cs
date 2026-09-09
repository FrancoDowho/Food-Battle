using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
public class RulesDisplayer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] GameObject _popUp;
    [SerializeField] TMP_Text _text;
    public void OnPointerEnter(PointerEventData eventData)
    {
        _popUp.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _popUp.SetActive(false);

    }

    public void SetText(string txt)
    {
        _text.text = txt;
    }

    
}
