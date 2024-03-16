using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuitInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] protected string interactableText = "Finalizar Ciclo de Sofrimento";
    public string InteractableText => interactableText;

    public void Interact()
    {
        Application.Quit();
        Debug.LogError("VOCÊ SE MATOU!");
    }
}
