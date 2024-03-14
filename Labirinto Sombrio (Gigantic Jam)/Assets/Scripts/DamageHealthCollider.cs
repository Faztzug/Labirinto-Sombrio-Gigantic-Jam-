using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;

public class DamageHealthCollider : MonoBehaviour
{
    [SerializeField] protected float damage;
    public float Damage => damage;
    private List<Health> lastDamages = new List<Health>();
    protected virtual float InvicibilityTime => 0.01f;
    [SerializeField] protected bool damageOnTrigger;
    [SerializeField] protected bool damageOnCollision = true;
    public UnityEvent OnDealtDamage = new UnityEvent();

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger collision betwwen " + gameObject.name + other.gameObject.name);
        if(!damageOnTrigger) return;
        else 
        {
            DealDamage(GetHealth(other));
        }
    }

    void OnCollisionEnter(Collision other)
    {
        Debug.Log("collision betwwen " + gameObject.name + other.gameObject.name);
        if(!damageOnCollision) return;
        else 
        {
            DealDamage(GetHealth(other));
        }
    }

    protected Health GetHealth(Collider other)
    {
        Health h;
        if(other.attachedRigidbody != null) h = GetHealth(other.attachedRigidbody.gameObject);
        else h = GetHealth(other.gameObject);
        if(h) h.BleedVFX(other.ClosestPointOnBounds(transform.position), this.transform);
        return h;
        
    } 
    protected Health GetHealth(Collision other)
    {
        Health h;
        if(other.rigidbody != null) h = GetHealth(other.rigidbody.gameObject);
        else h = GetHealth(other.gameObject);
        if(h) h.BleedVFX(other.contacts[0].point, this.transform);
        return h;
    } 

    protected void DealDamage(Health h)
    {
        if(h == null) return;
        Debug.Log("DealingDamage! to: " + h.gameObject.name);
        h.UpdateHealth(Damage);
        OnDealtDamage?.Invoke();
    }

    protected Health GetHealth(GameObject go)
    {
        var curTransform = go.transform;
        var healthObj = curTransform.GetComponentInChildren<Health>();
        while (healthObj == null && curTransform.parent != null)
        {
            curTransform = curTransform.parent;
            healthObj = curTransform.GetComponent<Health>();
        }
        
        if(lastDamages.Contains(healthObj)) return null;
        if(healthObj != null && !lastDamages.Contains(healthObj))
        {
            lastDamages.Add(healthObj);
            EndEnemyInvicibility(healthObj);
        }
        return healthObj;
    }

    protected async void EndEnemyInvicibility(Health healthObj)
    {
        await Task.Delay(Mathf.RoundToInt(InvicibilityTime * 1000));
        //Debug.Log("removing health of " + healthObj?.gameObject.ToString());
        if(this == null) return;
        lastDamages.Remove(healthObj);
    }
    
    private void OnValidate() 
    {
        if(damage > 0) damage = -damage;
    }

    public void SetDamage(float dmg) => damage = dmg;
}
