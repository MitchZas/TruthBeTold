using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public bool canMove {get; set;} = true;
    private bool ShouldCrouch => (Input.GetKeyDown(crouchKey)||Input.GetKeyUp(crouchKey)) && !duringCrouchAnimation && characterController.isGrounded;

    public BookController bookControllerScript;

    [Header("Functional Options")]
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canUseHeadbob = true;
    [SerializeField] private bool useFootsteps = true;
    [SerializeField] private bool canZoom = true;
    [SerializeField] private bool bookVisible = false;

    
    [Header("Controls")]
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] private KeyCode zoomKey = KeyCode.Mouse1;
    
    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float gravity = 30.0f;
    [SerializeField] private float crouchSpeed = 1.5f;
    
    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;

    [Header("Crouch Parameters")]
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standHeight = 2.0f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0,0.5f,0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0,0,0);
    private bool isCrouching;
    private bool duringCrouchAnimation;

    [Header("Headbob Parameters")]
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float crouchBobSpeed = 8f;
    [SerializeField] private float crouchBobAmount = 0.025f;
    private float defaultYPos = 0;
    private float timer;

    [Header("Zoom Parameters")]
    [SerializeField] private float timeToZoom = 0.3f;
    [SerializeField] private float zoomFOV = 30f;
    private float defaultFOV;
    private Coroutine zoomRoutine;

    [Header("Footstep Parameters")]
    [SerializeField] private float baseStepSpeed = 0.5f;
    [SerializeField] private float crouchStepMultipler = 1.5f;
    [SerializeField] private AudioSource footstepAudioSource = default;
    [SerializeField] private AudioClip[] woodClips = default;
    [SerializeField] private AudioClip[] metalClips = default;
    [SerializeField] private AudioClip[] grassClips = default;
    [SerializeField] private AudioClip[] concreteClips = default;
    private float footstepTimer = 0f;
    private float GetCurrentOffset => isCrouching ? baseStepSpeed * crouchStepMultipler : baseStepSpeed;

    private Camera playerCamera;
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;


    public GameObject mainCamera;
    public KeyCode OpenClose;

    void Awake()
    {
        playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        defaultYPos = playerCamera.transform.localPosition.y;
        defaultFOV = playerCamera.fieldOfView;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            HandleMovementInput();
            HandleMouseLook();

            if (canCrouch)
                HandleCrouch();

            if (canUseHeadbob)
                HandleHeadbob();

            if (useFootsteps)
                Handle_Footsteps();

            if(canZoom)
                HandleZoom();

            ApplyFinalMovements();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("Book is opened");
        }
    }

    private void HandleMovementInput()
    {
        currentInput = new Vector2((isCrouching ? crouchSpeed :walkSpeed) * Input.GetAxis("Vertical"), (isCrouching ? crouchSpeed :walkSpeed) * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDirection.y;

        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
    }

    private void HandleMouseLook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX,-upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }

    private void HandleCrouch()
    {
        if(ShouldCrouch)
        StartCoroutine(CrouchStand());
    }

    private void HandleHeadbob()
    {
        if(!characterController.isGrounded) return;

        if(Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : walkBobSpeed);
            playerCamera.transform.localPosition = new Vector3(
            playerCamera.transform.localPosition.x,
            defaultYPos + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : walkBobAmount),
            playerCamera.transform.localPosition.z);
        }
    }

    private void HandleZoom()
    {
        if(Input.GetKeyDown(zoomKey))
        {
            if(zoomRoutine != null)
            {
               StopCoroutine(zoomRoutine);
               zoomRoutine = null;
            }

            zoomRoutine = StartCoroutine(ToggleZoom(true));
        }

        if(Input.GetKeyUp(zoomKey))
        {
            if(zoomRoutine != null)
            {
               StopCoroutine(zoomRoutine);
               zoomRoutine = null;
            }

            zoomRoutine = StartCoroutine(ToggleZoom(false));
        }
    }

    private void Handle_Footsteps()
    {
       if (!characterController.isGrounded) return;
       if (currentInput == Vector2.zero) return;

       footstepTimer -= Time.deltaTime;

       if(footstepTimer <= 0)
       {
            if(Physics.Raycast(playerCamera.transform.position, Vector3.down, out RaycastHit hit, 3))
            {
                switch(hit.collider.tag)
                {
                    case "Footsteps/Wood":
                    footstepAudioSource.PlayOneShot(woodClips[UnityEngine.Random.Range(0, woodClips.Length -1)]);
                        break;
                    case "Footsteps/Grass":
                    footstepAudioSource.PlayOneShot(grassClips[UnityEngine.Random.Range(0, grassClips.Length -1)]);
                        break;
                    case "Footsteps/Metal":
                    footstepAudioSource.PlayOneShot(metalClips[UnityEngine.Random.Range(0, metalClips.Length -1)]);
                        break;
                    case "Footsteps/Concrete":
                    footstepAudioSource.PlayOneShot(concreteClips[UnityEngine.Random.Range(0, concreteClips.Length -1)]);
                        break;
                }
            }

            footstepTimer = GetCurrentOffset;
       }
    }
    
    private void ApplyFinalMovements()
    {
        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

            characterController.Move(moveDirection * Time.deltaTime);
    }


    void RaycastCheck()
        {
            RaycastHit hit;

            if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward), out hit, 2.3f))
            {
                if (hit.collider.gameObject.GetComponent<SimpleOpenClose>())
                {
                    // Debug.Log("Object with SimpleOpenClose script found");
                    hit.collider.gameObject.BroadcastMessage("ObjectClicked");
                }

                else
                {
                    // Debug.Log("Object doesn't have script SimpleOpenClose attached");

                }
                // Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                // Debug.Log("Did Hit");
            }
            else
            {
                // Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.TransformDirection(Vector3.forward) * 1000, Color.white);
                //   Debug.Log("Did not Hit");


            }

        }

    private IEnumerator CrouchStand()
    {
        if(isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
            yield break;
        
        duringCrouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;

        isCrouching = !isCrouching;
        
        while(timeElapsed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed/timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed/timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;

        duringCrouchAnimation = false;
    }

    private IEnumerator ToggleZoom(bool isEnter)
    {
        float targetFOV = isEnter ? zoomFOV : defaultFOV;
        float startingFOV = playerCamera.fieldOfView;
        float timeElapsed = 0f;

        while(timeElapsed <timeToZoom)
        {
            playerCamera.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed / timeToZoom);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.fieldOfView = targetFOV;
        zoomRoutine = null;
    }
}
