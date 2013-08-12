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

    public TaskTests()
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
    public void MethodEnumAttributeShouldThrowWhenPassedANonEnum()
    {
        var exception = Try(() =>
                                {
                                    var instance = assembly.GetInstance("ClassWithMethodEnumConstraint");
                                    instance.Method<string>();
                                });
        Assert.AreEqual("The type 'string' must be a non-nullable value type in order to use it as parameter 'T' in the generic type or method 'ClassWithMethodEnumConstraint.Method<T>()'", exception.Message);
    }
    
    [Test]
    public void MethodEnumAttributeShouldThrowWhenPassedAnInCompatibleEnum()
    {
        var exception = Try(() =>
        {
            var instance = assembly.GetInstance("ClassWithMethodEnumConstraint2");
            instance.Method<ConsoleKey>();
        });
        Assert.AreEqual("The type 'System.ConsoleKey' cannot be used as type parameter 'T' in the generic type or method 'ClassWithMethodEnumConstraint2.Method<T>()'. There is no boxing conversion from 'System.ConsoleKey' to 'System.ConsoleColor'.", exception.Message);
    }

    [Test]
    public void MethodWithEnumAttributeShouldBeCallable()
    {
        var instance = assembly.GetInstance("ClassWithMethodEnumConstraint");
        instance.Method<AttributeTargets>();
    }

	[Test]
	public void MethodWithEnumAttributeShouldHaveDelegateConstraint()
	{
		var genericParameterConstraints = assembly.GetType("ClassWithMethodEnumConstraint")
			.GetMethods()
			.First(x => x.Name == "Method")
			.GetGenericArguments();
		Assert.AreEqual(genericParameterConstraints.First().BaseType, typeof(Enum));
	}

    [Test]
    public void MethodWithEnumAttributeShouldHaveDelegateConstraint2()
    {
        var genericParameterConstraints = assembly.GetType("ClassWithMethodEnumConstraint2")
            .GetMethods()
            .First(x => x.Name == "Method")
            .GetGenericArguments();
        Assert.AreEqual(genericParameterConstraints.First().BaseType, typeof(ConsoleColor));
    }

    [Test]
	public void InterfaceMethodWithEnumAttributeShouldHaveDelegateConstraint()
	{
		var genericParameterConstraints = assembly.GetType("InterfaceWithMethodEnumConstraint")
			.GetMethods()
			.First(x => x.Name == "Method")
			.GetGenericArguments();
		Assert.AreEqual(genericParameterConstraints.First().BaseType, typeof(Enum));
	}

    [Test]
    public void InterfaceMethodWithEnumAttributeShouldHaveDelegateConstraint2()
    {
        var genericParameterConstraints = assembly.GetType("InterfaceWithMethodEnumConstraint2")
            .GetMethods()
            .First(x => x.Name == "Method")
            .GetGenericArguments();
        Assert.AreEqual(genericParameterConstraints.First().BaseType, typeof(ConsoleColor));
    }

    [Test]
	public void ReferenceToExtraConstraintsShouldBeRemoved()
    {
        Assert.IsFalse(assembly.GetReferencedAssemblies().Any(x => x.Name == "ExtraConstraints"));
    }

    [Test]
    public void MethodDelegateAttributeShouldThrowWhenPassedANonDelegate()
    {
        var exception = Try(() =>
                                {
                                    var instance = assembly.GetInstance("ClassWithMethodDelegateConstraint");
                                    instance.Method<string>();
                                });
        Assert.AreEqual("The type 'string' cannot be used as type parameter 'T' in the generic type or method 'ClassWithMethodDelegateConstraint.Method<T>()'. There is no implicit reference conversion from 'string' to 'System.Delegate'.", exception.Message);
    }

    [Test]
    public void MethodDelegateAttributeShouldThrowWhenPassedAnIncompatibleDelegate()
    {
        var exception = Try(() =>
        {
            var instance = assembly.GetInstance("ClassWithMethodDelegateConstraint2");
            instance.Method<Func<string>>();
        });
        Assert.AreEqual("The type 'System.Func<string>' cannot be used as type parameter 'T' in the generic type or method 'ClassWithMethodDelegateConstraint2.Method<T>()'. There is no implicit reference conversion from 'System.Func<string>' to 'System.Func<int>'.", exception.Message);
    }

    [Test]
    public void MethodWithDelegateAttributeShouldBeCallable()
    {
        var instance = assembly.GetInstance("ClassWithMethodDelegateConstraint");
        instance.Method<Action>();
    }

	[Test]
	public void MethodWithDelegateAttributeShouldHaveDelegateConstraint()
	{
		var genericParameterConstraints = assembly.GetType("ClassWithMethodDelegateConstraint")
			.GetMethods()
			.First(x => x.Name == "Method")
			.GetGenericArguments();
		Assert.AreEqual(genericParameterConstraints.First().BaseType, typeof(Delegate));
	}

    [Test]
    public void MethodWithDelegateAttributeShouldHaveDelegateConstraint2()
    {
        var genericParameterConstraints = assembly.GetType("ClassWithMethodDelegateConstraint2")
            .GetMethods()
            .First(x => x.Name == "Method")
            .GetGenericArguments();
        Assert.AreEqual(genericParameterConstraints.First().BaseType, typeof(Func<int>));
    }

    [Test]
	public void InterfaceMethodWithDelegateAttributeShouldHaveDelegateConstraint()
	{
		var genericParameterConstraints = assembly.GetType("InterfaceWithMethodDelegateConstraint")
			.GetMethods()
			.First(x => x.Name == "Method")
			.GetGenericArguments();
		Assert.AreEqual(genericParameterConstraints.First().BaseType, typeof(Delegate));
	}

    [Test]
    public void InterfaceMethodWithDelegateAttributeShouldHaveDelegateConstraint2()
    {
        var genericParameterConstraints = assembly.GetType("InterfaceWithMethodDelegateConstraint2")
            .GetMethods()
            .First(x => x.Name == "Method")
            .GetGenericArguments();
        Assert.AreEqual(genericParameterConstraints.First().BaseType, typeof(Func<int>));
    }

    [Test]
    public void ClassWithEnumAttributeShouldThrowWhenPassedANonEnum()
    {
        var exception = Try(() => {assembly.GetInstance<string>("ClassWithTypeEnumConstraint");});
        Assert.AreEqual("GenericArguments[0], 'System.String', on 'ClassWithTypeEnumConstraint`1[T]' violates the constraint of type 'T'.", exception.Message);
    }

    [Test]
    public void ClassWithEnumAttributeShouldThrowWhenPassedAnIncompatibleEnum()
    {
        var exception = Try(() => { assembly.GetInstance<ConsoleKey>("ClassWithTypeEnumConstraint2"); });
        Assert.AreEqual("GenericArguments[0], 'System.ConsoleKey', on 'ClassWithTypeEnumConstraint2`1[T]' violates the constraint of type 'T'.", exception.Message);
    }

    [Test]
    public void ClassWithEnumAttributeShouldBeCallable()
    {
        assembly.GetInstance<AttributeTargets>("ClassWithTypeEnumConstraint");
    }

    [Test]
    public void ClassWithEnumAttributeShouldBeCallable2()
    {
        assembly.GetInstance<ConsoleColor>("ClassWithTypeEnumConstraint");
    }

    [Test]
    public void ClassWithEnumAttributeShouldHaveEnumConstraint()
    {
	    var genericParameterConstraints = assembly.GetType("ClassWithTypeEnumConstraint`1").GetGenericArguments();
	    Assert.AreEqual(genericParameterConstraints.First().BaseType, typeof (Enum));
    }

    [Test]
    public void ClassWithEnumAttributeShouldHaveEnumConstraint2()
    {
        var genericParameterConstraints = assembly.GetType("ClassWithTypeEnumConstraint2`1").GetGenericArguments();
        Assert.AreEqual(genericParameterConstraints.First().BaseType, typeof(ConsoleColor));
    }

    [Test]
    public void InterfaceWithEnumAttributeShouldHaveEnumConstraint()
    {
	    var genericParameterConstraints = assembly.GetType("InterfaceWithTypeEnumConstraint`1").GetGenericArguments();
	    Assert.AreEqual(genericParameterConstraints.First().BaseType, typeof (Enum));
    }

    [Test]
    public void InterfaceWithEnumAttributeShouldHaveEnumConstraint2()
    {
        var genericParameterConstraints = assembly.GetType("InterfaceWithTypeEnumConstraint2`1").GetGenericArguments();
        Assert.AreEqual(genericParameterConstraints.First().BaseType, typeof(ConsoleColor));
    }

    [Test]
    public void ClassWithDelegateAttributeShouldThrowWhenPassedNonDelegate()
    {
        var exception = Try(() => assembly.GetInstance<string>("ClassWithTypeDelegateConstraint"));
        Assert.AreEqual("GenericArguments[0], 'System.String', on 'ClassWithTypeDelegateConstraint`1[T]' violates the constraint of type 'T'.", exception.Message);
    }

    [Test]
    public void ClassWithDelegateAttributeShouldThrowWhenPassedIncompatibleDelegate()
    {
        var exception = Try(() => assembly.GetInstance<Func<string>>("ClassWithTypeDelegateConstraint2"));
        Assert.AreEqual("GenericArguments[0], 'System.Func`1[System.String]', on 'ClassWithTypeDelegateConstraint2`1[T]' violates the constraint of type 'T'.", exception.Message);
    }

    [Test]
    public void ClassWithDelegateAttributeShouldBeCallable()
    {
        assembly.GetInstance<Action>("ClassWithTypeDelegateConstraint");
    }

    [Test]
    public void ClassWithDelegateAttributeShouldBeCallable2()
    {
        assembly.GetInstance<Func<int>>("ClassWithTypeDelegateConstraint");
    }

    [Test]
	public void ClassWithDelegateAttributeShouldHaveDelegateConstraint()
	{
		var genericParameterConstraints = assembly.GetType("ClassWithTypeDelegateConstraint`1").GetGenericArguments();
		Assert.AreEqual(genericParameterConstraints.First().BaseType, typeof(Delegate));
	}

    [Test]
    public void ClassWithDelegateAttributeShouldHaveDelegateConstraint2()
    {
        var genericParameterConstraints = assembly.GetType("ClassWithTypeDelegateConstraint2`1").GetGenericArguments();
        Assert.AreEqual(genericParameterConstraints.First().BaseType, typeof(Func<int>));
    }

    [Test]
	public void InterfaceWithDelegateAttributeShouldHaveDelegateConstraint()
	{
		var genericParameterConstraints = assembly.GetType("InterfaceWithTypeDelegateConstraint`1").GetGenericArguments();
		Assert.AreEqual(genericParameterConstraints.First().BaseType, typeof(Delegate));
	}

    [Test]
    public void InterfaceWithDelegateAttributeShouldHaveDelegateConstraint2()
    {
        var genericParameterConstraints = assembly.GetType("InterfaceWithTypeDelegateConstraint2`1").GetGenericArguments();
        Assert.AreEqual(genericParameterConstraints.First().BaseType, typeof(Func<int>));
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