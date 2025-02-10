using UnityEngine;

public abstract class ConditionalAttribute : PropertyAttribute
{
    public string ComparedPropertyName { get; private set; }
    public object ComparedValue { get; private set; }

    protected ConditionalAttribute(string comparedPropertyName, object comparedValue)
    {
        ComparedPropertyName = comparedPropertyName;
        ComparedValue = comparedValue;
    }

}