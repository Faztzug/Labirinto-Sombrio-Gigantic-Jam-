using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{ 
    [SerializeField] [Range(0, 100)] protected int ammountPercent = 100;
    [SerializeField] protected Sound collectSound;
    [SerializeField] protected string _interactText = "Interagir";
    public virtual string InteractableText => string.IsNullOrWhiteSpace(_interactText) | string.IsNullOrEmpty(_interactText) ? null : _interactText;

    protected virtual void Start() { }
    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            CollectItem(other.gameObject);
        }

    }

    public virtual void CollectItem(GameObject obj)
    {
        Debug.Log("Collectiing item: " + gameObject.name);
    }

    public virtual void Interact()
    {
        CollectItem(GameState.PlayerTransform.gameObject);
    }

    public virtual void DestroyItem()
    {
        if(collectSound.clip != null) GameState.InstantiateSound(collectSound, transform.position);
        this.gameObject.SetActive(false);
    }
}