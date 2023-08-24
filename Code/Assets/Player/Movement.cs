using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] GameObject bodyModel;
    [SerializeField] float walkSpeed = 8f;
    [SerializeField] float sprintSpeed = 15f;
    [SerializeField] float crouchSpeed = 5f;
    [SerializeField] Camera playerCamera;
    [SerializeField] float gravity = -60f;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float jumpHeight = 3.5f;
    [SerializeField] float headBobAmount = 0.1f;
    [SerializeField] float headBobSpeed = 14f;
    [SerializeField] float crouchTransition = .1f;
    [SerializeField] float normalHeight = 2f;
    [SerializeField] float crouchHeight = 1f;
    [SerializeField] LayerMask ignorePlayer;
    Vector2 horizontalInput;
    bool isCrouching;
    bool stoppingCrouching;
    bool jump;
    bool isSprinting;
    bool isGrounded;
    float distToGround = 1f;
    float speed = 11f;
    float bobTimer = 0;
    float defaultHeadPosY = 0;
    float desiredHeight = 0;
    
    Vector3 verticalVelocity = Vector3.zero;
    Vector3 hitNormal;

    public void RecieveInput(Vector2 _horizontalInput, bool _isSprinting){
        horizontalInput = _horizontalInput;
        isSprinting = _isSprinting;
    }
    // Start is called before the first frame update
    void Start()
    {
        controller.minMoveDistance = 0;
        defaultHeadPosY = playerCamera.transform.localPosition.y;
        stoppingCrouching = false;
    }

    void doHeadBob(){
        if (isCrouching){
            headBobSpeed = 10;
        }
        else{
            if (isSprinting){
            headBobSpeed = 20;
            }
            else{
                headBobSpeed = 14;
            }
        }
        
        bobTimer += Time.deltaTime * headBobSpeed;
        playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, defaultHeadPosY + Mathf.Sin(bobTimer) * headBobAmount, playerCamera.transform.localPosition.z);
    }

    void ResetHeadBob(){
        bobTimer = 0;
        playerCamera.transform.localPosition = new Vector3(playerCamera.transform.localPosition.x, Mathf.Lerp(playerCamera.transform.localPosition.y, defaultHeadPosY, Time.deltaTime * headBobAmount), playerCamera.transform.localPosition.z);
    }

    void OnControllerColliderHit (ControllerColliderHit hit) 
    {
        hitNormal = hit.normal;
    }

    // Update is called once per frame
    void Update()
    {
        // NEED TO FIX SLOPE JUMPING
        //isGrounded = CheckGrounded();
        isGrounded = controller.isGrounded;


        Vector3 horizontalVelocity = (transform.right * horizontalInput.x + transform.forward * horizontalInput.y) * speed;

        // Head Bobbing
        if (isGrounded){
            // 
            verticalVelocity.y = -.1f;
            // Activate head bobbing if grounded
            if (horizontalVelocity.magnitude > 0){
                doHeadBob();
            }
            else{
                ResetHeadBob();
            }
        }
        else{
            ResetHeadBob();
        }

        if (stoppingCrouching){
            TryDisableCrouch();
        }
        // crouching/sprinting/walking logic
        if (isCrouching){
            if (isGrounded){                 
                speed = crouchSpeed;
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, 60, .1f);
                // Perform Crouch
                desiredHeight = crouchHeight;
            }
            else{
                desiredHeight = normalHeight;
            }
        }
        else{
            // if height is smaller than normal and object close to head don't disable crouch
            desiredHeight = normalHeight;
            if (isSprinting){
                if (isGrounded){
                    speed = sprintSpeed;
                    playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, 70, .1f);
                }            
            }
            else{
                speed = walkSpeed;
                playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, 60, .1f);
            }
        }
        
        if (controller.height != desiredHeight){
            
            AdjustPlayerHeight(desiredHeight);
            //print(controller.height);

            var camPos = playerCamera.transform.localPosition;
            camPos.y = controller.center.y + 0.46f;
            playerCamera.transform.localPosition = camPos;

            var bodyPos = bodyModel.transform.localPosition;
            bodyPos.y = controller.center.y - .2f;
            bodyModel.transform.localPosition = bodyPos;
            var bodyScale = bodyModel.transform.localScale;
            bodyScale.y = controller.height/2;
            bodyModel.transform.localScale = bodyScale;

        }
        // adjust horizontal velocity to be in line with slope
        horizontalVelocity = AdjustVelocityToSlope(horizontalVelocity);
        // If the slope is too steep stop the player from being able to climb up it
        bool isGroundFlatEnough = (Vector3.Angle (Vector3.up, hitNormal) <= controller.slopeLimit);
        if (!isGroundFlatEnough) {
            isGrounded = false;
        }
        
        controller.Move(horizontalVelocity * Time.deltaTime);

        if (jump){
            if (isGrounded){
                verticalVelocity.y = Mathf.Sqrt(-2f * jumpHeight * gravity);
            }
            jump = false;
        }

        verticalVelocity.y += gravity * Time.deltaTime;
        controller.Move(verticalVelocity * Time.deltaTime);
    }

    bool CheckGrounded(){
        Vector3 centre = transform.position; //+ Vector3.down * 0.5f;
        float radius = .5f;
        Vector3 direction = Vector3.down;

        RaycastHit hit;

        //bool grounded = Physics.SphereCast(centre, radius, direction, out hit, 100f, ~ignorePlayer);

        bool grounded = Physics.SphereCast(centre, radius, direction, out hit, 0.6f, ~ignorePlayer);

        if (grounded){
            return true;
        }
        return false;
    }

    private Vector3 AdjustVelocityToSlope(Vector3 velocity){
        var ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hitinfo, 0.2f, ~ignorePlayer)){
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, hitinfo.normal);
            var adjustedVelocity = slopeRotation * velocity;

            if (adjustedVelocity.y < 0){
                return adjustedVelocity;
            }
        }
        return velocity;
    }

    public void AdjustPlayerHeight(float height){
        float center = height / 2;
        
        controller.center = Vector3.Lerp(controller.center, new Vector3(0, center, 0),Time.deltaTime * crouchTransition);
        controller.height = Mathf.Lerp(controller.height, height, Time.deltaTime * crouchTransition);    
    }

    public void EnableCrouch(){
        isCrouching = true;
    }
    public void TryDisableCrouch(){
        stoppingCrouching = true;          
        // only disable crouch if sufficient space above player
        float _playerBottom = controller.center.y; //- controller.height/2;
        var _blockingObject = false;

        Vector3 _rayPos = transform.position;
        _rayPos.y = _playerBottom;
        Ray upRay = new Ray(_rayPos, Vector3.up);
        RaycastHit[] hits = Physics.RaycastAll(_rayPos, Vector3.up, 2);

        foreach(RaycastHit hit in hits)
        {
            if(!hit.transform.IsChildOf(transform))
            {
                _blockingObject = true;
                break;
            }
        }       
        if (!_blockingObject){
            stoppingCrouching = false;
            isCrouching = false;
        } 
    }
    public void StartTryDisableCrouch(){
        if (!stoppingCrouching && isCrouching){
            stoppingCrouching = true;
        }
        
    }
    public void OnJumpPressed(){
        if (!isCrouching){
            jump = true;
        }    
    }

    
}
