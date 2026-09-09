using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGChange : MonoBehaviour
{
    // Start is called before the first frame update
    int _currentBGSelected;
    [SerializeField] Image _bgDisplay;
    private LevelOptions.BackgroundType _currentBGType;

    void Start()
    {
        _bgDisplay.sprite = LevelOptions.instance.bgOptions[_currentBGSelected];
    }
    public void ChangeBackground(int amount)
    {
        int bgCount = LevelOptions.instance.bgOptions.Length;

        _currentBGSelected += amount;

        if (_currentBGSelected >= bgCount)
            _currentBGSelected = 0;
        else if (_currentBGSelected < 0)
            _currentBGSelected = bgCount - 1;

        LevelOptions.instance.bgSelected = (LevelOptions.BackgroundType)_currentBGSelected;
        _bgDisplay.sprite = LevelOptions.instance.bgOptions[_currentBGSelected];

    }
    

    // Update is called once per frame
    void Update()
    {
        
    }
}
