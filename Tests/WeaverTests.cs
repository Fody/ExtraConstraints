using System;
using System.Linq;
using System.Reflection;
using Fody;
using VerifyXunit;
using Xunit;
using Xunit.Abstractions;

public class WeaverTests :
    VerifyBase
{
    static TestResult testResult;
    static Assembly assembly;

    static WeaverTests()
    {
        var weavingTask = new ModuleWeaver();
        testResult = weavingTask.ExecuteTestRun("AssemblyToProcess.dll");
        assembly = testResult.Assembly;
    }

    [Fact]
    public void MethodEnumAttributeShouldThrowWhenPassedANonEnum()
    {
        var exception = Try(() =>
        {
            var instance = testResult.GetInstance("ClassWithMethodEnumConstraint");
            instance.Method<string>();
        });
        Assert.Equal("The type 'string' must be a non-nullable value type in order to use it as parameter 'T' in the generic type or method 'ClassWithMethodEnumConstraint.Method<T>()'", exception.Message);
    }

    [Fact]
    public void MethodEnumAttributeShouldThrowWhenPassedAnInCompatibleEnum()
    {
        var exception = Try(() =>
        {
            var instance = testResult.GetInstance("ClassWithMethodEnumConstraint2");
            instance.Method<ConsoleKey>();
        });
        Assert.Equal("The type 'System.ConsoleKey' cannot be used as type parameter 'T' in the generic type or method 'ClassWithMethodEnumConstraint2.Method<T>()'. There is no boxing conversion from 'System.ConsoleKey' to 'System.ConsoleColor'.", exception.Message);
    }

    [Fact]
    public void MethodWithEnumAttributeShouldBeCallable()
    {
        var instance = testResult.GetInstance("ClassWithMethodEnumConstraint");
        instance.Method<AttributeTargets>();
    }

    [Fact]
    public void MethodWithEnumAttributeShouldHaveDelegateConstraint()
    {
        var genericParameterConstraints = assembly.GetType("ClassWithMethodEnumConstraint")
            .GetMethods()
            .First(x => x.Name == "Method")
            .GetGenericArguments();
        Assert.Equal(typeof(Enum), genericParameterConstraints.First().BaseType);
    }

    [Fact]
    public void MethodWithEnumAttributeShouldHaveDelegateConstraint2()
    {
        var genericParameterConstraints = assembly.GetType("ClassWithMethodEnumConstraint2")
            .GetMethods()
            .First(x => x.Name == "Method")
            .GetGenericArguments();
        Assert.Equal(typeof(ConsoleColor), genericParameterConstraints.First().BaseType);
    }

    [Fact]
    public void InterfaceMethodWithEnumAttributeShouldHaveDelegateConstraint()
    {
        var genericParameterConstraints = assembly.GetType("InterfaceWithMethodEnumConstraint")
            .GetMethods()
            .First(x => x.Name == "Method")
            .GetGenericArguments();
        Assert.Equal(typeof(Enum), genericParameterConstraints.First().BaseType);
    }

    [Fact]
    public void InterfaceMethodWithEnumAttributeShouldHaveDelegateConstraint2()
    {
        var genericParameterConstraints = assembly.GetType("InterfaceWithMethodEnumConstraint2")
            .GetMethods()
            .First(x => x.Name == "Method")
            .GetGenericArguments();
        Assert.Equal(typeof(ConsoleColor), genericParameterConstraints.First().BaseType);
    }

    [Fact]
    public void ReferenceToExtraConstraintsShouldBeRemoved()
    {
        Assert.DoesNotContain(assembly.GetReferencedAssemblies(), x => x.Name == "ExtraConstraints");
    }

    [Fact]
    public void MethodDelegateAttributeShouldThrowWhenPassedANonDelegate()
    {
        var exception = Try(() =>
        {
            var instance = testResult.GetInstance("ClassWithMethodDelegateConstraint");
            instance.Method<string>();
        });
        Assert.Equal("The type 'string' cannot be used as type parameter 'T' in the generic type or method 'ClassWithMethodDelegateConstraint.Method<T>()'. There is no implicit reference conversion from 'string' to 'System.Delegate'.", exception.Message);
    }

    [Fact]
    public void MethodDelegateAttributeShouldThrowWhenPassedAnIncompatibleDelegate()
    {
        var exception = Try(() =>
        {
            var instance = testResult.GetInstance("ClassWithMethodDelegateConstraint2");
            instance.Method<Func<string>>();
        });
        Assert.Equal("The type 'System.Func<string>' cannot be used as type parameter 'T' in the generic type or method 'ClassWithMethodDelegateConstraint2.Method<T>()'. There is no implicit reference conversion from 'System.Func<string>' to 'System.Func<int>'.", exception.Message);
    }

    [Fact]
    public void MethodWithDelegateAttributeShouldBeCallable()
    {
        var instance = testResult.GetInstance("ClassWithMethodDelegateConstraint");
        instance.Method<Action>();
    }

    [Fact]
    public void MethodWithDelegateAttributeShouldHaveDelegateConstraint()
    {
        var genericParameterConstraints = assembly.GetType("ClassWithMethodDelegateConstraint")
            .GetMethods()
            .First(x => x.Name == "Method")
            .GetGenericArguments();
        Assert.Equal(typeof(Delegate), genericParameterConstraints.First().BaseType);
    }

    [Fact]
    public void MethodWithDelegateAttributeShouldHaveDelegateConstraint2()
    {
        var genericParameterConstraints = assembly.GetType("ClassWithMethodDelegateConstraint2")
            .GetMethods()
            .First(x => x.Name == "Method")
            .GetGenericArguments();
        Assert.Equal(typeof(Func<int>), genericParameterConstraints.First().BaseType);
    }

    [Fact]
    public void InterfaceMethodWithDelegateAttributeShouldHaveDelegateConstraint()
    {
        var genericParameterConstraints = assembly.GetType("InterfaceWithMethodDelegateConstraint")
            .GetMethods()
            .First(x => x.Name == "Method")
            .GetGenericArguments();
        Assert.Equal(typeof(Delegate), genericParameterConstraints.First().BaseType);
    }

    [Fact]
    public void InterfaceMethodWithDelegateAttributeShouldHaveDelegateConstraint2()
    {
        var genericParameterConstraints = assembly.GetType("InterfaceWithMethodDelegateConstraint2")
            .GetMethods()
            .First(x => x.Name == "Method")
            .GetGenericArguments();
        Assert.Equal(typeof(Func<int>), genericParameterConstraints.First().BaseType);
    }

    [Fact]
    public void ClassWithEnumAttributeShouldThrowWhenPassedANonEnum()
    {
        var exception = Try(() => assembly.GetInstance<string>("ClassWithTypeEnumConstraint"));
        Assert.Equal("GenericArguments[0], 'System.String', on 'ClassWithTypeEnumConstraint`1[T]' violates the constraint of type 'T'.", exception.Message);
    }

    [Fact]
    public void ClassWithEnumAttributeShouldThrowWhenPassedAnIncompatibleEnum()
    {
        var exception = Try(() => assembly.GetInstance<ConsoleKey>("ClassWithTypeEnumConstraint2"));
        Assert.Equal("GenericArguments[0], 'System.ConsoleKey', on 'ClassWithTypeEnumConstraint2`1[T]' violates the constraint of type 'T'.", exception.Message);
    }

    [Fact]
    public void ClassWithEnumAttributeShouldBeCallable()
    {
        assembly.GetInstance<AttributeTargets>("ClassWithTypeEnumConstraint");
    }

    [Fact]
    public void ClassWithEnumAttributeShouldBeCallable2()
    {
        assembly.GetInstance<ConsoleColor>("ClassWithTypeEnumConstraint");
    }

    [Fact]
    public void ClassWithEnumAttributeShouldHaveEnumConstraint()
    {
        var genericParameterConstraints = assembly.GetType("ClassWithTypeEnumConstraint`1").GetGenericArguments();
        Assert.Equal(typeof(Enum), genericParameterConstraints.First().BaseType);
    }

    [Fact]
    public void ClassWithEnumAttributeShouldHaveEnumConstraint2()
    {
        var genericParameterConstraints = assembly.GetType("ClassWithTypeEnumConstraint2`1").GetGenericArguments();
        Assert.Equal(typeof(ConsoleColor), genericParameterConstraints.First().BaseType);
    }

    [Fact]
    public void InterfaceWithEnumAttributeShouldHaveEnumConstraint()
    {
        var genericParameterConstraints = assembly.GetType("InterfaceWithTypeEnumConstraint`1").GetGenericArguments();
        Assert.Equal(typeof(Enum), genericParameterConstraints.First().BaseType);
    }

    [Fact]
    public void InterfaceWithEnumAttributeShouldHaveEnumConstraint2()
    {
        var genericParameterConstraints = assembly.GetType("InterfaceWithTypeEnumConstraint2`1").GetGenericArguments();
        Assert.Equal(typeof(ConsoleColor), genericParameterConstraints.First().BaseType);
    }

    [Fact]
    public void ClassWithDelegateAttributeShouldThrowWhenPassedNonDelegate()
    {
        var exception = Try(() => assembly.GetInstance<string>("ClassWithTypeDelegateConstraint"));
        Assert.Equal("GenericArguments[0], 'System.String', on 'ClassWithTypeDelegateConstraint`1[T]' violates the constraint of type 'T'.", exception.Message);
    }

    [Fact]
    public void ClassWithDelegateAttributeShouldThrowWhenPassedIncompatibleDelegate()
    {
        var exception = Try(() => assembly.GetInstance<Func<string>>("ClassWithTypeDelegateConstraint2"));
        Assert.Equal("GenericArguments[0], 'System.Func`1[System.String]', on 'ClassWithTypeDelegateConstraint2`1[T]' violates the constraint of type 'T'.", exception.Message);
    }

    [Fact]
    public void ClassWithDelegateAttributeShouldBeCallable()
    {
        assembly.GetInstance<Action>("ClassWithTypeDelegateConstraint");
    }

    [Fact]
    public void ClassWithDelegateAttributeShouldBeCallable2()
    {
        assembly.GetInstance<Func<int>>("ClassWithTypeDelegateConstraint");
    }

    [Fact]
    public void ClassWithDelegateAttributeShouldHaveDelegateConstraint()
    {
        var genericParameterConstraints = assembly.GetType("ClassWithTypeDelegateConstraint`1").GetGenericArguments();
        Assert.Equal(typeof(Delegate), genericParameterConstraints.First().BaseType);
    }

    [Fact]
    public void ClassWithDelegateAttributeShouldHaveDelegateConstraint2()
    {
        var genericParameterConstraints = assembly.GetType("ClassWithTypeDelegateConstraint2`1").GetGenericArguments();
        Assert.Equal(typeof(Func<int>), genericParameterConstraints.First().BaseType);
    }

    [Fact]
    public void InterfaceWithDelegateAttributeShouldHaveDelegateConstraint()
    {
        var genericParameterConstraints = assembly.GetType("InterfaceWithTypeDelegateConstraint`1").GetGenericArguments();
        Assert.Equal(typeof(Delegate), genericParameterConstraints.First().BaseType);
    }

    [Fact]
    public void InterfaceWithDelegateAttributeShouldHaveDelegateConstraint2()
    {
        var genericParameterConstraints = assembly.GetType("InterfaceWithTypeDelegateConstraint2`1").GetGenericArguments();
        Assert.Equal(typeof(Func<int>), genericParameterConstraints.First().BaseType);
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

    public WeaverTests(ITestOutputHelper output) :
        base(output)
    {
    }
}