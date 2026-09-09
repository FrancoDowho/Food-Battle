using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Mira : NetworkBehaviour
{
    private ChangeDetector _changeDetector;


    [SerializeField] GameObject _miraObject;

    SpriteRenderer _spr;
    public static Mira instance;
    public Vector3 rotation;
    [SerializeField] GameObject _mask;
    float _maximumMasklenght = 3.37f;
    [SerializeField] float _distance = 1.2f;
    [SerializeField] float _angleOffset = 0f;
    [SerializeField] Camera _cam;
    [SerializeField] Transform _target;

    [Networked] public bool isActive { get; set; }



    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

    }
    void Start()
    {
        _spr = _miraObject.gameObject.GetComponent<SpriteRenderer>();


    }
    public override void Render()
    {
        if (_changeDetector != null)
        {
            foreach (var change in _changeDetector.DetectChanges(this))
            {
                switch (change)
                {
                    case nameof(isActive):
                        _spr.enabled = isActive;
                        break;
                }
            }
        }
        
    }


    // Update is called once per frame
    void Update()
    {
        if (BasicSpawner.instance == null)
            return;

        if (_cam == null)
            _cam = Camera.main;

        if (_cam == null)
            return;

        var localPlayer = BasicSpawner.instance.localPlayer;

        if (localPlayer == null || !localPlayer.Object.HasInputAuthority || !localPlayer.itsMyTurn)
        {
            if (_spr != null)
                _spr.enabled = false;
            return;
        }

        if (_target == null)
        {
            if (TurnManager.instance == null || TurnManager.instance.currentCharacter == null)
            {
                if (_spr != null)
                    _spr.enabled = false;
                return;
            }

            _target = TurnManager.instance.currentCharacter.transform;
        }

        if (_target == null)
        {
            if (_spr != null)
                _spr.enabled = false;
            return;
        }

        _spr.enabled = true;

        Vector3 _mouseScreenPos = Input.mousePosition;
        _mouseScreenPos.z = Mathf.Abs(_cam.transform.position.z);

        Vector3 _mouseWorldPos = _cam.ScreenToWorldPoint(_mouseScreenPos);
        _mouseWorldPos.z = 0f;

        Vector3 _targetPos = _target.position;
        _targetPos.z = 0f;

        Vector3 _direction = _mouseWorldPos - _targetPos;
        _direction.z = 0f;

        if (_direction.sqrMagnitude <= 0.0001f)
            return;

        _direction.Normalize();
        rotation = _direction;

        transform.position = _targetPos + _direction * _distance;

        float _angle = Mathf.Atan2(_direction.y, _direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, _angle + _angleOffset);
    }

    public void ChangePJ(GameObject pos)
    {
        rotation = Vector3.zero;
        ChargeShoot(0f);

        if (pos != null)
            _target = pos.transform;
        else
            _target = null;
    }


    public override void FixedUpdateNetwork()
    {
       
    }

    public void ChargeShoot(float progress)
    {
        _mask.gameObject.transform.localScale = new Vector3(progress  * _maximumMasklenght, 20, 1);
    }

    
}
