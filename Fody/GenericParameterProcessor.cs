using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public class GenericParameterProcessor
{
    IAssemblyResolver assemblyResolver;
    TypeDefinition enumType;
    TypeDefinition delegateType;

    public GenericParameterProcessor(IAssemblyResolver assemblyResolver)
    {
        this.assemblyResolver = assemblyResolver;
        var coreTypes = new List<TypeDefinition>();
        AppendTypes("mscorlib", coreTypes);
        AppendTypes("System.Runtime", coreTypes);
        enumType = coreTypes.First(x => x.Name == "Enum");
        delegateType = coreTypes.First(x => x.Name == "Delegate");
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

    void AppendTypes(string name, List<TypeDefinition> coreTypes)
    {
        var definition = assemblyResolver.Resolve(name);
        if (definition != null)
        {
            coreTypes.AddRange(definition.MainModule.Types);
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

    TypeReference CreateConstraint(TypeDefinition typeDefinition, GenericParameter parameter)
    {
        return new TypeReference(typeDefinition.Namespace, typeDefinition.Name, parameter.Module, typeDefinition.Module, false);
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
                var message = string.Format("The type '{0}' is not an enum type. Only enum types are permitted in an EnumConstraintAttribute.", typeReference.FullName);
                throw new WeavingException(message);
            }
            return typeReference;
        }
        return CreateConstraint(enumType, parameter);
    }

    TypeReference GetDelegateType(CustomAttribute attribute, GenericParameter parameter)
    {
        if (attribute.HasConstructorArguments)
        {
            var typeReference = (TypeReference) attribute.ConstructorArguments[0].Value;
            if (!typeReference.IsDelegateType())
            {
                var message = string.Format("The type '{0}' is not a delegate type. Only delegate types are permitted in a DelegateConstraintAttribute.", typeReference.FullName);
                throw new WeavingException(message);
            }
            return typeReference;
        }
        return CreateConstraint(delegateType, parameter);
    }

}