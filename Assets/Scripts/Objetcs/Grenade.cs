using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class Grenade : Bullet
{
    // Start is called before the first frame update
    private ChangeDetector _changeDetector;

    public float explosionRadius;
    public float explosionForce;
    [SerializeField] TMP_Text _txt_cooldown;
    [Networked] float _counter { get; set; }

    public override void Spawned()
    {
        base.Spawned();
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
        

        StartCoroutine(startLater());

        //TurnManager.instance.
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public override void Render()
    {
        foreach (var change in _changeDetector.DetectChanges(this))
        {
            switch (change)
            {
                case nameof(_counter):
                    _txt_cooldown.text = Mathf.Round(_counter).ToString();
                    break;
                
            }
        }
    }
    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        _counter -= Runner.DeltaTime;
        



        _txt_cooldown.gameObject.transform.rotation = new Quaternion(0, 0, -transform.rotation.eulerAngles.z +transform.rotation.eulerAngles.z, 0);

    }

    protected override void DespawnMethod()
    {
        //SpawnExplosion
        var SplashParticle = Runner.Spawn(Flyweight_WeaponsPre.instance.particlesPref[0], transform.position, Quaternion.identity, Object.InputAuthority);
        SplashParticle.ChangeLifeTime(2).ChangeSize(explosionRadius);
        ExplosionMethod();
        base.DespawnMethod();


    }

    void ExplosionMethod()
    {
        Collider2D[] colisions = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        Debug.Log(colisions.Length);

        foreach (var item in colisions)
        {
            if (item.gameObject.GetComponent<Rigidbody2D>() != null)
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
        //nada
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    IEnumerator startLater()
    {
        yield return new WaitForSeconds(0.03f);
        _counter = timeToExplode;
    }

}
