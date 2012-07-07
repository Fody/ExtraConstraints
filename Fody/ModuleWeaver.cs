using System;
using System.Linq;
using Mono.Cecil;

public class ModuleWeaver
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
        var allTypesFinder = new AllTypesFinder
                                 {
                                     ModuleDefinition = ModuleDefinition,
                                 };
        allTypesFinder.Execute();
        var genericParameterProcessor = new GenericParameterProcessor(ModuleDefinition);
        foreach (var typeDefinition in allTypesFinder.AllTypes)
        {
            genericParameterProcessor.Process(typeDefinition);
            foreach (var methodDefinition in typeDefinition.Methods)
            {
                genericParameterProcessor.Process(methodDefinition);
            }
        }


        RemoveAttributesTypes();
    }

    void RemoveAttributesTypes()
    {
        foreach (var typeDefinition in ModuleDefinition.Types
            .Where(x =>
                   x.Name == "EnumConstraintAttribute" ||
                   x.Name == "DelegateConstraintAttribute").ToList())
        {
            ModuleDefinition.Types.Remove(typeDefinition);
        }
    }
}