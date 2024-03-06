using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Health
{
    public void GainHealth(float value)
    {
        health += Mathf.Clamp(health + value, 0, maxHealth);
    }
}
