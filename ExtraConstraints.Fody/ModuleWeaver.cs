using System.Collections.Generic;
using System.Linq;
using Fody;
using Mono.Cecil;

public class ModuleWeaver: BaseModuleWeaver
{
    public override void Execute()
    {
        var allTypes = ModuleDefinition
            .GetTypes()
                                       .Where(_ => _.IsClass ||
                                                   _.IsInterface)
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

    public override IEnumerable<string> GetAssembliesForScanning() =>
        Enumerable.Empty<string>();

    void RemoveAttributesTypes(List<TypeDefinition> allTypes)
    {
        foreach (var typeDefinition in allTypes
            .Where(_ =>
                   _.Name is
                       "EnumConstraintAttribute" or
                       "DelegateConstraintAttribute").ToList())
        {
            ModuleDefinition.Types.Remove(typeDefinition);
        }
    }

    public override bool ShouldCleanReference => true;
}