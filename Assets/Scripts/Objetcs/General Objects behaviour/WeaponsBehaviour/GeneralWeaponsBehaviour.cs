using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class GeneralWeaponsBehaviour : NetworkBehaviour
{
    protected Bullet _bulletBehaviour;
    public override void Spawned()
    {
        base.Spawned();
        _bulletBehaviour = gameObject.GetComponent<Bullet>();
    }

    public virtual void RandomizeStatistics()
    {

    }
}
