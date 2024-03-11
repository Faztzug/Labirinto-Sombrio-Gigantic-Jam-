using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonIA : EnemyIA
{
    [SerializeField] float meeleDistance = 3f;
    protected override void AsyncUpdateIA()
    {
        base.AsyncUpdateIA();

        GoToPlayerOffset();
    }

    protected override void Update()
    {
        base.Update();
        if(distance <= meeleDistance) anim.SetTrigger("meele");
    }
}
