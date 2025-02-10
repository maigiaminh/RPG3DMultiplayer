using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class InputManager : Singleton<InputManager>
{
    //public static InputManager Instance { get; private set; } 
    public InputActionAsset InputActionAsset; 
    public bool IsRebinding = false;
    [SerializeField] private InputReader inputReader;
    [SerializeField] private InputComponent[] inputComponents;
    
    [SerializeField] private Slider sensitivitySlider;      
    [SerializeField] private Toggle invertXToggle;         
    [SerializeField] private Toggle invertYToggle;         
    [SerializeField] private List<CinemachineFreeLook> freeLookCameras;

    private float defaultSensitivity = 1f;
    private bool defaultInvertX = false;
    private bool defaultInvertY = false;

    private const string RebindsKey = "rebinds";
    
    protected override void Awake()
    {
        base.Awake();
        LoadBindings();
        LoadMouseSettings();
    }

    public InputAction GetAction(string actionName)
    {
        var action = InputActionAsset.FindAction(actionName);
        if (action == null)
        {
            Debug.LogError($"Action '{actionName}' không tồn tại!");
        }
        return action;
    }

    public void SaveBindings()
    {
        string rebinds = InputActionAsset.SaveBindingOverridesAsJson();

        PlayerPrefs.SetString(RebindsKey, rebinds);
        PlayerPrefs.Save();
        Debug.Log("Rebinds" + rebinds);

        IsRebinding = false;

        inputReader.SaveBindings();

        ApplyMouseSettings();
    }

    public void LoadBindings()
    {
        if (PlayerPrefs.HasKey(RebindsKey))
        {
            string rebinds = PlayerPrefs.GetString(RebindsKey);

            if (!string.IsNullOrEmpty(rebinds)){
                InputActionAsset.LoadBindingOverridesFromJson(rebinds);
            }
        }
        else
        {
            ResetToDefaults();
        }

        foreach (var inputComponent in inputComponents)
        {
            inputComponent.UpdateButtonText();
        }
    }

    public void ResetToDefaults()
    {
        InputActionAsset.RemoveAllBindingOverrides();
        inputReader.ResetBindind();
        Debug.Log("Binding đã reset về mặc định!");

        IsRebinding = false;
        foreach (var inputComponent in inputComponents)
        {
            inputComponent.UpdateButtonText();
        }

        ResetMouseSettings();
    }

    
    public void LoadMouseSettings(){
        sensitivitySlider.value = PlayerPrefs.GetFloat("MouseSensitivity", defaultSensitivity);
        invertXToggle.isOn = PlayerPrefs.GetInt("InvertX", defaultInvertX ? 1 : 0) == 1;
        invertYToggle.isOn = PlayerPrefs.GetInt("InvertY", defaultInvertY ? 1 : 0) == 1;

        ApplyMouseSettings();

        sensitivitySlider.onValueChanged.AddListener(SetSensitivity);
        invertXToggle.onValueChanged.AddListener(SetInvertX);
        invertYToggle.onValueChanged.AddListener(SetInvertY);
    }

    public void SetSensitivity(float sensitivity)
    {
        foreach (var camera in freeLookCameras)
        {
            if (camera != null)
            {
                camera.m_XAxis.m_MaxSpeed = sensitivity * 300; 
                camera.m_YAxis.m_MaxSpeed = sensitivity * 2;
            }
        }
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
    }

    public void SetInvertX(bool isInverted)
    {
        foreach (var camera in freeLookCameras)
        {
            if (camera != null)
            {
                camera.m_XAxis.m_InvertInput = isInverted;
            }
        }
        PlayerPrefs.SetInt("InvertX", isInverted ? 1 : 0);
    }

    public void SetInvertY(bool isInverted)
    {
        foreach (var camera in freeLookCameras)
        {
            if (camera != null)
            {
                camera.m_YAxis.m_InvertInput = !isInverted;
            }
        }
        PlayerPrefs.SetInt("InvertY", isInverted ? 1 : 0);
    }

    public void ResetMouseSettings()
    {
        sensitivitySlider.value = defaultSensitivity;
        invertXToggle.isOn = defaultInvertX;
        invertYToggle.isOn = defaultInvertY;

        ApplyMouseSettings();
    }

    private void ApplyMouseSettings()
    {
        SetSensitivity(sensitivitySlider.value);
        SetInvertX(invertXToggle.isOn);
        SetInvertY(invertYToggle.isOn);
    }

    public string GetKeyName(string actionName)
    {
        if (InputActionAsset == null)
        {
            Debug.LogError("InputActionAsset is not assigned!");
            return "None";
        }
        
        var action = InputActionAsset.FindAction(actionName, true);

        if (action == null)
        {
            Debug.LogError($"Action '{actionName}' not found!");
            return "None";
        }
        
        foreach (var binding in action.bindings)
        {
            if (!binding.isComposite && binding.path.Contains("Keyboard"))
            {
                return binding.ToDisplayString();
            }
        }

        return "None";
    }
}
