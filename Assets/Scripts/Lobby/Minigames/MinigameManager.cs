using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameManager : MonoBehaviour
{
    public static MinigameManager instance;
    [SerializeField] GameObject _visual;
    [SerializeField] GuessTheNumber _guessTheNumber;
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

    public void CurBeginMinigame()
    {
        _visual.SetActive(true);

        foreach (var item in BasicSpawner.instance.playersControlls)
        {
            if (item != null && item.Object.HasInputAuthority)
                item.ForceReadyState(false);
        }
        _guessTheNumber.gameObject.SetActive(true);
        _guessTheNumber.StartGame();
        _guessTheNumber.gameObject.SetActive(true);


    }
    // Start is called before the first frame update
    void Start()
    {
        LobbyReadyManager.Instance.ReadyMethod = CurBeginMinigame;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
