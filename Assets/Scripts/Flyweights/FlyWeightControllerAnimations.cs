using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyWeightControllerAnimations : MonoBehaviour
{
    public static FlyWeightControllerAnimations instance;
    public RuntimeAnimatorController[] veggiesCharacters;
    public RuntimeAnimatorController[] FruitsCharacters;

    public HeartLifeBar prefabHeartLifeBar;
    public List<HeartLifeBar> heartsToGrab = new List<HeartLifeBar>();

    public Sprite[] armSprites;
    public GameObject hurtNumbersPopUp;
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
