using System;
using System.Linq;
using System.Reflection;
using Fody;
using TestResult = Fody.TestResult;
using System.Threading.Tasks;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

public class WeaverTests
{
    static TestResult testResult;
    static Assembly assembly;

    static WeaverTests()
    {
        var weaver = new ModuleWeaver();
        testResult = weaver.ExecuteTestRun("AssemblyToProcess.dll");
        assembly = testResult.Assembly;
    }

    [Test]
    public async Task MethodEnumAttributeShouldThrowWhenPassedANonEnum()
    {
        var exception = Try(() =>
        {
            var instance = testResult.GetInstance("ClassWithMethodEnumConstraint");
            instance.Method<string>();
        });
        await Assert.That(exception!.Message).IsEqualTo("The type 'string' must be a non-nullable value type in order to use it as parameter 'T' in the generic type or method 'ClassWithMethodEnumConstraint.Method<T>()'");
    }

    [Test]
    public async Task MethodEnumAttributeShouldThrowWhenPassedAnInCompatibleEnum()
    {
        var exception = Try(() =>
        {
            var instance = testResult.GetInstance("ClassWithMethodEnumConstraint2");
            instance.Method<ConsoleKey>();
        });
        await Assert.That(exception!.Message).IsEqualTo("The type 'System.ConsoleKey' cannot be used as type parameter 'T' in the generic type or method 'ClassWithMethodEnumConstraint2.Method<T>()'. There is no boxing conversion from 'System.ConsoleKey' to 'System.ConsoleColor'.");
    }

    [Test]
    public async Task MethodWithEnumAttributeShouldBeCallable()
    {
        var instance = testResult.GetInstance("ClassWithMethodEnumConstraint");
        instance.Method<AttributeTargets>();
    }

    [Test]
    public async Task MethodWithEnumAttributeShouldHaveDelegateConstraint()
    {
        var genericParameterConstraints = assembly.GetType("ClassWithMethodEnumConstraint")
            .GetMethods()
            .First(_ => _.Name == "Method")
            .GetGenericArguments();
        await Assert.That(genericParameterConstraints.First().BaseType).IsEqualTo(typeof(Enum));
    }

    [Test]
    public async Task MethodWithEnumAttributeShouldHaveDelegateConstraint2()
    {
        var genericParameterConstraints = assembly.GetType("ClassWithMethodEnumConstraint2")
            .GetMethods()
            .First(_ => _.Name == "Method")
            .GetGenericArguments();
        await Assert.That(genericParameterConstraints.First().BaseType).IsEqualTo(typeof(ConsoleColor));
    }

    [Test]
    public async Task InterfaceMethodWithEnumAttributeShouldHaveDelegateConstraint()
    {
        var genericParameterConstraints = assembly.GetType("InterfaceWithMethodEnumConstraint")
            .GetMethods()
            .First(_ => _.Name == "Method")
            .GetGenericArguments();
        await Assert.That(genericParameterConstraints.First().BaseType).IsEqualTo(typeof(Enum));
    }

    [Test]
    public async Task InterfaceMethodWithEnumAttributeShouldHaveDelegateConstraint2()
    {
        var genericParameterConstraints = assembly.GetType("InterfaceWithMethodEnumConstraint2")
            .GetMethods()
            .First(_ => _.Name == "Method")
            .GetGenericArguments();
        await Assert.That(genericParameterConstraints.First().BaseType).IsEqualTo(typeof(ConsoleColor));
    }

    [Test]
    public async Task ReferenceToExtraConstraintsShouldBeRemoved()
    {
        await Assert.That(assembly.GetReferencedAssemblies().Any(_ => _.Name == "ExtraConstraints")).IsFalse();
    }

    [Test]
    public async Task MethodDelegateAttributeShouldThrowWhenPassedANonDelegate()
    {
        var exception = Try(() =>
        {
            var instance = testResult.GetInstance("ClassWithMethodDelegateConstraint");
            instance.Method<string>();
        });
        await Assert.That(exception!.Message).IsEqualTo("The type 'string' cannot be used as type parameter 'T' in the generic type or method 'ClassWithMethodDelegateConstraint.Method<T>()'. There is no implicit reference conversion from 'string' to 'System.Delegate'.");
    }

    [Test]
    public async Task MethodDelegateAttributeShouldThrowWhenPassedAnIncompatibleDelegate()
    {
        var exception = Try(() =>
        {
            var instance = testResult.GetInstance("ClassWithMethodDelegateConstraint2");
            instance.Method<Func<string>>();
        });
        await Assert.That(exception!.Message).IsEqualTo("The type 'System.Func<string>' cannot be used as type parameter 'T' in the generic type or method 'ClassWithMethodDelegateConstraint2.Method<T>()'. There is no implicit reference conversion from 'System.Func<string>' to 'System.Func<int>'.");
    }

    [Test]
    public async Task MethodWithDelegateAttributeShouldBeCallable()
    {
        var instance = testResult.GetInstance("ClassWithMethodDelegateConstraint");
        instance.Method<Action>();
    }

    [Test]
    public async Task MethodWithDelegateAttributeShouldHaveDelegateConstraint()
    {
        var genericParameterConstraints = assembly.GetType("ClassWithMethodDelegateConstraint")
            .GetMethods()
            .First(_ => _.Name == "Method")
            .GetGenericArguments();
        await Assert.That(genericParameterConstraints.First().BaseType).IsEqualTo(typeof(Delegate));
    }

    [Test]
    public async Task MethodWithDelegateAttributeShouldHaveDelegateConstraint2()
    {
        var genericParameterConstraints = assembly.GetType("ClassWithMethodDelegateConstraint2")
            .GetMethods()
            .First(_ => _.Name == "Method")
            .GetGenericArguments();
        await Assert.That(genericParameterConstraints.First().BaseType).IsEqualTo(typeof(Func<int>));
    }

    [Test]
    public async Task InterfaceMethodWithDelegateAttributeShouldHaveDelegateConstraint()
    {
        var genericParameterConstraints = assembly.GetType("InterfaceWithMethodDelegateConstraint")
            .GetMethods()
            .First(_ => _.Name == "Method")
            .GetGenericArguments();
        await Assert.That(genericParameterConstraints.First().BaseType).IsEqualTo(typeof(Delegate));
    }

    [Test]
    public async Task InterfaceMethodWithDelegateAttributeShouldHaveDelegateConstraint2()
    {
        var genericParameterConstraints = assembly.GetType("InterfaceWithMethodDelegateConstraint2")
            .GetMethods()
            .First(_ => _.Name == "Method")
            .GetGenericArguments();
        await Assert.That(genericParameterConstraints.First().BaseType).IsEqualTo(typeof(Func<int>));
    }

    [Test]
    public async Task ClassWithEnumAttributeShouldThrowWhenPassedANonEnum()
    {
        var exception = Try(() => assembly.GetInstance<string>("ClassWithTypeEnumConstraint"));
        await Assert.That(exception!.Message).IsEqualTo("GenericArguments[0], 'System.String', on 'ClassWithTypeEnumConstraint`1[T]' violates the constraint of type 'T'.");
    }

    [Test]
    public async Task ClassWithEnumAttributeShouldThrowWhenPassedAnIncompatibleEnum()
    {
        var exception = Try(() => assembly.GetInstance<ConsoleKey>("ClassWithTypeEnumConstraint2"));
        await Assert.That(exception!.Message).IsEqualTo("GenericArguments[0], 'System.ConsoleKey', on 'ClassWithTypeEnumConstraint2`1[T]' violates the constraint of type 'T'.");
    }

    [Test]
    public async Task ClassWithEnumAttributeShouldBeCallable()
    {
        assembly.GetInstance<AttributeTargets>("ClassWithTypeEnumConstraint");
    }

    [Test]
    public async Task ClassWithEnumAttributeShouldBeCallable2()
    {
        assembly.GetInstance<ConsoleColor>("ClassWithTypeEnumConstraint");
    }

    [Test]
    public async Task ClassWithEnumAttributeShouldHaveEnumConstraint()
    {
        var genericParameterConstraints = assembly.GetType("ClassWithTypeEnumConstraint`1").GetGenericArguments();
        await Assert.That(genericParameterConstraints.First().BaseType).IsEqualTo(typeof(Enum));
    }

    [Test]
    public async Task ClassWithEnumAttributeShouldHaveEnumConstraint2()
    {
        var genericParameterConstraints = assembly.GetType("ClassWithTypeEnumConstraint2`1").GetGenericArguments();
        await Assert.That(genericParameterConstraints.First().BaseType).IsEqualTo(typeof(ConsoleColor));
    }

    [Test]
    public async Task InterfaceWithEnumAttributeShouldHaveEnumConstraint()
    {
        var genericParameterConstraints = assembly.GetType("InterfaceWithTypeEnumConstraint`1").GetGenericArguments();
        await Assert.That(genericParameterConstraints.First().BaseType).IsEqualTo(typeof(Enum));
    }

    [Test]
    public async Task InterfaceWithEnumAttributeShouldHaveEnumConstraint2()
    {
        var genericParameterConstraints = assembly.GetType("InterfaceWithTypeEnumConstraint2`1").GetGenericArguments();
        await Assert.That(genericParameterConstraints.First().BaseType).IsEqualTo(typeof(ConsoleColor));
    }

    [Test]
    public async Task ClassWithDelegateAttributeShouldThrowWhenPassedNonDelegate()
    {
        var exception = Try(() => assembly.GetInstance<string>("ClassWithTypeDelegateConstraint"));
        await Assert.That(exception!.Message).IsEqualTo("GenericArguments[0], 'System.String', on 'ClassWithTypeDelegateConstraint`1[T]' violates the constraint of type 'T'.");
    }

    [Test]
    public async Task ClassWithDelegateAttributeShouldThrowWhenPassedIncompatibleDelegate()
    {
        var exception = Try(() => assembly.GetInstance<Func<string>>("ClassWithTypeDelegateConstraint2"));
        await Assert.That(exception!.Message).IsEqualTo("GenericArguments[0], 'System.Func`1[System.String]', on 'ClassWithTypeDelegateConstraint2`1[T]' violates the constraint of type 'T'.");
    }

    [Test]
    public async Task ClassWithDelegateAttributeShouldBeCallable()
    {
        assembly.GetInstance<Action>("ClassWithTypeDelegateConstraint");
    }

    [Test]
    public async Task ClassWithDelegateAttributeShouldBeCallable2()
    {
        assembly.GetInstance<Func<int>>("ClassWithTypeDelegateConstraint");
    }

    [Test]
    public async Task ClassWithDelegateAttributeShouldHaveDelegateConstraint()
    {
        var genericParameterConstraints = assembly.GetType("ClassWithTypeDelegateConstraint`1").GetGenericArguments();
        await Assert.That(genericParameterConstraints.First().BaseType).IsEqualTo(typeof(Delegate));
    }

    [Test]
    public async Task ClassWithDelegateAttributeShouldHaveDelegateConstraint2()
    {
        var genericParameterConstraints = assembly.GetType("ClassWithTypeDelegateConstraint2`1").GetGenericArguments();
        await Assert.That(genericParameterConstraints.First().BaseType).IsEqualTo(typeof(Func<int>));
    }

    [Test]
    public async Task InterfaceWithDelegateAttributeShouldHaveDelegateConstraint()
    {
        var genericParameterConstraints = assembly.GetType("InterfaceWithTypeDelegateConstraint`1").GetGenericArguments();
        await Assert.That(genericParameterConstraints.First().BaseType).IsEqualTo(typeof(Delegate));
    }

    [Test]
    public async Task InterfaceWithDelegateAttributeShouldHaveDelegateConstraint2()
    {
        var genericParameterConstraints = assembly.GetType("InterfaceWithTypeDelegateConstraint2`1").GetGenericArguments();
        await Assert.That(genericParameterConstraints.First().BaseType).IsEqualTo(typeof(Func<int>));
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
}