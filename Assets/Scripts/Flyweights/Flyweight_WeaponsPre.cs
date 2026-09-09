using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flyweight_WeaponsPre : MonoBehaviour
{
    public static Flyweight_WeaponsPre instance;
    public Bullet[] weaponsPrefs;

    public particle[] particlesPref;
    
    // Start is called before the first frame update

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }


    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
