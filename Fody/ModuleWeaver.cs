using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;

public partial class ModuleWeaver
{

    public Action<string> LogInfo { get; set; }
    public Action<string> LogWarning { get; set; }
    public ModuleDefinition ModuleDefinition { get; set; }

    public ModuleWeaver()
    {
        LogInfo = s => { };
        LogWarning = s => { };
    }

    public void Execute()
    {
        var allTypes = ModuleDefinition.GetTypes()
                                       .Where(x => x.IsClass || x.IsInterface)
                                       .ToList();
        var genericParameterProcessor = new GenericParameterProcessor(ModuleDefinition);
        foreach (var typeDefinition in allTypes)
        {
            genericParameterProcessor.Process(typeDefinition);
            foreach (var methodDefinition in typeDefinition.Methods)
            {
                genericParameterProcessor.Process(methodDefinition);
            }
        }

        RemoveAttributesTypes(allTypes);
        RemoveReference();
    }

    void RemoveAttributesTypes(List<TypeDefinition> allTypes)
    {
        foreach (var typeDefinition in allTypes
            .Where(x =>
                   x.Name == "EnumConstraintAttribute" ||
                   x.Name == "DelegateConstraintAttribute").ToList())
        {
            ModuleDefinition.Types.Remove(typeDefinition);
        }
    }
}