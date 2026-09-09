using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class particle : NetworkBehaviour
{
    [SerializeField] float _secondsToDissapear;
    [SerializeField] float _secondsToDissapearCounter;
    SpriteRenderer _spr;
    [Networked] TickTimer _counterToDissapear { get; set; }
    [Networked] Color _sprColor { get; set; }
    float _normalizeSize;
    float _explosionRadius;
    ChangeDetector _changeDetector;

    public override void Spawned()
    {

        base.Spawned();
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);

        _normalizeSize = gameObject.transform.localScale.x;
        _spr = gameObject.GetComponent<SpriteRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public override void Render()
    {
        if (_changeDetector != null)
        {
            foreach (var change in _changeDetector.DetectChanges(this))
            {
                switch (change)
                {
                    case nameof(_sprColor):
                        _spr.color = _sprColor;
                        break;
                        
                }
            }
        }

    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        _secondsToDissapearCounter -= Runner.DeltaTime;
        _spr.color = new Color(_spr.color.r, _spr.color.g, _spr.color.b, _secondsToDissapearCounter / _secondsToDissapear);

        if (_counterToDissapear.Expired(Runner))
        {
            Runner.Despawn(Object);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public particle ChangeLifeTime(float lifeTime)
    {
        _secondsToDissapear = lifeTime;
        _secondsToDissapearCounter = lifeTime;
        _counterToDissapear = TickTimer.CreateFromSeconds(Runner, lifeTime);

        return this;
    }

    public particle ChangeColor(Color colorchosen)
    {
        _sprColor = colorchosen;
        return this;
    }

    public particle ChangeSize(float size)
    {
        _explosionRadius = size;
        float scaleFactorX = (size * 2f) / _spr.sprite.bounds.size.x;
        float scaleFactorY = (size * 2f) / _spr.sprite.bounds.size.y;
        gameObject.transform.localScale = new Vector2(scaleFactorX, scaleFactorY );
        return this;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}
