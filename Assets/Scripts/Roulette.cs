using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roulette<T> : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static T PickAWinner(List<T> Options, float numberChosen)
    {
        float cirlce360 = 360;
        float AngleEachObject = cirlce360 / Options.Count;
        T Winner = Options[0];
        for (int i = 0; i < Options.Count; i++)
        {
            if (numberChosen >= AngleEachObject * i && numberChosen < AngleEachObject * (i + 1))
            {
                Winner = Options[i];
                break;
            }


        }
        return Winner;

    }

   
}
