using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class BaseController 
{
    NetworkBehaviour _behaviour;
    [Networked] TickTimer _delay { get; set; }
    float _seconds;
    Bullet _bullet;
    public BaseController(NetworkBehaviour behaviour , float seconds)
    {
        _behaviour = behaviour;

        _seconds = seconds;
    }

    public void Update()
    {
        if(_behaviour.GetInput(out NetworkInputData data))
        {
            if(_behaviour.HasStateAuthority && _delay.ExpiredOrNotRunning(_behaviour.Runner))
            {
                if (data.buttons.IsSet(NetworkInputData.MOUSEBUTTON0))
                {
                    _delay = TickTimer.CreateFromSeconds(_behaviour.Runner , _seconds);

                    Debug.Log("Carga bala");

                    //_behaviour.Runner.Spawn(_bullet, _behaviour.transform.position, _behaviour.transform.rotation,_behaviour.Object.InputAuthority);
                }
            }
        }
    }
}
