using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonIA : EnemyIA
{
    protected override void AsyncUpdateIA()
    {
        base.AsyncUpdateIA();

        GoToPlayerOffset();
    }
}
