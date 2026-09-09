using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RuleSelector : MonoBehaviour
{
    int _selectedRule;
    [SerializeField] TMP_Text _nameDisplay;
    [SerializeField] TMP_Text _ruleDescription;
    [SerializeField] Image _ruleImage;

    // Start is called before the first frame update
    void Start()
    {
        ChangeRuleSelected(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeRuleSelected(int amount)
    {
        _selectedRule += amount;
        if (_selectedRule >= LevelOptions.instance.rules.Length)
        {
            _selectedRule = 0;
        }
        else if (_selectedRule < 0)
        {
            _selectedRule = LevelOptions.instance.rules.Length - 1;
        }
        _ruleImage.sprite = LevelOptions.instance.rules[_selectedRule].ruleSprite;
        LevelOptions.instance.gmSelected = (LevelOptions.Gamemode)_selectedRule;
        _nameDisplay.text = LevelOptions.instance.rules[_selectedRule].ruleName;
        _ruleDescription.text = LevelOptions.instance.rules[_selectedRule].ruleName + ": " + LevelOptions.instance.rules[_selectedRule].description;
    }
}
