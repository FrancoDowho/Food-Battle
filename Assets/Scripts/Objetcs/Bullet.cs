using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public enum DestroysUponContactWith
    {
        Anything, OnlyCharacters, DoesntDestroy
    }
    public DestroysUponContactWith DestroyUponContact;
    Rigidbody2D _rb;
    CircleCollider2D _cc2d;


    [Networked] TickTimer _counterForColliders { get; set; }
    [Networked] TickTimer _counterAutoDestruct { get; set; }

    public float timeToExplode = 20;
    [SerializeField] public int dmg;
    [SerializeField] protected float _cooldownToColliders;
    [HideInInspector] public Vector2 dir;
    [HideInInspector] public float forceShoot;
    public bool isGrenadeType;
    public float knockBackForceMultiplier;
    Spin _spinBehaviour;

    public override void Spawned()
    {
        TurnManager.instance.currentAmountOfBulletsOnScreen++;
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _cc2d = gameObject.GetComponent<CircleCollider2D>();
        _cc2d.enabled = false;
        _spinBehaviour = gameObject.GetComponent<Spin>();

        _counterForColliders = TickTimer.CreateFromSeconds(Runner, _cooldownToColliders);
        StartCoroutine(Delay(20));
        base.Spawned();
    }
    public virtual void ForceThrow(Vector3 dir, float force)
    {
        dir.z = 0f;

        if (dir.sqrMagnitude <= 0.0001f)
            return;

        dir.Normalize();

        this.dir = dir;
        forceShoot = force;

        if (_rb == null)
            _rb = GetComponent<Rigidbody2D>();

        _rb.velocity = Vector2.zero;
        _rb.angularVelocity = 0f;

        float _angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, _angle);

        _rb.AddForce((Vector2)dir * force, ForceMode2D.Impulse);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (_counterForColliders.Expired(Runner))
            _cc2d.enabled = true;

        if (_counterAutoDestruct.Expired(Runner))
        {
            DespawnMethod();

        }

    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        var hit = collision.gameObject.GetComponent<Idamagable>();
        if (hit != null)
        {
            hit.RPCDamaged(dmg, _rb.velocity.magnitude * knockBackForceMultiplier, transform.position);
        }
        

        if (DestroyUponContact == DestroysUponContactWith.Anything)
        {
            DespawnMethod();
            
        }
        else if(DestroyUponContact == DestroysUponContactWith.OnlyCharacters)
        {
            if (hit != null)
            {
                DespawnMethod();
            }
        }

        

       
        //FIXME cambiar magnitude por SQRmagnitud, preguntar como era el calculo
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
            DespawnMethod();

    }

    protected virtual void DespawnMethod()
    {
        TurnManager.instance.currentAmountOfBulletsOnScreen--;

        Runner.Despawn(Object);
        
        

    }

    public void DespawnMe()
    {
        DespawnMethod();
    }

    protected IEnumerator Delay(float defTime)
    {
        yield return new WaitForSeconds(0.03f);
        if (!isGrenadeType)
        _counterAutoDestruct = TickTimer.CreateFromSeconds(Runner, defTime);
        else _counterAutoDestruct = TickTimer.CreateFromSeconds(Runner, timeToExplode);


    }

    

}
