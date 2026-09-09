using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;

public class levelSelectorManager : NetworkBehaviour
{
    public static levelSelectorManager instance;
    public GameObject[] maps;
    public MapsConfigs[] mapsConfigs;
    public SpriteRenderer bgImage;
    [Networked, OnChangedRender(nameof(OnBackgroundChanged))]
    int _backgroundIndex { get; set; }

    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }


    }
    private void Start()
    {
        
    }
    public void SpawnMap(int IDMap)
    {
        Runner.Spawn(maps[IDMap], Vector3.zero);

    }

    public void SpawnMap(LevelOptions.BackgroundType typeOfMap)
    {
        if (!HasStateAuthority)
            return;
        _backgroundIndex = (int)typeOfMap;

        if(typeOfMap == LevelOptions.BackgroundType.Moon)
        {
            Physics2D.gravity = new Vector2(0f, -6f);
        }
        else
        {
             Physics2D.gravity = new Vector2(0f, -9.81f);
        }

        for (int i = 0; i < mapsConfigs.Length; i++)
        {
            if(mapsConfigs[i].background == typeOfMap)
            {
                int randomMap = Random.Range(0, mapsConfigs[i].maps.Length);
                Runner.Spawn(mapsConfigs[i].maps[randomMap], Vector3.zero, Quaternion.identity);
                return;
            }
        }

    }

    public override void Spawned()
    {
        base.Spawned();
        OnBackgroundChanged();
    }
    void OnBackgroundChanged()
    {
        bgImage.sprite = LevelOptions.instance.bgOptions[_backgroundIndex];
    }

}
