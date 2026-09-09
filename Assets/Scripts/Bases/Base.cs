using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base : NetworkBehaviour
{
    NetworkCharacterController _networkController;
    BaseController _controller;
    BaseModel _model;
    BaseView _view;

    public float throwForce;
    public float throwForceUp;

    public int health;

    [SerializeField] float _delaySeconds;

    SpriteRenderer _renderer;

    [SerializeField] Sprite _junk;

    [Networked] public bool junkFood { get; set; }

    ChangeDetector _changeDetector;

    private void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public override void Spawned()
    {
        //_changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        _networkController = GetComponent<NetworkCharacterController>();
        _model = new BaseModel(this);
        _view = new BaseView();
        _controller = new BaseController(this, _delaySeconds);

        _model.Start();
    }

    /*public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(junkFood):
                    _renderer.sprite = _junk;

                    _renderer.color = Color.red;
                    break;
            }
        }
    }*/

    public override void FixedUpdateNetwork()
    {
       // _controller.Update();
    }

    public void TakeDmg(int dmg)
    {
        _model.TakeDmg(dmg, ref health);
    }
}
