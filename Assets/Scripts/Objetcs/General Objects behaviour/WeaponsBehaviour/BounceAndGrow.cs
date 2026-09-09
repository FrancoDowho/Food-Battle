using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public class BounceAndGrow : GeneralWeaponsBehaviour
{
    Vector3 _ogSize;
    [SerializeField] bool _isScaleGrowConstant;
    [SerializeField] float _growPercentage;
    [SerializeField] int _dmgIncrease;
    [SerializeField] float _dmgIncreasePercentage;
    [SerializeField] float _addToDmgIncreaseEachBounce;
    [SerializeField] int _maxBounces;
    int _currentBounces;
    public override void Spawned()
    {
        base.Spawned();
        _ogSize = gameObject.transform.localScale;
        

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isScaleGrowConstant)
        {
            gameObject.transform.localScale += (_ogSize * _growPercentage);

        }
        else gameObject.transform.localScale += gameObject.transform.localScale * _growPercentage;

        _dmgIncreasePercentage +=_addToDmgIncreaseEachBounce;
        _bulletBehaviour.dmg += Mathf.RoundToInt(_dmgIncrease * _dmgIncreasePercentage);
        _currentBounces++;
        if (_currentBounces >= _maxBounces)
            {
            _bulletBehaviour.DespawnMe();
            return;
        }


    }
    
    public override void RandomizeStatistics()
    {
        base.RandomizeStatistics();
        int chance = Random.Range(0, 2);

        _isScaleGrowConstant = chance == 0 ? false : true;

        _growPercentage = Random.Range(0.1f, 0.75f);
        _dmgIncrease = Random.Range(10, 60);
        _dmgIncreasePercentage = Random.Range(1, 2);
        _addToDmgIncreaseEachBounce = Random.Range(0.05f, 0.35f);

    }
}
