using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using UnityEngine.Rendering;
using UnityEngine.Animations.Rigging;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    public float maxHealth = 1f;
    private float _health;
    protected float health { get => _health; set { _health = Mathf.Clamp(value, 0f, maxHealth); } }
    public float CurHealth => health;
    [HideInInspector] public bool isDead = false;
    protected Animator anim;
    protected EnemyIA thisEnemy;
    [SerializeField] private GameObject bloodVFX;
    [SerializeField] private GameObject DeathVFX;
    public Sound[] damageSounds;
    public Sound[] extraDamageSounds;
    public Sound[] headshootSounds;
    public Sound deathSound;
    public AudioSource audioSource;
    protected float damageSoundTimer;
    public bool CanDoDamageSound => damageSoundTimer < 0;
    protected float alternateDamageSoundTimer;
    [SerializeField] protected bool doesDestroyOnDeath = true;
    public UnityEvent OnDeath;
    public UnityEvent OnDamage;
    public const float kEasyHealthPctg = 0.8f;
    protected Transform lastDamageTransform = null;
    public Transform AssailantTrans => lastDamageTransform;


    public virtual void Start()
    {
        thisEnemy = GetComponent<EnemyIA>();
        health = maxHealth;
        anim = GetComponentInChildren<Animator>();
        if(audioSource == null) GetComponentInChildren<AudioSource>();
    }

    protected virtual void Update() 
    {
        damageSoundTimer -= Time.deltaTime;
        alternateDamageSoundTimer -= Time.deltaTime;
    }

    public virtual void UpdateHealth(float value)
    {
        
        health += value;
        if(health <= 0) DestroyCharacter();

        if(value < 0)
        {
            if(anim != null) anim.SetTrigger("Damage");
            OnDamage?.Invoke();
        }

        if(value < 0 && health >= 0)
        {
            PlayDamageSound(damageSounds);
            PlayDamageSound(extraDamageSounds, TimerToUse.alternate);
        }
    }

    public enum TimerToUse
    {
        normal,
        alternate,
        critical,
    }

    public void PlayDamageSound(Sound[] sounds, TimerToUse timer = TimerToUse.normal)
    {
        var normal = timer == TimerToUse.normal;
        var aternateTimer = timer == TimerToUse.alternate;
        var crit = timer == TimerToUse.critical;

        if (sounds != null & sounds.Length > 0 & 
            (crit || (damageSoundTimer < 0f) || (aternateTimer & alternateDamageSoundTimer < 0f)))
        {
            var index = UnityEngine.Random.Range(0, sounds.Length);
            GameState.InstantiateSound(sounds[index], this.transform.position);

            if (aternateTimer) alternateDamageSoundTimer = 0.2f;
            else if(normal) damageSoundTimer = 0.2f;
        }
    }

    public virtual void BleedVFX(Vector3 position, Transform assailant)
    {
        if(bloodVFX != null) 
        {
            var go = GameObject.Instantiate(bloodVFX, position, Quaternion.identity, null);
            if(this.gameObject.GetComponentInChildren<Animator>() != null) go.transform.parent = this.transform;
            GameObject.Destroy(go, 3f);
        }
        lastDamageTransform = assailant;
    }

    public virtual void DestroyCharacter()
    {
        if (isDead) return;

        if (thisEnemy != null)
        {
            thisEnemy.EnemyDeath();
        }
        else if ((anim == null || anim.parameterCount == 0) & doesDestroyOnDeath)
        {
            Destroy(this.gameObject, 0f);
        }

        isDead = true;

        OnDeath?.Invoke();

        GetComponentInChildren<AudioSource>()?.Stop();
        if(DeathVFX != null) GameObject.Destroy(GameObject.Instantiate(DeathVFX, transform.position, transform.rotation, null), 5f);
        if (audioSource == null || anim == null || doesDestroyOnDeath)
        {
            GameState.InstantiateSound(deathSound, transform.position);
            Destroy(this.gameObject, 0f);
        }
        else
        {
            deathSound.PlayOn(audioSource);
        }

        if(anim != null)
        {
            foreach (var collider in GetComponentsInChildren<Collider>())
            {
                if(collider is CharacterController) continue;
                collider.enabled = false;
            }
            foreach (var script in GetComponentsInChildren<MonoBehaviour>())
            {
                if(script == this || script is GameState || script is Volume || LayerMask.LayerToName(script.gameObject.layer) == "UI") 
                {
                    continue;
                }

                if(script is EnemyIA) continue;
                if(script is MovimentoMouse) script.enabled = false;
                script.enabled = false;
            }
        }
    }
}
