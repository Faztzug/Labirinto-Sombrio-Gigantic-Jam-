using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class InteractionRaycast : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI canvasInteractor; 
    [SerializeField] protected float range = 5f; 
    protected Transform camTrans;
    protected string interectButton = "E: ";

    void Start()
    {
        camTrans = Camera.main.transform;
        canvasInteractor.gameObject.SetActive(false);
    }
    
    void Update()
    {
        if(GameState.isGamePaused) return;
        RaycastHit rayHit;
        IInteractable item = null;
        if(Physics.SphereCast(camTrans.position, 0.2f, camTrans.forward, out rayHit, range, ~(LayerMask.GetMask("Player")), QueryTriggerInteraction.UseGlobal))
        {
            if(rayHit.rigidbody != null) item = rayHit.rigidbody.gameObject.GetComponentInChildren<IInteractable>(false);
            else item = rayHit.collider.GetComponentInChildren<IInteractable>(false);
            if(item != null) 
            {
                if((item as MonoBehaviour).isActiveAndEnabled)
                {
                    canvasInteractor.text = interectButton + item.InteractableText;
                    if(Input.GetButtonDown("Use")) item.Interact(); 
                }
                else item = null;
            }
        }
        canvasInteractor.gameObject.SetActive(item != null);
    }
}
