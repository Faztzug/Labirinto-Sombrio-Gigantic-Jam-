using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : Item
{
    public override void CollectItem(GameObject obj)
    {
        base.CollectItem(obj);
        if(obj.TryGetComponent<PlayerHealth>(out PlayerHealth hp))
        {
            if(hp.CurHealth == hp.maxHealth) return;
            var porcent = ammount / 100f;
            var value = hp.maxHealth * porcent;
            hp.GainHealth(value);
            DestroyItem();
        }
    }
}
