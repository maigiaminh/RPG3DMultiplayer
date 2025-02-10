public sealed class ShowIfAttribute : ConditionalAttribute
{
    public ShowIfAttribute(string comparedPropertyName, object comparedValue) : base(comparedPropertyName, comparedValue) { }
}
