using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class lifeObserverManager : NetworkBehaviour
{
    ChangeDetector _changeDetector;

    public static lifeObserverManager instance;
    [Networked]
    public int veggiesLife { get; set; }
    [Networked] 
    public int fruitLife { get; set; }
    [HideInInspector] public List<food_charac> charactersHurt = new List<food_charac>();
    bool test;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void Spawned()
    {

        base.Spawned();
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        
    }
    public override void Render()
    {
        if (_changeDetector != null)
        {
            foreach (var change in _changeDetector.DetectChanges(this))
            {
                switch (change)
                {
                    case nameof(veggiesLife):
                        //hi
                        break;
                    case nameof(fruitLife):
                        //hihello
                        break;
                }
            }
        }
        

        
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }


    }
    
    public void UpdateCharactersLifes()
    {
        foreach (var ch in charactersHurt)
        {
            float destiny = (float)ch.life / (float)ch.maxlife;
            ch.heartLifeHUD.RPC_UpdateHeartBar(destiny);
        }

        charactersHurt.Clear();
    }

    public void UpdateLifes()
    {
        veggiesLife = Mathf.RoundToInt(GetCharactersLifes(TurnManager.instance.vegetables));
        fruitLife = Mathf.RoundToInt(GetCharactersLifes(TurnManager.instance.fruits));

        if (fruitLife <= 0)
        {
            HUDManager.instance.RPC_EndMatch(true);
            foreach (var item in TurnManager.instance.vegetables)
            {
                item.ChangeAnim(food_charac.AnimState.Win);
            }
        }
        else if(veggiesLife <= 0)
        {
            HUDManager.instance.RPC_EndMatch(false);
            foreach (var item in TurnManager.instance.fruits)
            {
                item.ChangeAnim(food_charac.AnimState.Win);
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(HUDManager.instance.SlideBarChange(3, HUDManager.instance.VeggieSlider));
        }
        



    }

    public float GetCharactersLifes(List<food_charac> Charact)
    {
        float Tlife = 0;
        foreach (var item in Charact)
        {
            Tlife += item.life;


        }
        return Tlife;

    }

   
}
