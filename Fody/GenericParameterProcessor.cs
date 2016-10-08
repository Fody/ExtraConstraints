using System.Linq;
using Mono.Cecil;

public class GenericParameterProcessor
{
    AssemblyNameReference corLib;

    public GenericParameterProcessor(ModuleDefinition moduleDefinition)
    {
        corLib = moduleDefinition.AssemblyReferences
                                   .FirstOrDefault(a => a.Name == "mscorlib");

        if (corLib == null || moduleDefinition.AssemblyResolver.Resolve(corLib).MainModule.Types.All(x => x.Name != "Enum"))
        {
            corLib = moduleDefinition.AssemblyReferences
                               .FirstOrDefault(a => a.Name == "System.Runtime");
            if (corLib == null)
            {
                throw new WeavingException("Could not find constraint types in `mscorlib` or `System.Runtime`.");
            }
        }
    }


    public void Process(IGenericParameterProvider provider)
    {
        if (!provider.HasGenericParameters)
        {
            return;
        }
        foreach (var parameter in provider.GenericParameters
                                          .Where(x => x.HasCustomAttributes))
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

                var typeReference = GetDelegateType(attribute, parameter);

                parameter.Constraints.Add(typeReference);
                attributes.RemoveAt(i--);
            }
            else if (IsEnumConstraintAttribute(attribute))
            {
                hasEnumConstraint = true;
                parameter.Attributes = GenericParameterAttributes.NonVariant | GenericParameterAttributes.NotNullableValueTypeConstraint;
                parameter.Constraints.Clear();

                var typeReference = GetEnumType(attribute, parameter);

                parameter.Constraints.Add(typeReference);
                attributes.RemoveAt(i--);
            }
        }
        if (hasDelegateConstraint && hasEnumConstraint)
        {
            throw new WeavingException("Cannot contain both [EnumConstraint] and [DelegateConstraint].");
        }
    }

    TypeReference CreateConstraint(string @namespace, string name, GenericParameter parameter)
    {
        return new TypeReference(@namespace, name, parameter.Module, corLib, false);
    }

    bool IsEnumConstraintAttribute(CustomAttribute attribute)
    {
        return attribute.AttributeType.Name == "EnumConstraintAttribute";
    }

    bool IsDelegateConstraintAttribute(CustomAttribute attribute)
    {
        return attribute.AttributeType.Name == "DelegateConstraintAttribute";
    }

    TypeReference GetEnumType(CustomAttribute attribute, GenericParameter parameter)
    {
        if (attribute.HasConstructorArguments)
        {
            var typeReference = (TypeReference) attribute.ConstructorArguments[0].Value;
            if (!typeReference.IsEnumType())
            {
                var message = $"The type '{typeReference.FullName}' is not an enum type. Only enum types are permitted in an EnumConstraintAttribute.";
                throw new WeavingException(message);
            }
            return typeReference;
        }
        return CreateConstraint("System", "Enum", parameter);
    }

    TypeReference GetDelegateType(CustomAttribute attribute, GenericParameter parameter)
    {
        if (attribute.HasConstructorArguments)
        {
            var typeReference = (TypeReference) attribute.ConstructorArguments[0].Value;
            if (!typeReference.IsDelegateType())
            {
                var message = $"The type '{typeReference.FullName}' is not a delegate type. Only delegate types are permitted in a DelegateConstraintAttribute.";
                throw new WeavingException(message);
            }
            return typeReference;
        }
        return CreateConstraint("System", "Delegate", parameter);
    }

}