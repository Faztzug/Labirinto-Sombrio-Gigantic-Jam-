using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody), typeof(NavMeshAgent))] 
public class EnemyIA : MonoBehaviour
{
    [SerializeField] [Range(0,1)] protected float[] updateRateRNG = new float[2];
    [Range(0, 1)] protected float updateRate;
    protected float advanceUpdate = 0f;
    [HideInInspector] public Transform player => GameState.PlayerTransform;
    [SerializeField] protected Vector3 playerOffsetGoTo;
    protected NavMeshAgent agent;
    public NavMeshAgent Agent => agent;
    protected Rigidbody rgbd;
    [SerializeField] protected float lowSpeed = 3f;
    [SerializeField] protected float walkingSpeed = 5f;
    [SerializeField] protected float runSpeed = 8f;
    [SerializeField] private float baseFindPlayerDistance = 30f;
    public float CurFindPlayerDistance => GetFindPlayerDistance();
    [SerializeField] protected float minPlayerDistance = 2f;
    protected float distance => Vector3.Distance(player.position, this.transform.position);
    protected Animator anim;

    protected bool inWalkRange => distance <= CurFindPlayerDistance && distance >= minPlayerDistance;
    protected AudioSource audioSource;

    [SerializeField] private Color gizmoColor = new Color(0.7f, 0.75f, 0.2f, 0.2f);

    protected bool alive = true;

    protected Vector3 lastKnownPosition;

    protected virtual void Start() 
    {
        updateRate = Random.Range(updateRateRNG[0], updateRateRNG[1]);
        agent = GetComponent<NavMeshAgent>();
        rgbd = GetComponent<Rigidbody>();
        rgbd.maxAngularVelocity = 0;
        anim = GetComponentInChildren<Animator>();

        agent.speed = walkingSpeed;
        audioSource = GetComponentInChildren<AudioSource>();
        
        StartCoroutine(CourotineAsyncUpdateIA());
    }

    protected virtual void Update() 
    {
        if(!alive) StopMoving();
        anim.SetFloat("speed", Agent.velocity.magnitude / runSpeed);
    }

    protected IEnumerator CourotineAsyncUpdateIA()
    {
        updateRate = Random.Range(updateRateRNG[0], updateRateRNG[1]);
        updateRate = Mathf.Clamp(updateRate - advanceUpdate, 0, updateRateRNG[1]);
        advanceUpdate = 0f;

        yield return new WaitForSeconds(updateRate);

        rgbd.velocity = Vector3.zero;

        AsyncUpdateIA();

        StartCoroutine(CourotineAsyncUpdateIA());
    }

    protected virtual void AsyncUpdateIA()
    {
        
    }

    protected float GetFindPlayerDistance()
    {
        var distance = baseFindPlayerDistance;
        var litMod = GameState.IsTorchLit ? 2 : 0.5f;
        distance *= litMod *= GameState.SpeedPorcent;
        return distance;
    }

    protected bool IsPlayerAlive()
    {
        if(player != null && player.gameObject.activeSelf && GameState.IsPlayerDead == false) return true;
        else return false;
    }
    public virtual void EnemyDeath()
    {
        StopMoving();

        if(anim == null) GameObject.Destroy(this.gameObject);
        if(anim != null) anim.SetTrigger("death");

        if(agent.isOnNavMesh) agent.SetDestination(transform.position);
        if(agent.isOnNavMesh) agent.isStopped = true;
        agent.enabled = false;
        rgbd.velocity = Vector3.zero;
        rgbd.useGravity = false;
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, 10f, LayerMask.GetMask("Default"), QueryTriggerInteraction.Ignore))
        {
            transform.DOMove(hit.point, 1f).SetEase(Ease.InSine);
        }

        if(!alive) return;
        alive = false;
        this.StopAllCoroutines();
    }

    public virtual void OnDamage()
    {

    }

    public void GoToPlayerDirect(bool ignoreFindDistance = false)
    {
        if(agent.isOnNavMesh)
        {
            if((ignoreFindDistance || distance < CurFindPlayerDistance) && IsPlayerAlive()) //ignores min distance
            {
                var playerFlatPos = player.position;
                playerFlatPos.y = transform.position.y;
                var directionPlayer = playerFlatPos - transform.position;

                var thisPos = transform.position;
                agent.isStopped = false;
                var sucess = agent.SetDestination(player.position + directionPlayer * 0.5f);
                lastKnownPosition = player.position + directionPlayer * 0.5f;
            }
            else 
            {
                var pos = transform.position;
                agent.SetDestination(lastKnownPosition);
                rgbd.velocity = Vector3.zero;
                rgbd.angularVelocity = Vector3.zero;
            }
        }
        else
        {
            Debug.LogError(gameObject.name + " OUT OF NAV MESH!");
        }
    }
    protected void GoToPlayerOffset(bool goToOffset = true)
    {
        if(agent.isOnNavMesh)
        {
            if(distance > minPlayerDistance && distance < CurFindPlayerDistance && IsPlayerAlive())
            {
                var offset = goToOffset ? (player.rotation * playerOffsetGoTo) : Vector3.zero;
                agent.SetDestination(player.position + offset);
                agent.isStopped = false;
                lastKnownPosition = player.position + offset;
            }
            else 
            {
                var pos = transform.position;
                agent.SetDestination(lastKnownPosition);
                rgbd.velocity = Vector3.zero;
                rgbd.angularVelocity = Vector3.zero;
            }
        }
        else
        {
            Debug.LogError(gameObject.name + " OUT OF NAV MESH!");
        }
    }

    public void StopMoving()
    {
        if(agent.isOnNavMesh) 
        {   
            agent.SetDestination(this.transform.position);
            agent.isStopped = true;
        }
    }

    protected bool IsMoving() => agent.isOnNavMesh && !agent.isStopped && Vector3.Distance(agent.destination, transform.position) > minPlayerDistance;

    protected void TurnToPlayer()
    {
        var playerFlatPos = player.position;
        playerFlatPos.y = transform.position.y;
        var directionPlayer = playerFlatPos - transform.position;
        //var newDirection = Vector3.RotateTowards(transform.forward, directionPlayer, 30, 0);

        transform.DORotate(directionPlayer, 1f, RotateMode.FastBeyond360);
    }

    private void OnDrawGizmos() 
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, baseFindPlayerDistance);
    }
}
