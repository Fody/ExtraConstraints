using System.Linq;
using Mono.Cecil;

public class GenericParameterProcessor
{
    AssemblyNameReference msCorLib;

    public GenericParameterProcessor(ModuleDefinition moduleDefinition)
    {
        msCorLib = moduleDefinition.AssemblyReferences.First(a => a.Name == "mscorlib");

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
                parameter.Constraints.Add(CreateConstraint("System", "Delegate", parameter));
                attributes.RemoveAt(i--);
            }
            else if (IsEnumConstraintAttribute(attribute))
            {
                hasEnumConstraint = true;
                parameter.Attributes = GenericParameterAttributes.NonVariant | GenericParameterAttributes.NotNullableValueTypeConstraint;
                parameter.Constraints.Clear();
                var typeReference = CreateConstraint("System", "Enum", parameter);
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

    bool IsDelegateConstraintAttribute(CustomAttribute attribute)
    {
        return attribute.AttributeType.Name == "DelegateConstraintAttribute";
    }
}