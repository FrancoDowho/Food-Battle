using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class Spin : NetworkBehaviour
{
    
    float _spinSpeed;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public override void Spawned()
    {
        base.Spawned();
        _spinSpeed = Random.Range(-500, -250);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        float rotation = _spinSpeed * Runner.DeltaTime;


        transform.Rotate(0, 0, rotation);
    }
}
