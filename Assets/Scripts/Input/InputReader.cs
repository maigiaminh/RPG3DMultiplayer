using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;
using static LookControl;

public class InputReader : MonoBehaviour, NewControls.IPlayerInputActions
{
    public bool IsAttacking { get; private set; }
    public bool IsSprinting { get; private set; }
    public bool IsSecondaryPerforming { get; private set; }
    public Vector2 MovementValue { get; private set; }
    public Vector2 ZoomValue { get; private set; }
    public event Action JumpEvent;
    public event Action DodgeEvent;
    public event Action TargetEvent;
    public event Action CrouchEvent;
    public event Action RollEvent;
    public event Action ChangeWeaponEvent;
    public event Action TestEvent;
    public event Action<int> SkillEvent;
    public event Action OpenInventoryEvent;
    public event Action OpenSkillPanelEvent;
    public event Action OpenDungeonDescriptPanelEvent;
    public event Action OpenDialogeEvent;
    public event Action OpenEcsModeEvent;
    public event Action OpenMouseModeEvent;
    public event Action OpenStatBoardEvent;
    public event Action OpenMapEvent;
    public event Action PickUpEvent;
    private NewControls newControls;
    private const string RebindsKey = "rebinds";
    private string defaultBindingsJson;
    
    private void Start()
    {
        string rebinds = PlayerPrefs.GetString(RebindsKey);
        if(newControls != null) { return; }

        InitBindings();
        
        defaultBindingsJson = InputActionRebindingExtensions.SaveBindingOverridesAsJson(newControls);

        if (!string.IsNullOrEmpty(rebinds)){
            InputActionRebindingExtensions.LoadBindingOverridesFromJson(newControls, rebinds);
            Debug.Log("LOAD REBINDS " + rebinds);
        }
    }

    public void SaveBindings()
    {
        string rebinds = PlayerPrefs.GetString(RebindsKey);
        
        Debug.Log("SAVE BIND " + rebinds);

        if (!string.IsNullOrEmpty(rebinds)){
            newControls.Disable();
            newControls.RemoveAllBindingOverrides();
            newControls.LoadBindingOverridesFromJson(rebinds);
            newControls.Enable();
            newControls.PlayerInput.SetCallbacks(this);
        }
    }

    public void ResetBindind(){
        if(PlayerPrefs.HasKey(RebindsKey))
            PlayerPrefs.DeleteKey(RebindsKey);

        if(!string.IsNullOrEmpty(defaultBindingsJson))
            InputActionRebindingExtensions.LoadBindingOverridesFromJson(newControls, defaultBindingsJson);

        else {
            if(newControls != null){
                InputActionRebindingExtensions.RemoveAllBindingOverrides(newControls);
            }
        }
    }

    private void InitBindings()
    {
        newControls = new NewControls();
        newControls.PlayerInput.SetCallbacks(this);
        newControls.PlayerInput.Enable();
    }


    private void OnDestroy()
    {
        if(newControls == null) { return; }
        newControls.PlayerInput.Disable();
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        
        JumpEvent?.Invoke();
    }

    public void OnDogde(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        
        DodgeEvent?.Invoke();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MovementValue = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {

    }

    public void OnTarget(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }

        TargetEvent?.Invoke();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IsAttacking = true;
        }
        else if (context.canceled)
        {
            IsAttacking = false;
        }
    }

    public void OnSecondary(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IsSecondaryPerforming = true;
        }
        else if (context.canceled)
        {
            IsSecondaryPerforming = false;
        }    
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            IsSprinting = true;
        }
        else if (context.canceled)
        {
            IsSprinting = false;
        }
    }
    
    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        
        CrouchEvent?.Invoke();
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }

        RollEvent?.Invoke();
    }

    public void OnSealthUnsealth(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        
        ChangeWeaponEvent?.Invoke();
    }

    public void OnTest(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }

        TestEvent?.Invoke();
    }

    public void OnSkill1(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }

        SkillEvent?.Invoke(0);
    }

    public void OnSkill2(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }

        SkillEvent?.Invoke(1);
    }

    public void OnSkill3(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }

        SkillEvent?.Invoke(2);
    }

    public void OnSkill4(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }

        SkillEvent?.Invoke(3);
    }

    public void OnInventory(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        OpenInventoryEvent?.Invoke();
    }

    public void OnSkillPanel(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        OpenSkillPanelEvent?.Invoke();
    }

    public void OnDungeonDescript(InputAction.CallbackContext context)
    {
        if(!context.performed) { return; }
        OpenDungeonDescriptPanelEvent?.Invoke();
    }

    public void OnDialoge(InputAction.CallbackContext context)
    {
        if(!context.performed) { return; }
        OpenDialogeEvent?.Invoke();
    }
    public void OnEcsMode(InputAction.CallbackContext context)
    {
        if(!context.performed) { return; }
        OpenEcsModeEvent?.Invoke();
    }

    public void OnCameraZoom(InputAction.CallbackContext context)
    {
        ZoomValue = context.ReadValue<Vector2>();
    }

    public void OnPickup(InputAction.CallbackContext context)
    {
        if (!context.performed) { return; }
        PickUpEvent?.Invoke();
    }

    public void OnMouseMode(InputAction.CallbackContext context)
    {
        if(!context.performed) { return; }
        OpenMouseModeEvent?.Invoke();
    }

    public void OnStatBoard(InputAction.CallbackContext context)
    {
        if(!context.performed) { return; }
        OpenStatBoardEvent?.Invoke();
    }

    public void OnMap(InputAction.CallbackContext context)
    {
        if(!context.performed) { return; }
        OpenMapEvent?.Invoke();
    }
}
