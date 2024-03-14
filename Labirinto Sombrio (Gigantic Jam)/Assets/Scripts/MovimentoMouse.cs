using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentoMouse : MonoBehaviour
{
    public float sensibilidadeMouse = 10f;
    private float senX => GameState.SettingsData.sensibilidadeX;
    private float senY => GameState.SettingsData.sensibilidadeY;
    [SerializeField] private Vector2 maxXRotation = new Vector2(-80f, 80f);

    public Transform playerBody;
    public Transform playerHead;

    private float camRotationY;
    private Camera cam => Camera.main;
    public Vector3 raycastResult {get; private set;}
    public const int kHorizonPoint = 150;
    public float distanceFromTarget => Vector3.Distance(raycastResult, cam.transform.position);
    [SerializeField] private float distanceToInteract = 3f;
    public bool isOnInteractableDistance => distanceFromTarget <= distanceToInteract;

    [Header("Sway Settings")]
    [SerializeField] private Transform swayHolder;
    [SerializeField] private float smooth = 8f;
    [SerializeField] private float multiplier = 2f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    void Update()
    {
        if (GameState.isGamePaused) return;
        float mouseX = (Input.GetAxis("Mouse X") * sensibilidadeMouse * senX);
        float mouseY = (Input.GetAxis("Mouse Y") * sensibilidadeMouse * senY);

        camRotationY += mouseY;
        camRotationY = Mathf.Clamp(camRotationY, maxXRotation.x, maxXRotation.y);

        playerHead.localRotation = Quaternion.Euler(camRotationY, 0f,0f);
        playerBody.Rotate(Vector3.up * mouseX);

        GetTargetHealth();

        SwayWeapons();
    }

    private void SwayWeapons()
    {
        // get mouse input
        float mouseX = Input.GetAxisRaw("Mouse X") * multiplier;
        float mouseY = Input.GetAxisRaw("Mouse Y") * multiplier * 2f;

        // calculate target rotation
        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        // rotate 
        swayHolder.localRotation = Quaternion.Slerp(swayHolder.localRotation, targetRotation, smooth * Time.deltaTime);
    }

    static public int GetLayers(bool isPlayerCast = true)
    {
        if (isPlayerCast) return ~(LayerMask.GetMask("Player") | LayerMask.GetMask("ColPlayerOnly") 
                | LayerMask.GetMask("Item") | LayerMask.GetMask("ColDefaultOnly")); 
        else return ~(LayerMask.GetMask("Enemy") | LayerMask.GetMask("ColPlayerOnly") | 
                LayerMask.GetMask("Item") | LayerMask.GetMask("ColDefaultOnly"));
    }

    protected int ItemLayers() => ~(LayerMask.GetMask("Player") | LayerMask.GetMask("ColPlayerOnly"));

    public Health GetTargetHealth()
    {
        var layer = ItemLayers();
        RaycastHit rayHit;

        if(Physics.Raycast(cam.transform.position, cam.transform.forward, out rayHit, kHorizonPoint, layer))
        {
            raycastResult = rayHit.point;
            var curTransform = rayHit.transform;
            var healthObj = curTransform.GetComponentInChildren<Health>();
            var itemObj = curTransform.GetComponentInChildren<Item>();
            while (healthObj == null && curTransform.parent != null)
            {
                curTransform = curTransform.parent;
                healthObj = curTransform.GetComponent<Health>();
                if(itemObj == null) itemObj = curTransform.GetComponent<Item>();
            }
            return healthObj;
        }
        else
        {
            return null;
        }
    }
}
