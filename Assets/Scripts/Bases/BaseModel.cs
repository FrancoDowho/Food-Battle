using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseModel 
{
    Base_Attacks _attacks;
    Base _player;

    public BaseModel(Base player)
    {
        _player = player;
    }
   
    public void Start()
    {
        _attacks = new Base_Attacks(_player);
    }

    
    public void TakeDmg(int dmg, ref int health)
    {
        health -= dmg;
    }
}
