using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnChance : MonoBehaviour
{
    [SerializeField] [Range(0,1)] protected float chance = 0.5f;
    void Start()
    {
        var rng = Random.Range(0f,1f);
        if(chance < rng) gameObject.SetActive(false);
    }
}
