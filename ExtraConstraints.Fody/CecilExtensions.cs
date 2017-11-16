using Mono.Cecil;

public static class CecilExtensions
{
    public static bool IsDelegateType(this TypeReference typeReference)
    {
        var definition = typeReference.Resolve();
        return definition.BaseType.FullName == "System.MulticastDelegate";
    }

    public static bool IsEnumType(this TypeReference typeReference)
    {
        var definition = typeReference.Resolve();
        return definition.BaseType.FullName == "System.Enum";
    }
}