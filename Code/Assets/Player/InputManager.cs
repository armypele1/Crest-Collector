using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
# pragma warning disable 649
{
    [SerializeField] Movement movement;
    [SerializeField] Shooting shooting;
    [SerializeField] PauseMenu pauseMenu;
    [SerializeField] MouseLook mouseLook;
    PlayerControls controls;
    PlayerControls.GroundMovementActions groundMovement;
    PlayerControls.PauseActions pauseInput;

    Vector2 horizontalInput;
    Vector2 mouseInput;
    bool sprintInput;
    bool crouchInput;

    private void Awake(){
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.lockState = CursorLockMode.Confined;

        controls = new PlayerControls();
        groundMovement = controls.GroundMovement;
        pauseInput = controls.Pause;

        //groundMovement.[action].performed += context => do something
        groundMovement.HorizontalMovement.performed += ctx => horizontalInput = ctx.ReadValue<Vector2>();
        groundMovement.Jump.performed += _ => movement.OnJumpPressed();
        groundMovement.Sprint.started += _ => sprintInput = true;
        groundMovement.Sprint.canceled += _ => sprintInput = false;
        groundMovement.Crouch.started += _ => movement.EnableCrouch();
        groundMovement.Crouch.canceled += _ => movement.StartTryDisableCrouch();
        groundMovement.MouseX.performed += ctx => mouseInput.x = ctx.ReadValue<float>();
        groundMovement.MouseY.performed += ctx => mouseInput.y = ctx.ReadValue<float>();
        groundMovement.Shoot.performed += _ => shooting.OnShootPressed();
        pauseInput.Pause.performed += _ => pauseMenu.OnPausePressed(groundMovement);

    }
    private void OnEnable(){
        controls.Enable();
    }
    private void OnDisable(){
        controls.Disable();
    }


    public void DisableMovement(){
        groundMovement.Disable();
        mouseInput.x = 0;
        mouseInput.y = 0;
        mouseLook.RecieveInput(mouseInput);
        movement.RecieveInput(new Vector3(0,0,0), false);
    }
    public void EnableMovement(){
        groundMovement.Enable();
    }


    // Update is called once per frame
    void Update()
    {
        movement.RecieveInput(horizontalInput, sprintInput);
        mouseLook.RecieveInput(mouseInput);

    }
}
