using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoulAI : EnemyIA
{
    [SerializeField] protected Transform[] patrolPositions;
    protected Vector3 runAwayPos = Vector3.zero;

    protected override void Start()
    {
        base.Start();
        health.OnDamage.AddListener(RunAway);
    }

    void OnDestroy()
    {
        health.OnDamage.RemoveAllListeners();
    }

    protected override void AsyncUpdateIA()
    {
        base.AsyncUpdateIA();
        if(health.AssailantTrans != null) RunAway();
    }

    public void RunAway()
    {
        if(!agent) return;
        var assailantPos = health.AssailantTrans.position;
        var flatPos = transform.position;
        flatPos.y = 0;
        assailantPos.y = 0;
        var runDir = flatPos - assailantPos;
        agent.SetDestination(flatPos + runDir * 20f);
        //Debug.Log("Running away! TO: " + (flatPos + runDir * 50f).ToString());
        agent.speed = runSpeed;
    }

    protected override void Update()
    {
        base.Update();
        if(agent.velocity.magnitude > agent.speed 
        & Vector3.Distance(transform.position, health.AssailantTrans.position) > 19) 
        {
            agent.speed = walkingSpeed;
        }
    }
}
