using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonIA : EnemyIA
{
    [SerializeField] float meeleDistance = 3f;
    [SerializeField] protected Sound[] meeleGrunt;
    [SerializeField] protected Sound foundSound;
    protected override void AsyncUpdateIA()
    {
        base.AsyncUpdateIA();

        var wasSeeing = seeingPlayer;
        GoToPlayerOffset();
        if(!wasSeeing & seeingPlayer) foundSound?.PlayOn(audioSource);
    }

    protected override void Update()
    {
        base.Update();
        if(distance <= meeleDistance) 
        {
            anim.SetTrigger("meele");
            var r= Random.Range(0, meeleGrunt.Length);
            meeleGrunt[r]?.PlayOn(audioSource);
        }
    }
}
