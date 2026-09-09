using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class food_charac : NetworkBehaviour, Idamagable
{
    public enum AnimState{Idle, PreparaLanzamiento,Lanza, hurt, Win

    };
    public int life;
    [HideInInspector] public int maxlife;
    [SerializeField] Bullet _bullPrefab;
    [SerializeField] bool _isAVeggie;
    Rigidbody2D _rb;
    [SerializeField] Animator _anim;
    ChangeDetector _changeDetector;
    [Networked] int _currentAnim { get; set; }
     public HeartLifeBar heartLifeHUD;

    [HideInInspector]public Vector3 rotation;
    [SerializeField] float _offsetArmRotation;

    bool _isGrounded;
    private float lastYVelocity;

    // Start is called before the first frame update
    void Start()
    {
        //ELIMINAR DSP
        _rb = gameObject.GetComponent<Rigidbody2D>();
        _anim = gameObject.GetComponent<Animator>();

    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();

       
    }
    public override void Spawned()
    {

        maxlife = life;
        StartCoroutine(testi());
        /*_anim.runtimeAnimatorController =
            FlyWeightControllerAnimations.instance.veggiesCharacters[0];*/
        if (_isAVeggie)
        {
            TurnManager.instance.vegetables.Add(this);

        }
        else
        {
            TurnManager.instance.fruits.Add(this);
            
        }
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);


        gameObject.transform.parent = null;
        var heart = FlyWeightControllerAnimations.instance.heartsToGrab[0];
        
        heartLifeHUD = heart;
        heart.myChar = this.gameObject;
        FlyWeightControllerAnimations.instance.heartsToGrab.Remove(heart);

        //heartLifeHUD = Runner.Spawn(FlyWeightControllerAnimations.instance.prefabHeartLifeBar,transform.position,transform.rotation, Object.InputAuthority);
        //heartLifeHUD.myChar = this.gameObject;


    }

    
    IEnumerator testi()
    {
        yield return new WaitForSeconds(0.05f);
        if(_isAVeggie)
        _anim.runtimeAnimatorController =
            FlyWeightControllerAnimations.instance.veggiesCharacters[Random.Range(0, FlyWeightControllerAnimations.instance.veggiesCharacters.Length)];
        else _anim.runtimeAnimatorController =
           FlyWeightControllerAnimations.instance.FruitsCharacters[Random.Range(0, FlyWeightControllerAnimations.instance.FruitsCharacters.Length)];

    }

    // Update is called once per frame
    void Update()
    {
        if (!HasStateAuthority)
            return;

        lastYVelocity = _rb.velocity.y;
        
    }
    public override void Render()
    {
        if (_changeDetector != null)
        {
            foreach (var change in _changeDetector.DetectChanges(this))
            {
                switch (change)
                {
                    case nameof(_currentAnim):

                        if (_currentAnim == 1)
                        {
                            _anim.SetBool("IsCharging", true);


                        }
                        if (_currentAnim == 2)
                            _anim.SetBool("IsCharging", false);
                        if (_currentAnim == 3)
                        {
                            _anim.SetTrigger("Hurt");
                            _currentAnim = 0;
                        }
                        if (_currentAnim == 4)
                            _anim.SetBool("Win", true);

                        break;
                }
            }
        }

    }
    public void ChangeAnim(AnimState animacionAEligir)
    {
        if (animacionAEligir == AnimState.PreparaLanzamiento)
            _currentAnim = 1;
        if (animacionAEligir == AnimState.Lanza)
            _currentAnim = 2;
        if (animacionAEligir == AnimState.hurt)
            _currentAnim = 3;
        if (animacionAEligir == AnimState.Win)
            _currentAnim = 4;


    }


    public void Shoot(Vector3 direction, float force, Bullet proyectile, float tToExplode)
    {
        direction.z = 0f;

        if (direction.sqrMagnitude <= 0.0001f)
            return;

        direction.Normalize();

        float _spawnOffset = 0.6f;
        Vector3 _spawnPosition = transform.position + direction * _spawnOffset;
        _spawnPosition.z = 0f;

        float _angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion _bulletRotation = Quaternion.Euler(0f, 0f, _angle);

        var _bullet = Runner.Spawn(proyectile, _spawnPosition, _bulletRotation, Object.InputAuthority);
        _bullet.timeToExplode = tToExplode;
        _bullet.ForceThrow(direction, force);
    }

    public void RPCDamaged(float dmg, float hitForce, Vector2 pos)
    {
        if (FlyWeightGameVariables.instance.isSensibleModeActive)
            life = 0;

        dmg *= FlyWeightGameVariables.instance.dmgMultiplier;
        int finalDamage = Mathf.RoundToInt(dmg);
        life -= finalDamage;

        Debug.Log(finalDamage);

        var txt = Runner.Spawn(
            FlyWeightControllerAnimations.instance.hurtNumbersPopUp,
            transform.position,
            Quaternion.identity,
            Object.InputAuthority
        );

        txt.GetComponent<TxtDmgPrefab>().SetDamage(finalDamage);

        if (life <= 0)
        {
            if (_isAVeggie)
                TurnManager.instance.vegetables.Remove(this);
            else
                TurnManager.instance.fruits.Remove(this);

            Runner.Despawn(Object);
            return;
        }

        ChangeAnim(AnimState.hurt);

        if (!lifeObserverManager.instance.charactersHurt.Contains(this))
            lifeObserverManager.instance.charactersHurt.Add(this);

        if (hitForce == 0)
            return;

        hitForce /= 16;
        float defaultUpForce = 5;
        int direction = 1;

        if (pos.x >= transform.position.x)
            direction *= -1;

        _rb.AddForce(
            (hitForce * Vector2.right * 3 * direction * _rb.mass) +
            (defaultUpForce * Vector2.up * _rb.mass),
            ForceMode2D.Impulse
        );
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            if (_isAVeggie)
            {
                TurnManager.instance.vegetables.Remove(this);

            }
            else TurnManager.instance.fruits.Remove(this);
            lifeObserverManager.instance.charactersHurt.Remove(this);

            Runner.Despawn(Object);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            _isGrounded = true;
            float impactSpeed = Mathf.Abs(lastYVelocity);

            if (impactSpeed > FlyWeightGameVariables.instance.fallThreshold)
            {
                float fallDmg = (impactSpeed - FlyWeightGameVariables.instance.fallThreshold) * FlyWeightGameVariables.instance.fallDmgMultiplier;
                RPCDamaged(fallDmg, 0, transform.position);

            }


        }
        if (collision.gameObject.TryGetComponent<food_charac>(out food_charac ch))
        {
            ch.RPCDamaged(10, 15, transform.position);

        }

        


    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 3)
        {
            _isGrounded = false;
        }
    }
}
