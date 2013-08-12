using System.Linq;
using Mono.Cecil;
using System;

public class GenericParameterProcessor
{
    AssemblyNameReference msCorLib;
    TypeDefinition delegateTypeDefinition;
    TypeDefinition enumTypeDefinition;
    TypeDefinition objectTypeDefinition;

    public GenericParameterProcessor(ModuleDefinition moduleDefinition)
    {
        msCorLib = moduleDefinition.AssemblyReferences.First(a => a.Name == "mscorlib");
        delegateTypeDefinition = new TypeReference("System", "Delegate", moduleDefinition, msCorLib).Resolve();
        enumTypeDefinition = new TypeReference("System", "Enum", moduleDefinition, msCorLib).Resolve();
        objectTypeDefinition = new TypeReference("System", "Object", moduleDefinition, msCorLib).Resolve();
    }

    public void Process(IGenericParameterProvider provider)
    {
        if (!provider.HasGenericParameters)
        {
            return;
        }
        foreach (var parameter in provider.GenericParameters)
        {
            Process(parameter);
        }
    }

    void Process(GenericParameter parameter)
    {
        if (!parameter.HasCustomAttributes)
        {
            return;
        }

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
                if (!IsDelegateType(typeReference))
                    throw new WeavingException(string.Format("The type {0} is not a delegate type. Only delegate types are permitted in a DelegateConstraintAttribute", typeReference.FullName));

                parameter.Constraints.Add(typeReference);
                attributes.RemoveAt(i--);
            }
            else if (IsEnumConstraintAttribute(attribute))
            {
                hasEnumConstraint = true;
                parameter.Attributes = GenericParameterAttributes.NonVariant | GenericParameterAttributes.NotNullableValueTypeConstraint;
                parameter.Constraints.Clear();

                var typeReference = GetEnumType(attribute, parameter);
                if (!IsEnumType(typeReference))
                    throw new WeavingException(string.Format("The type {0} is not an enum type. Only enum types are permitted in an EnumConstraintAttribute", typeReference.FullName));

                parameter.Constraints.Add(typeReference);
                attributes.RemoveAt(i--);
            }
        }
        if (hasDelegateConstraint && hasEnumConstraint)
        {
            throw new WeavingException("Can not contain both [EnumConstraint] and [DelegateConstraint].");
        }
    }

    TypeReference CreateConstraint(string @namespace, string name, GenericParameter parameter)
    {
        return new TypeReference(@namespace, name, parameter.Module, msCorLib, false);
    }

    bool IsEnumConstraintAttribute(CustomAttribute attribute)
    {
        return attribute.AttributeType.Name == "EnumConstraintAttribute";
    }

    TypeReference GetEnumType(CustomAttribute attribute, GenericParameter parameter)
    {
        if (IsEnumConstraintAttribute(attribute))
            return attribute.HasConstructorArguments ? (attribute.ConstructorArguments[0].Value as TypeReference) : CreateConstraint("System", "Enum", parameter);
        else
            return null; // Should never happen
    }

    bool IsEnumType(TypeReference typeReference)
    {
        var definition = typeReference.Resolve();
        if (definition.FullName == enumTypeDefinition.FullName)
            return true;
        if (definition.FullName == objectTypeDefinition.FullName)
            return false;
        
        return IsEnumType(definition.BaseType);
    }

    bool IsDelegateConstraintAttribute(CustomAttribute attribute)
    {
        return attribute.AttributeType.Name == "DelegateConstraintAttribute";
    }

    TypeReference GetDelegateType(CustomAttribute attribute, GenericParameter parameter)
    {
        if (IsDelegateConstraintAttribute(attribute))
            return attribute.HasConstructorArguments ? (attribute.ConstructorArguments[0].Value as TypeReference) : CreateConstraint("System", "Delegate", parameter);
        else
            return null; // Should never happen
    }

    bool IsDelegateType(TypeReference typeReference)
    {
        var definition = typeReference.Resolve();
        if (definition.FullName == delegateTypeDefinition.FullName)
            return true;
        if (definition.FullName == objectTypeDefinition.FullName)
            return false;

        return IsDelegateType(definition.BaseType);
    }
}