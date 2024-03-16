using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchBatteryItem : Item
{
    protected AudioSource audioSource;

    protected override void Start()
    {
        base.Start();
        audioSource = GetComponentInChildren<AudioSource>();
    }
    public override void CollectItem(GameObject obj)
    {
        base.CollectItem(obj);
        var torch = obj.GetComponentInChildren<TorchLight>();
        torch.TorchBattery += torch.TorchMaxBattery * ((float)ammountPercent / 100f);
        Debug.Log((torch.TorchMaxBattery * ((float)ammountPercent / 100f)).ToString());
        collectSound?.PlayOn(audioSource);
    }
}
