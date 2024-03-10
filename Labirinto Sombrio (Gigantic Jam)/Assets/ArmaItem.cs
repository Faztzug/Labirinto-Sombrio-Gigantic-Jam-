using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmaItem : Item
{
    public override void CollectItem(GameObject obj)
    {
        base.CollectItem(obj);
        foreach (var item in obj.GetComponentsInChildren<MeeleWeapon>(true))
        {
            if(item.gameObject.activeInHierarchy == false) 
            {
                item.gameObject.SetActive(true);
                DestroyItem();
            }
        }
    }
}
