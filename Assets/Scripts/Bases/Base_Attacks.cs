using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_Attacks 
{
    Base _player;
    Bullet _bullet;
    float _throwForce;
    float _throwForceUp;

    public Base_Attacks(Base player)
    {
        _player = player;
        _throwForce = player.throwForce;
        _throwForceUp = player.throwForceUp;
    }

    public void Shoot()
    {
        //Bullet bullet = Runner.Spawn bla bla bla
        
        //Rigidbody2D _rb = bullet.GetComponent<Rigidbody2D>();

        //Vector3 force = multiplicar posicion del mouse por fuerzas o algo asi

        // _rb.AddForce(force , ForceMode.Impulse);
    }
    
}
