using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using DG.Tweening;

public class Movimento : MonoBehaviour
{
    [Header("Character Values")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float runSpeed = 10f;
    [SerializeField] private float runAccelaration = 3f;
    [SerializeField] private float inerciaDeccalaration = 5f;
    private float currentSpeed = 0f;
    private Vector3 movementInput = Vector3.zero;
    private Vector3 lastMovementInput = Vector3.zero;
    private float lastInputSpeed = 0f;

    [SerializeField] [Range(0.5f,1f)] private float backWardsMultiplier = 1f;
    [SerializeField] [Range(0.5f,1f)] private float strafeMultiplier = 1f;
    [SerializeField] private float jumpForce = 10f;
    private float ungroudedTime = 0.05f;
    public Vector3 LookAtRayHit{get; private set;}
    private CharacterController controller;
    private Rigidbody rgbd;
    [SerializeField] private Camera mainCam;
    private Animator anim;

    [Header("Gravity Values")]
    [SerializeField] private float gravity = 1f;
    private float gravityAcceleration;
    [SerializeField] private Sound jumpSound;
    [SerializeField] private Sound landingSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioSource passosAudio;
    [SerializeField] private Sound passosSound;
    [SerializeField] private AudioSource correrAudio;
    [SerializeField] private Sound correrSound;
    [SerializeField] private AudioSource respirarAudio;
    [SerializeField] private Sound respirarSound;

    private bool isCrouching;
    [SerializeField] private float crouchingSpeed = 3f;
    private float upwardsHeight;
    private Vector3 upwardsCenter;
    private Vector3 upwardsCamLocalPos;
    [SerializeField] private float crouchingHeight;
    [SerializeField] private Vector3 crouchingCenter;
    [SerializeField] private Vector3 crouchingCamLocalPos;
    [SerializeField] private Vector3 deathCamLocalPos;
    private RigBuilder rigBuilder;


    private void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();
        rigBuilder = GetComponentInChildren<RigBuilder>();
        UpdateIK();
        StartCoroutine(UpdateRigBuilder());
        upwardsHeight = controller.height;
        upwardsCenter = controller.center;
        upwardsCamLocalPos = mainCam.transform.localPosition;
        rgbd = GetComponent<Rigidbody>();

        passosSound?.Setup(passosAudio);
        correrSound?.Setup(correrAudio);
        respirarSound?.Setup(respirarAudio);
    }

    private void UpdateIK()
    {
    }

    IEnumerator UpdateRigBuilder()
    {
        yield return new WaitForEndOfFrame();
        rigBuilder?.Build();
    }

    private void Update()
    {
        if (!GameState.isGamePaused)
        {
            Movement();
            Animations();
            UpdateIK();
        }
        else
        {
            if(passosSound?.audioSource != null)
            {
                passosSound.audioSource.Pause();
            }
            if (correrSound?.audioSource != null)
            {
                correrSound.audioSource.Pause();
                respirarSound.audioSource.Pause();
            }
        }
    }

    private void Movement()
    {
        MoveRotation();

        MoveInput();
    }

    private void Animations()
    {
        isCrouching = Input.GetButton("Crouch");
        anim.SetBool("crouching", isCrouching);

        
        if(GameState.IsPlayerDead)
        {
            mainCam.transform.localPosition = Vector3.Lerp(mainCam.transform.localPosition, deathCamLocalPos, 0.1f);
            return;
        }

        if(isCrouching)
        {
            controller.height = Mathf.Lerp(controller.height, crouchingHeight, 0.25f);
            controller.center = Vector3.Lerp(controller.center, crouchingCenter, 0.25f);
            mainCam.transform.localPosition = Vector3.Lerp(mainCam.transform.localPosition, crouchingCamLocalPos, 0.1f);
        }
        else
        {
            controller.height = Mathf.Lerp(controller.height, upwardsHeight, 0.25f);
            controller.center = Vector3.Lerp(controller.center, upwardsCenter, 0.25f);
            mainCam.transform.localPosition = Vector3.Lerp(mainCam.transform.localPosition, upwardsCamLocalPos, 0.1f);
        }
    }
    

    void OnAnimatorIK()
    {

    }

    private void MoveRotation()
    {
        /*var camRotation = mainCam.transform.rotation;
        var objRotation = transform.rotation;
        Vector3 setRotation = new Vector3(objRotation.eulerAngles.x, camRotation.eulerAngles.y, objRotation.eulerAngles.z);
        transform.eulerAngles = setRotation;*/
    }

    private void MoveInput()
    {
        if(controller.isGrounded)
        {
            if (ungroudedTime > 0) landingSound.PlayOn(audioSource);
            ungroudedTime = 0;
            gravityAcceleration = -gravity;
        } 
        else
        {
            ungroudedTime += Time.deltaTime;
            gravityAcceleration -= gravity * Time.deltaTime;
        }

        if(ungroudedTime < 0.1f && !GameState.IsPlayerDead)
        {
            anim.SetBool("isJumping", false);

            if (Input.GetButtonDown("Jump"))
            {
                gravityAcceleration = jumpForce;
                anim.SetBool("isJumping", true);
                jumpSound.PlayOn(audioSource);
            }
        }

        Vector3 vertical = Input.GetAxis("Vertical") * transform.forward;
        if(Input.GetAxis("Vertical") < 0) vertical = Input.GetAxis("Vertical") * transform.forward * backWardsMultiplier;

        Vector3 rawHorizontal = Input.GetAxis("Horizontal") * mainCam.transform.right;
        Vector3 horizontal = rawHorizontal * strafeMultiplier;

        movementInput = (vertical + horizontal).normalized;
        if(GameState.IsPlayerDead) movementInput = Vector3.zero;

        var hasMovingInput = (Vector3.Distance(movementInput, Vector3.zero) > 0.01);
        var isRuning = Input.GetButton("Sprint");
        anim.SetBool("isRuning", isRuning && hasMovingInput && !isCrouching);

        if((currentSpeed > crouchingSpeed & isCrouching) || !hasMovingInput)
        {
            var decreaseValue = 0f;
            if(lastInputSpeed > walkSpeed) decreaseValue = inerciaDeccalaration * 1f * Time.deltaTime;
            else if(lastInputSpeed > crouchingSpeed & lastInputSpeed < runSpeed) decreaseValue = inerciaDeccalaration * 2.5f * Time.deltaTime;
            else if(lastInputSpeed <= crouchingSpeed) decreaseValue = inerciaDeccalaration * 5f * Time.deltaTime;

            currentSpeed -= decreaseValue;
            var flag = isCrouching & hasMovingInput & currentSpeed > crouchingSpeed - 0.2f && currentSpeed < crouchingSpeed + 0.2f;
            if(flag) currentSpeed = crouchingSpeed;
        }
        else if((currentSpeed > walkSpeed & !isRuning))
        {
            var decreaseValue = 0f;
            if (lastInputSpeed > walkSpeed) decreaseValue = inerciaDeccalaration * 1f * Time.deltaTime;
            else decreaseValue = inerciaDeccalaration * 2.5f * Time.deltaTime;

            currentSpeed -= decreaseValue;
            var flag = hasMovingInput && currentSpeed > walkSpeed - 0.2f && currentSpeed < walkSpeed + 0.2f;
            if(flag) currentSpeed = walkSpeed;
        }

        else if (hasMovingInput & isRuning) currentSpeed += runAccelaration * Time.deltaTime;
        else if (hasMovingInput & !isRuning) currentSpeed += runAccelaration * 2 * Time.deltaTime;

        if (lastInputSpeed <= walkSpeed & currentSpeed > walkSpeed & !isRuning) currentSpeed = walkSpeed;

        currentSpeed = Mathf.Clamp(currentSpeed, 0, runSpeed);

        if(hasMovingInput)
        {
            lastInputSpeed = currentSpeed;
            lastMovementInput = movementInput;
        }

        var movement = (hasMovingInput ? movementInput : lastMovementInput) * currentSpeed;
        movement.y += gravityAcceleration;
        movement = movement * Time.deltaTime;
        
        controller.Move(movement);
        
        var velocitylAbs = Mathf.Abs(vertical.magnitude) + Mathf.Abs(horizontal.magnitude);
        anim.SetFloat("Movement", velocitylAbs);

        anim.SetFloat("Velocidade", Mathf.Abs((vertical.magnitude * currentSpeed) / runSpeed));

        if(Input.GetAxis("Horizontal") >= 0) anim.SetFloat("Strafe", (horizontal.magnitude * currentSpeed) / runSpeed);
        else if(Input.GetAxis("Horizontal") < 0) anim.SetFloat("Strafe", (-horizontal.magnitude * currentSpeed) / runSpeed);


        if((velocitylAbs > 0.1) && controller.isGrounded && !GameState.isGamePaused)
        {
            if(currentSpeed <= walkSpeed & passosSound.IsPlaying == false && passosSound.clip)
            {
                passosSound.PlayOn(passosAudio, false);
                correrSound.audioSource.Pause();
                respirarTween.Kill();
                respirarTween = respirarSound.audioSource.DOFade(0, 2f);
            }
            if(currentSpeed > walkSpeed & !correrSound.IsPlaying)
            {
                passosSound.audioSource.Pause();
                correrSound.PlayOn(correrAudio, false);
                if(!respirarSound.IsPlaying) respirarSound.PlayOn(respirarAudio, false);
                respirarSound.audioSource.volume = 0f;
                respirarTween = respirarSound.audioSource.DOFade(respirarSound.SFXVolume, 3f);
            }
            //passosSound.audioSource.pitch = passosSound.pitch * currentSpeed / runSpeed;
        }
        else if(passosSound.audioSource != null)
        {
            passosSound.audioSource.Pause();
            correrSound.audioSource.Pause();
            if (currentSpeed <= walkSpeed)
            {
                respirarTween.Kill();
                respirarTween = respirarSound.audioSource.DOFade(0, 1f);
            }
        }
    }

    Tween respirarTween;

    public void GoToCheckPoint(Vector3 checkpoint)
    {

    }

    private void OnControllerColliderHit(ControllerColliderHit other)
    {
        if (other.rigidbody is null) return;
        var pushable = other.rigidbody.GetComponent<PushableBody>();
        pushable?.Push(rgbd);
    }
}