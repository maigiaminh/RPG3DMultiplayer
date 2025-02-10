using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[System.Serializable]
public class InputComponent : MonoBehaviour
{
    [Header("Input Settings")]
    public TextMeshProUGUI bindingDisplayText;
    public Button rebindButton;  
    public InputActionReference actionReference; 

    [Header("Composite Settings")]
    public bool isComposite;
    public string compositeName;

    private int bindingIndex = 0;
    private InputBinding? originalBinding = null;


    private void Start()
    {
        rebindButton.onClick.AddListener(() => ListenToRebindKey());

        UpdateButtonText();
    }

    public void UpdateButtonText()
    {
        if (actionReference != null && actionReference.action != null)
        {
            var action = actionReference.action;

            if (action.bindings[bindingIndex].isComposite)
            {
                if(isComposite){
                    int firstPartIndex = bindingIndex + 1;

                    int partIndex = compositeName switch
                    {
                        "Up" => firstPartIndex,
                        "Down" => firstPartIndex + 1,
                        "Left" => firstPartIndex + 2,
                        "Right" => firstPartIndex + 3,
                        _ => throw new System.Exception("Invalid Composite Part")
                    };                 

                    bindingDisplayText.text = action.bindings[partIndex].ToDisplayString();
                }
            }
            else
            {
                var bindingDisplayString = actionReference.action.bindings[bindingIndex].ToDisplayString();
                bindingDisplayText.text = bindingDisplayString;
            }
        }
    }

    private void ListenToRebindKey()
    {
        if (InputManager.Instance.IsRebinding)
        {
            Debug.Log("Already Rebinding!");
            return;
        }
        else{
            InputManager.Instance.IsRebinding = true;
        }
        InputAction matchedAction = FindMatchingInputActionRef(actionReference);
        matchedAction.Disable();
        if (matchedAction.bindings[bindingIndex].isComposite)
        {
            int firstPartIndex = bindingIndex + 1;

            int partIndex = compositeName switch
            {
                "Up" => firstPartIndex,
                "Down" => firstPartIndex + 1,
                "Left" => firstPartIndex + 2,
                "Right" => firstPartIndex + 3,
                _ => throw new System.Exception("Invalid Composite Part")
            };

            originalBinding = matchedAction.bindings[partIndex];
            Debug.Log("Original Binding: " + originalBinding.Value.path);
            var rebindOperation = matchedAction.PerformInteractiveRebinding(partIndex)
                .WithControlsExcluding("Mouse")
                .WithCancelingThrough("<Keyboard>/escape")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation => RebindComplete(operation, actionReference, partIndex))
                .OnCancel(operation => RebindCancel(operation))
                .Start();
        }
        else{
            originalBinding = matchedAction.bindings[bindingIndex];
            var rebindOperation = matchedAction.PerformInteractiveRebinding()
                .WithCancelingThrough("<Keyboard>/escape")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation => RebindComplete(operation, actionReference))
                .OnCancel(operation => RebindCancel(operation))
                .Start(); 
        }

        bindingDisplayText.text = "Rebinding..";
    }

    private void RebindComplete(InputActionRebindingExtensions.RebindingOperation operation, InputActionReference actionRef, int partIndex = 0)
    {
        string newBindingPath = operation.selectedControl.path;
        Debug.Log("New Binding Path: " + newBindingPath);
        InputManager.Instance.IsRebinding = false;
        operation.Dispose();

        if (IsDuplicateBinding(newBindingPath, actionRef.action))
        {
            Debug.Log("Duplicate Key!");
            actionRef.action.ChangeBinding(partIndex).To(originalBinding.Value);

            UpdateButtonText();
            return;
        }
        
        SaveBindingOverride(actionRef, partIndex);
        if (actionRef.action.actionMap.enabled){
            actionRef.action.actionMap.Enable();
            actionRef.action.Enable();
        }
    }

    private void RebindCancel(InputActionRebindingExtensions.RebindingOperation operation){
        operation.Dispose();
        InputManager.Instance.IsRebinding = false;
        UpdateButtonText();
    }
    public void SaveBindingOverride(InputActionReference actionRef, int partIndex)
    {
        if(!isComposite){
            foreach (var binding in actionRef.action.bindings)
            if (!binding.isPartOfComposite)
            {
                string bindingName = InputControlPath.ToHumanReadableString(
                    binding.effectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice);
                bindingDisplayText.text = bindingName;
            }
        }
        else{
            var action = actionReference.action;
            string bindingName = InputControlPath.ToHumanReadableString(
                    action.bindings[partIndex].effectivePath,
                    InputControlPath.HumanReadableStringOptions.OmitDevice);
            bindingDisplayText.text = bindingName;
        }
    }

    private InputAction FindMatchingInputActionRef(InputActionReference inputActionRef)
    {
        foreach (var map in InputManager.Instance.InputActionAsset.actionMaps)
            foreach (var action in map.actions)
                if (action.id == inputActionRef.action.id) return action;
        Debug.LogError("Matching action not found in inputActions");
        return null;
    }

    private bool IsDuplicateBinding(string newBindingPath, InputAction currentAction)
    {
        var allActions = InputManager.Instance.InputActionAsset;

        string normalizedNewBindingPath = InputControlPath.ToHumanReadableString(
            NormalizeControlPath(newBindingPath),
            InputControlPath.HumanReadableStringOptions.OmitDevice
        );



        foreach (var map in allActions.actionMaps)
        {
            foreach (var action in map.actions)
            {
                if (action == currentAction && !isComposite) 
                    continue;
                
                foreach (var binding in action.bindings)
                {
                    if(binding.name.ToLower() == compositeName.ToLower() && isComposite && compositeName != "")
                        continue;

                    string normalizedBindingPath = InputControlPath.ToHumanReadableString(
                        binding.effectivePath,
                        InputControlPath.HumanReadableStringOptions.OmitDevice
                    );

                    if (normalizedBindingPath.ToLower() == normalizedNewBindingPath.ToLower())
                    {
                        return true; 
                    }
                }
            }
        }

        return false; 
    }

    private string NormalizeControlPath(string path)
    {
        path = path.Replace("/Keyboard/leftCtrl", "/Keyboard/Left Control")
                .Replace("/Keyboard/rightCtrl", "/Keyboard/Right Control")
                .Replace("/Keyboard/leftShift", "/Keyboard/Left Shift")
                .Replace("/Keyboard/rightShift", "/Keyboard/Right Shift")
                .Replace("/Keyboard/capsLock", "/Keyboard/Caps Lock")
                .Replace("/Keyboard/leftAlt", "/Keyboard/Scroll")
                .Replace("/Keyboard/rightAlt", "/Keyboard/Scroll")
                .Replace("/Keyboard/leftMeta", "/Keyboard/Left System")
                .Replace("/Keyboard/rightMeta", "/Keyboard/Right System")
                .Replace("/Keyboard/backquote", "/Keyboard/`");
        return path;
    }
}
