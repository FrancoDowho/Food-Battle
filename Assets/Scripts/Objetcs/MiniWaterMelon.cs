using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class MiniWaterMelon : Bullet
{

    public float explosionRadius;
    public float explosionForce;
    [SerializeField] Color _explosionParticleColor;

    public override void Spawned()
    {
        base.Spawned();

        //TurnManager.instance.
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        
    }

    protected override void DespawnMethod()
    {
        //SpawnExplosion
        var SplashParticle = Runner.Spawn(Flyweight_WeaponsPre.instance.particlesPref[0], transform.position, Quaternion.identity, Object.InputAuthority);
        SplashParticle.ChangeLifeTime(0.8f).ChangeSize(explosionRadius).ChangeColor(_explosionParticleColor);
        ExplosionMethod();
        base.DespawnMethod();




    }

    void ExplosionMethod()
    {
        Collider2D[] colisions = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var item in colisions)
        {
            if (item.gameObject.GetComponent<Rigidbody2D>() != null && item.gameObject.GetComponent<MiniWaterMelon>() == null)
            {
                var dir = (item.transform.position - transform.position).normalized;

                item.GetComponent<Rigidbody2D>().AddForce((dir * explosionForce) + Vector3.up * 500, ForceMode2D.Impulse);

                if (item.TryGetComponent<food_charac>(out food_charac chr))
                {
                    chr.RPCDamaged(dmg, 0, transform.position);
                }
            }
        }
    }
    public override void ForceThrow(Vector3 dir, float force)
    {
        base.ForceThrow(dir, force);
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        
        DespawnMethod();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    
}
