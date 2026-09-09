using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrid : MonoBehaviour
{
   [SerializeField]  ProfilePicMark _pfpPlayertwo;
    bool _isShowing;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Awake()
    {
       // EventManager.SubscribeToEvent(EventManager.EventsType.Event_PlayersJoined, CheckforOtherPlayers);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator CheckPlayersRoutine()
    {
        while (true)
        {
            int playerCount = 0;

                foreach (var player in BasicSpawner.instance.Runner.ActivePlayers)
                    playerCount++;
            

            if (playerCount > 1)
            {
                if (!_isShowing)
                {
                    _isShowing = true;
                    _pfpPlayertwo.Appear();
                }
            }
            else
            {
                if (_isShowing)
                {
                    _isShowing = false;
                    _pfpPlayertwo.Disappear();
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }
    public void CheckforOtherPlayers(params object[] parameters)
    {
        Debug.Log("Hola checkeado");
        int playerCount = (int)parameters[0];
        if (playerCount > 1)
        {
           StartCoroutine(Enable());
        }
            else
            {
                _pfpPlayertwo.Disappear();
        }


    }

    IEnumerator Enable()
    {
        yield return new WaitForSeconds(0.1f);
        _pfpPlayertwo.Appear();
    }

    private void OnEnable()
    {
        StartCoroutine(CheckPlayersRoutine());
        // EventManager.SubscribeToEvent(EventManager.EventsType.Event_PlayersJoined, CheckforOtherPlayers);
        // EventManager.SubscribeToEvent(EventManager.EventsType.Event_PlayersLeft, CheckforOtherPlayers);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        //EventManager.UnsubscribeToEvent(EventManager.EventsType.Event_PlayersJoined, CheckforOtherPlayers);
        //EventManager.UnsubscribeToEvent(EventManager.EventsType.Event_PlayersLeft, CheckforOtherPlayers);
    }
}
