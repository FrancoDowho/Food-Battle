using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GoesUpAndFall : NetworkBehaviour
{
    [SerializeField] float _fallSpeed;
    [SerializeField] float _fallSpeedMLT;
    [SerializeField] float _startSpeed;
    [SerializeField] float _dissapearAfter;
    [Networked] TickTimer _counterAutoDestruct { get; set; }
    float _horizontalSpeed;
    int _direction;


    // Start is called before the first frame update
    void Start()
    {
        int chance = Random.Range(0, 101);
        _direction = chance > 50 ? 1 : -1;
        _horizontalSpeed = Random.Range(0.005f, 0.1f) * _direction;
    }
    public override void Spawned()
    {
        base.Spawned();
        _counterAutoDestruct = TickTimer.CreateFromSeconds(Runner, _dissapearAfter);

    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        _fallSpeed *= _fallSpeedMLT;
        _startSpeed -= Runner.DeltaTime * _fallSpeed;
        gameObject.transform.position += (Vector3.up * _startSpeed) + (Vector3.right * _horizontalSpeed);

        if (_counterAutoDestruct.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
