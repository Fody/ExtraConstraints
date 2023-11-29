using System.Linq;
using Fody;
using Mono.Cecil;

public class GenericParameterProcessor
{
    TypeReference delegateType;
    TypeReference enumType;

    public GenericParameterProcessor(ModuleWeaver moduleWeaver)
    {
        var module = moduleWeaver.ModuleDefinition;
        delegateType = module.ImportReference(moduleWeaver.FindTypeDefinition("System.Delegate"));
        enumType = module.ImportReference(moduleWeaver.FindTypeDefinition("System.Enum"));
    }

    public void Process(IGenericParameterProvider provider)
    {
        if (!provider.HasGenericParameters)
        {
            return;
        }

        foreach (var parameter in provider.GenericParameters
                     .Where(_ => _.HasCustomAttributes))
        {
            Process(parameter);
        }
    }

    void Process(GenericParameter parameter)
    {
        var hasDelegateConstraint = false;
        var hasEnumConstraint = false;
        var attributes = parameter.CustomAttributes;
        for (var i = 0; i < attributes.Count; i++)
        {
            var attribute = attributes[i];
            if (IsDelegateConstraintAttribute(attribute))
            {
                hasDelegateConstraint = true;
                parameter.Attributes = GenericParameterAttributes.NonVariant;
                parameter.Constraints.Clear();

                var typeReference = GetDelegateType(attribute);

                parameter.Constraints.Add(new GenericParameterConstraint(typeReference));
                attributes.RemoveAt(i--);
            }
            else if (IsEnumConstraintAttribute(attribute))
            {
                hasEnumConstraint = true;
                parameter.Attributes = GenericParameterAttributes.NonVariant | GenericParameterAttributes.NotNullableValueTypeConstraint;
                parameter.Constraints.Clear();

                var typeReference = GetEnumType(attribute);

                parameter.Constraints.Add(new GenericParameterConstraint(typeReference));
                attributes.RemoveAt(i--);
            }
        }

        if (hasDelegateConstraint && hasEnumConstraint)
        {
            throw new WeavingException("Cannot contain both [EnumConstraint] and [DelegateConstraint].");
        }
    }

    static bool IsEnumConstraintAttribute(CustomAttribute attribute) =>
        attribute.AttributeType.Name == "EnumConstraintAttribute";

    static bool IsDelegateConstraintAttribute(CustomAttribute attribute) =>
        attribute.AttributeType.Name == "DelegateConstraintAttribute";

    TypeReference GetEnumType(CustomAttribute attribute)
    {
        if (!attribute.HasConstructorArguments)
        {
            return enumType;
        }

        var typeReference = (TypeReference) attribute.ConstructorArguments[0].Value;
        if (typeReference.IsEnumType())
        {
            return typeReference;
        }

        var message = $"The type '{typeReference.FullName}' is not an enum type. Only enum types are permitted in an EnumConstraintAttribute.";
        throw new WeavingException(message);
    }

    TypeReference GetDelegateType(CustomAttribute attribute)
    {
        if (!attribute.HasConstructorArguments)
        {
            return delegateType;
        }

        var typeReference = (TypeReference) attribute.ConstructorArguments[0].Value;
        if (typeReference.IsDelegateType())
        {
            return typeReference;
        }

        var message = $"The type '{typeReference.FullName}' is not a delegate type. Only delegate types are permitted in a DelegateConstraintAttribute.";
        throw new WeavingException(message);
    }
}