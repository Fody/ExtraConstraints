using System.Linq;

public partial class ModuleWeaver
{
    public void RemoveReference()
    {
        var referenceToRemove = ModuleDefinition.AssemblyReferences.FirstOrDefault(x => x.Name == "ExtraConstraints");
        if (referenceToRemove == null)
        {
            LogInfo("\tNo reference to 'ExtraConstraints' found. References not modified.");
            return;
        }

        ModuleDefinition.AssemblyReferences.Remove(referenceToRemove);
        LogInfo("\tRemoving reference to 'ExtraConstraints'.");
    }
}