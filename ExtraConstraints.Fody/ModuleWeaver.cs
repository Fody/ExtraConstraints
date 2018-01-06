using System.Collections.Generic;
using System.Linq;
using Fody;
using Mono.Cecil;

public partial class ModuleWeaver: BaseModuleWeaver
{
    public override void Execute()
    {
        var allTypes = ModuleDefinition.GetTypes()
                                       .Where(x => x.IsClass || x.IsInterface)
                                       .ToList();
        var genericParameterProcessor = new GenericParameterProcessor(this);
        foreach (var typeDefinition in allTypes)
        {
            genericParameterProcessor.Process(typeDefinition);
            foreach (var methodDefinition in typeDefinition.Methods)
            {
                genericParameterProcessor.Process(methodDefinition);
            }
        }

        RemoveAttributesTypes(allTypes);
    }

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "mscorlib";
        yield return "System.Runtime";
        yield return "netstandard";
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

    public override bool ShouldCleanReference => true;
}