using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : Item
{
    protected override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        if(other.GetComponentInChildren<EnemyIA>()) CollectItem(other.gameObject);
    }

    public override void CollectItem(GameObject obj)
    {
        base.CollectItem(obj);
        if(obj.TryGetComponent<Health>(out Health hp))
        {
            if(hp.CurHealth == hp.maxHealth) return;
            var porcent = ammountPercent / 100f;
            var value = hp.maxHealth * porcent;
            hp.UpdateHealth(value);
            DestroyItem();
        }
    }
}
