using System;
using System.Collections.Generic;
using System.Linq;
using ExtraConstraints.Fody;
using Mono.Cecil;

public class GenericParameterProcessor
{
	const string EnumConstraintAttribute = "EnumConstraintAttribute";
	const string DelegateConstraintAttribute = "DelegateConstraintAttribute";

    private readonly AssemblyNameReference corLib;
	private readonly IDictionary<string, Func<GenericParameter, CustomAttribute, TypeReference>> ConstraintTypes;
	private readonly IDictionary<string, GenericParameterAttributes> ParameterAttributes;

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

		ConstraintTypes = new Dictionary<string, Func<GenericParameter, CustomAttribute, TypeReference>>
		{
			{ EnumConstraintAttribute, (param, attr) => GetEnumType(attr, param, corLib) },
			{ DelegateConstraintAttribute, (param, attr) => GetDelegateType(attr, param, corLib) }
		};

		ParameterAttributes = new Dictionary<string, GenericParameterAttributes>
		{
			{ EnumConstraintAttribute, GenericParameterAttributes.NonVariant | GenericParameterAttributes.NotNullableValueTypeConstraint },
			{ DelegateConstraintAttribute, GenericParameterAttributes.NonVariant }
		};
    }

	public void ProcessType(TypeDefinition type)
	{
		Process(type, type);
	}

	public void ProcessMethod(MethodDefinition method)
	{
		Process(method, method.DeclaringType);
	}

	void Process(IGenericParameterProvider item, TypeDefinition nestedTypeRoot)
	{
		if (!item.HasGenericParameters) return;

		foreach (var parameter in item.GenericParameters)
		{
			foreach (var pair in ConstraintTypes)
			{
				var attribute = FindAndRemoveGenericParameterAttribute(parameter, pair.Key);
				if (attribute == null) continue;

				var typeReference = pair.Value(parameter, attribute);
				var paramAttributes = ParameterAttributes[pair.Key];
				AddGenericTypeConstraint(parameter, typeReference, paramAttributes);

				var genericParameters = nestedTypeRoot
					.RecurseTree(t => t.NestedTypes, false)
					.Where(t => t.HasGenericParameters)
					.SelectMany(t => t.GenericParameters)
					.Where(p => p.Name == parameter.Name);

				foreach (var genericParameter in genericParameters)
				{
					AddGenericTypeConstraint(genericParameter, typeReference, paramAttributes);
				}
			}
		}
	}

	CustomAttribute FindAndRemoveGenericParameterAttribute(GenericParameter parameter, string customAttributeName)
	{
		var attributes = parameter.CustomAttributes;
		for (var i = 0; i < attributes.Count; i++)
		{
			var attribute = attributes[i];
			if (attribute.AttributeType.Name == customAttributeName)
			{
				attributes.RemoveAt(i--);
				return attribute;
			}
		}

		return null;
	}

	void AddGenericTypeConstraint(GenericParameter parameter, TypeReference typeReference, GenericParameterAttributes genericParameterAttributes)
	{
		parameter.Attributes = genericParameterAttributes;
		parameter.Constraints.Clear();
		parameter.Constraints.Add(typeReference);
	}

	static TypeReference CreateConstraint(string @namespace, string name, GenericParameter parameter, AssemblyNameReference corlib)
    {
		return new TypeReference(@namespace, name, parameter.Module, corlib, false);
    }

	static TypeReference GetEnumType(CustomAttribute attribute, GenericParameter parameter, AssemblyNameReference corlib)
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
        return CreateConstraint("System", "Enum", parameter, corlib);
    }

    static TypeReference GetDelegateType(CustomAttribute attribute, GenericParameter parameter, AssemblyNameReference corlib)
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
        return CreateConstraint("System", "Delegate", parameter, corlib);
    }

}