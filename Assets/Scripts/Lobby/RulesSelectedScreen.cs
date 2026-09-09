using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RulesSelectedScreen : MonoBehaviour
{
    [SerializeField] RulesDisplayer _questionMark;
    [SerializeField] TMP_Text _rulename;
    [SerializeField] Image _ruleImage;
    [SerializeField] Image _BGImage;


    public void SetRules(Sprite _bgSpr, Sprite _ruleSprt, string ruleName, string ruleDesc)
    {
        _questionMark.SetText(ruleDesc);
        _rulename.text = ruleName;
        _ruleImage.sprite = _ruleSprt;
        _BGImage.sprite = _bgSpr;
    }

    public void SetBG()
    {
        _BGImage.sprite = null;

    }
}
