using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Mono.Cecil;
using NUnit.Framework;

[TestFixture]
public class TaskTests
{
    Assembly assembly;
    string beforeAssemblyPath;
    string afterAssemblyPath;

    public  TaskTests()
    {
        beforeAssemblyPath = Path.GetFullPath(@"..\..\..\AssemblyToProcess\bin\Debug\AssemblyToProcess.dll");
#if (!DEBUG)

        beforeAssemblyPath = beforeAssemblyPath.Replace("Debug", "Release");
#endif

        afterAssemblyPath = beforeAssemblyPath.Replace(".dll", "2.dll");
        File.Copy(beforeAssemblyPath, afterAssemblyPath, true);

        var moduleDefinition = ModuleDefinition.ReadModule(afterAssemblyPath, new ReaderParameters
        {
        });
        var weavingTask = new ModuleWeaver
        {
            ModuleDefinition = moduleDefinition,
        };

        weavingTask.Execute();
        moduleDefinition.Write(afterAssemblyPath);

        assembly = Assembly.LoadFile(afterAssemblyPath);
    }


    [Test]
    public void WithBadMethodEnumConstraint()
    {
        var exception = Try(() =>
                                {
                                    var instance = assembly.GetInstance("ClassWithMethodEnumConstraint");
                                    instance.Method<string>();
                                });
        Assert.AreEqual("The type 'string' must be a non-nullable value type in order to use it as parameter 'T' in the generic type or method 'ClassWithMethodEnumConstraint.Method<T>()'", exception.Message);
    }

    [Test]
    public void WithMethodEnumConstraint()
    {
        var instance = assembly.GetInstance("ClassWithMethodEnumConstraint");
        instance.Method<AttributeTargets>();
    }
    [Test]
    public void NoReferencs()
    {
        Assert.IsFalse(assembly.GetReferencedAssemblies().Any(x => x.Name == "ExtraConstraints"));
    }

    [Test]
    public void WithBadMethodDelegateConstraint()
    {
        var exception = Try(() =>
                                {
                                    var instance = assembly.GetInstance("ClassWithMethodDelegateConstraint");
                                    instance.Method<string>();
                                });
        Assert.AreEqual("The type 'string' cannot be used as type parameter 'T' in the generic type or method 'ClassWithMethodDelegateConstraint.Method<T>()'. There is no implicit reference conversion from 'string' to 'System.Delegate'.", exception.Message);
    }
    [Test]
    public void WithMethodDelegateConstraint()
    {
        var instance = assembly.GetInstance("ClassWithMethodDelegateConstraint");
        instance.Method<Action>();
    }

    [Test]
    public void WithBadTypeEnumConstraint()
    {
        var exception = Try(() =>
                                {
                                    var instance = assembly.GetInstance<string>("ClassWithTypeEnumConstraint");
                                });
        Assert.AreEqual("GenericArguments[0], 'System.String', on 'ClassWithTypeEnumConstraint`1[T]' violates the constraint of type 'T'.", exception.Message);
    }

    [Test]
    public void WithTypeEnumConstraint()
    {
        assembly.GetInstance<AttributeTargets>("ClassWithTypeEnumConstraint");
    }

    [Test]
    public void WithBadTypeDelegateConstraint()
    {
        var exception = Try(() => assembly.GetInstance<string>("ClassWithTypeDelegateConstraint"));
        Assert.AreEqual("GenericArguments[0], 'System.String', on 'ClassWithTypeDelegateConstraint`1[T]' violates the constraint of type 'T'.", exception.Message);
    }
    [Test]
    public void WithTypeDelegateConstraint()
    {
        assembly.GetInstance<Action>("ClassWithTypeDelegateConstraint");
    }

    static Exception Try(Action action)
    {
        try
        {
            action();
        }
        catch (Exception exception)
        {
            return exception; 
        }
        return null;
    }


#if(DEBUG)
    [Test]
    public void PeVerify()
    {
        Verifier.Verify(beforeAssemblyPath,afterAssemblyPath);
    }
#endif

}