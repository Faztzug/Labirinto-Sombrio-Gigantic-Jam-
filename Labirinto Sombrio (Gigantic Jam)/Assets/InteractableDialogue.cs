using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using Unity.VisualScripting.Dependencies.Sqlite;
using Unity.VisualScripting;

public interface IInteractable
{
    public string InteractableText{get;}
    public void Interact();
}

public class InteractableDialogue : MonoBehaviour, IInteractable
{
    [SerializeField] protected string interactableText = "Interagir";
    public string InteractableText => interactableText;
    [SerializeField] [TextArea(1,3)] protected string[] dialogueTexts;
    [SerializeField] protected TextMeshPro dialogueTmp;
    protected string curString = "";
    protected int curI = 0;
    protected float subsSpeed => Random.Range(8f, 20f);
    protected Tween subTween;
    protected Health thisHealth;
    [SerializeField] protected Sound speakSound;
    protected AudioSource audioSource;

    void Start()
    {
        dialogueTmp.text = "";
        thisHealth = GetComponentInChildren<Health>();
        thisHealth.OnDamage.AddListener(CheckPlayerIsAssailant);
        audioSource = GetComponentInChildren<AudioSource>();
    }

    protected void CheckPlayerIsAssailant()
    {
        Debug.Log("Assailant layer is: " + LayerMask.LayerToName(thisHealth.AssailantTrans.gameObject.layer));
        if(thisHealth.AssailantTrans.gameObject.layer == LayerMask.NameToLayer("Player")) 
        {
            Debug.Log("Disabling Interaction due Player Violence!");
            dialogueTmp.gameObject.SetActive(false);
            this.enabled = false;
        }
    }

    void OnDestroy()
    {
        thisHealth.OnDamage.RemoveListener(CheckPlayerIsAssailant);
    }

    public void Interact()
    {
        if(dialogueTexts == null || dialogueTexts.Length == 0) return;
        Debug.Log("TEXT GO!");
        if(subTween != null)
        {
            subTween?.Kill();
            ClearText();
        }
        
        subTween = DOTween.To(() => curString, x => curString = x, dialogueTexts[curI], dialogueTexts[curI].Length / subsSpeed)
        .OnUpdate(() => dialogueTmp.text = curString);

        curI++;
        curI %= dialogueTexts.Length;
        speakSound?.PlayOn(audioSource);
    }

    protected float textTimer = 10f;
    void Update()
    {
        var i = curI - 1;
        if(i < 0) i = dialogueTexts.Length - 1;
        if(curString != "" & curString == dialogueTexts[i]) textTimer -= Time.deltaTime;
        if(textTimer < 0)
        {
            textTimer = 10f;
            ClearText();
        }
    }

    protected void ClearText()
    {
        curString = "";
        dialogueTmp.text = "";
    }
}
