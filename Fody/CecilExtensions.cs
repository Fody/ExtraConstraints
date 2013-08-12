using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public static class CecilExtensions
{

    public static bool ContainsAttribute(this IEnumerable<CustomAttribute> attributes, string attributeName)
    {
        return attributes.Any(attribute => attribute.Constructor.DeclaringType.Name == attributeName);
    }

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