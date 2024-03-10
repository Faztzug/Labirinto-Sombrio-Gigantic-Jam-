using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchBatteryItem : Item
{
    public override void CollectItem(GameObject obj)
    {
        base.CollectItem(obj);
        var torch = obj.GetComponentInChildren<TorchLight>();
        torch.TorchBattery += torch.TorchMaxBattery * ((float)ammountPercent / 100f);
        Debug.Log((torch.TorchMaxBattery * ((float)ammountPercent / 100f)).ToString());
    }
}
