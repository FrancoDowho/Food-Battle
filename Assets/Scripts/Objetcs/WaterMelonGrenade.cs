using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class WaterMelonGrenade : Bullet
{
    public float explosionRadius;
    public float explosionForce;

    [SerializeField] TMP_Text _txtCooldown;
    [SerializeField] int _splitInto;
    [SerializeField] MiniWaterMelon _miniWaterMelonPrefab;
    [SerializeField] Color _explosionParticleColor;
    [SerializeField] float _explodeAfterSeconds = 3f;

    [Networked] TickTimer _explodeTimer { get; set; }

    bool _hasExploded;

    public override void Spawned()
    {
        base.Spawned();

        _hasExploded = false;
        _explodeTimer = TickTimer.CreateFromSeconds(Runner, _explodeAfterSeconds);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

        if (_hasExploded)
            return;

        if (_explodeTimer.Expired(Runner))
        {
            _hasExploded = true;
            DespawnMethod();
        }
    }

    public override void Render()
    {
        UpdateCooldownText();
        FixCooldownRotation();
    }

    void UpdateCooldownText()
    {
        if (_txtCooldown == null)
            return;

        if (!_explodeTimer.IsRunning)
        {
            _txtCooldown.text = "0";
            return;
        }

        float remainingTime = _explodeTimer.RemainingTime(Runner) ?? 0f;
        remainingTime = Mathf.Max(0f, remainingTime);

        int displayValue = Mathf.Max(0, Mathf.CeilToInt(remainingTime));
        _txtCooldown.text = displayValue.ToString();
    }

    void FixCooldownRotation()
    {
        if (_txtCooldown == null)
            return;

        _txtCooldown.transform.localRotation = Quaternion.Inverse(transform.rotation);
    }

    protected override void DespawnMethod()
    {
        if (Runner == null || Object == null || !Object.IsValid)
            return;

        var splashParticle = Runner.Spawn(
            Flyweight_WeaponsPre.instance.particlesPref[0],
            transform.position,
            Quaternion.identity,
            Object.InputAuthority
        );

        splashParticle
            .ChangeLifeTime(2)
            .ChangeSize(explosionRadius)
            .ChangeColor(_explosionParticleColor);

        ExplosionMethod();
        SpawnMinis(_splitInto);

        base.DespawnMethod();
    }

    void SpawnMinis(int howMany)
    {
        float defaultUpForce = 3f;
        HashSet<int> usedHorizontalForces = new HashSet<int>();

        for (int i = 0; i < howMany; i++)
        {
            int randomHorizontal = Random.Range(-10, 10);
            int safety = 0;

            while (usedHorizontalForces.Contains(randomHorizontal) && safety < 50)
            {
                randomHorizontal = Random.Range(-10, 10);
                safety++;
            }

            usedHorizontalForces.Add(randomHorizontal);

            var miniWatermelon = Runner.Spawn(
                _miniWaterMelonPrefab,
                transform.position + Vector3.up,
                transform.rotation,
                Object.InputAuthority
            );

            if (miniWatermelon.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                float upForce = Random.Range(1f, 10f);
                Vector2 force = (defaultUpForce * Vector2.up * upForce) + (randomHorizontal * Vector2.right);
                rb.AddForce(force, ForceMode2D.Impulse);
            }
        }
    }

    void ExplosionMethod()
    {
        Collider2D[] collisions = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var item in collisions)
        {
            if (item.TryGetComponent<Rigidbody2D>(out Rigidbody2D rb))
            {
                Vector2 dir = (item.transform.position - transform.position).normalized;
                rb.AddForce((dir * explosionForce) + (Vector2.up * 500f), ForceMode2D.Impulse);
            }

            if (item.TryGetComponent<food_charac>(out food_charac chr))
            {
                chr.RPCDamaged(dmg, 0, transform.position);
            }
        }
    }

    public override void ForceThrow(Vector3 dir, float force)
    {
        base.ForceThrow(dir, force);
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}