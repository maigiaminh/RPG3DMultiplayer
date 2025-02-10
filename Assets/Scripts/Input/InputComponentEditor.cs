using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

#if UNITY_EDITOR 
using UnityEditor;
[CustomEditor(typeof(InputComponent))]
public class InputComponentEditor : Editor
{
    public override void OnInspectorGUI()
    {
        InputComponent inputComponent = (InputComponent)target;

        EditorGUILayout.LabelField("Input Component Settings", EditorStyles.boldLabel);

        inputComponent.bindingDisplayText = (TextMeshProUGUI)EditorGUILayout.ObjectField(
            "Binding Display Text", inputComponent.bindingDisplayText, typeof(TextMeshProUGUI), true);

        inputComponent.rebindButton = (Button)EditorGUILayout.ObjectField(
            "Rebind Button", inputComponent.rebindButton, typeof(Button), true);

        inputComponent.actionReference = (InputActionReference)EditorGUILayout.ObjectField(
            "Action Reference", inputComponent.actionReference, typeof(InputActionReference), true);

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Composite Settings", EditorStyles.boldLabel);

        inputComponent.isComposite = EditorGUILayout.Toggle("Is Composite", inputComponent.isComposite);

        if (inputComponent.isComposite)
        {
            inputComponent.compositeName = EditorGUILayout.TextField("Composite Name", inputComponent.compositeName);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(inputComponent);
        }
    }
}

#endif
