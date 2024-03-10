using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeeleWeapon : MonoBehaviour
{
    
    protected Animator anim;
    [SerializeField] protected float meeleCooldown = 0.5f;
    [SerializeField] protected string inputButton = "Fire1";
    protected float meeleTimer;
    
    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        meeleTimer -= Time.deltaTime;
        if(Input.GetButtonDown(inputButton) & meeleTimer < 0)
        {
            anim.SetTrigger("meele");
            meeleTimer = meeleCooldown;
        }
    }
}
