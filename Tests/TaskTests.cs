using System;
using System.Reflection;
using NUnit.Framework;

[TestFixture]
public class TaskTests
{
    string projectPath;
    Assembly assembly;

    public  TaskTests()
    {
        projectPath = @"AssemblyToProcess\AssemblyToProcess.csproj";
#if (!DEBUG)
        projectPath = projectPath.Replace("Debug", "Release");
#endif
    }

    [TestFixtureSetUp]
    public void Setup()
    {
        var weaverHelper = new WeaverHelper(projectPath);
        assembly = weaverHelper.Assembly;
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
        Verifier.Verify(assembly.CodeBase.Remove(0, 8));
    }
#endif

}