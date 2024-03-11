using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class PlayerHealth : Health
{
    [SerializeField] protected Volume damageVolume;
    public void GainHealth(float value)
    {
        health += Mathf.Clamp(health + value, 0, maxHealth);
    }

    public override void UpdateHealth(float value)
    {
        base.UpdateHealth(value);
        damageVolume.weight = 1f - (CurHealth / maxHealth);
    }

    public override void DestroyCharacter()
    {
        base.DestroyCharacter();
        StartCoroutine(ReloadScene());
    }

    IEnumerator ReloadScene()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(gameObject.scene.name);
    }
}
