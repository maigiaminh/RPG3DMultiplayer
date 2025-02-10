using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "NewMap", menuName = "ScriptableObjects/MapData", order = 1)]
public class MapData : ScriptableObject
{
    public DungeonData dungeonData;
    public string mapName;
    [TextArea]
    public string description;
    public Sprite reviewImage;
    public Sprite mapImage;
    public bool active;

#if UNITY_EDITOR
    [ConditionalField("active", true)]
#endif
    public string sceneName;
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ConditionalFieldAttribute))]
public class ConditionalFieldDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var conditionalAttribute = attribute as ConditionalFieldAttribute;
        if (conditionalAttribute == null) return;

        SerializedProperty conditionProperty = property.serializedObject.FindProperty(conditionalAttribute.ConditionalField);
        if (conditionProperty != null && conditionProperty.boolValue == conditionalAttribute.ShowIfTrue)
        {
            EditorGUI.PropertyField(position, property, label, true);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var conditionalAttribute = attribute as ConditionalFieldAttribute;
        if (conditionalAttribute == null) return EditorGUI.GetPropertyHeight(property);

        SerializedProperty conditionProperty = property.serializedObject.FindProperty(conditionalAttribute.ConditionalField);
        if (conditionProperty != null && conditionProperty.boolValue == conditionalAttribute.ShowIfTrue)
        {
            return EditorGUI.GetPropertyHeight(property);
        }
        return 0f;
    }
}

public class ConditionalFieldAttribute : PropertyAttribute
{
    public string ConditionalField;
    public bool ShowIfTrue;

    public ConditionalFieldAttribute(string conditionalField, bool showIfTrue)
    {
        ConditionalField = conditionalField;
        ShowIfTrue = showIfTrue;
    }
}
#endif
