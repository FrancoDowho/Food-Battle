using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaveMatchManager : MonoBehaviour
{
    // Start is called before the first frame update
  NetworkRunner _runner;
    public static LeaveMatchManager Instance { get; private set; }
    bool _isLeaving;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            CallLeavePopup();
    }
    public void CallLeavePopup()
    {
        if (_isLeaving)
            return;

        if (_runner == null || !_runner.IsRunning)
            return;

        PopUpManager.instance.ActivePopUp()
            .DefaultButtonsText()
            .SetTitle("Salir")
            .SetDesc("¿Seguro que quieres salir de la partida?")
            .SetButtonAction(0, PopUpManager.instance.ClosePopUp)
            .SetButtonAction(1, () =>
            {
                PopUpManager.instance.ClosePopUp();
                LeaveLobby();
            });
    }
    public async void LeaveLobby()
    {
        if (_isLeaving)
            return;

        _isLeaving = true;

        if (_runner != null && _runner.IsRunning)
            await _runner.Shutdown();

        ReturnToMenu();
    }

    public void ForceReturnToMenu()
    {
        if (_isLeaving)
            return;

        _isLeaving = true;
        ReturnToMenu();

        Debug.Log("Host left, returning to menu");
        
    }

    void ReturnToMenu()
    {
        SceneManager.LoadScene(0, LoadSceneMode.Single);
    }

    public void SetRunner(NetworkRunner runner)
    {
        _runner = runner;
    }
    void Start()
    {
        
    }

    
}
