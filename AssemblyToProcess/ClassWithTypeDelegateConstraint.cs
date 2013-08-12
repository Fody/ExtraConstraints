using ExtraConstraints;

public class ClassWithTypeDelegateConstraint<[DelegateConstraint] T>
{
}

public class ClassWithTypeDelegateConstraint2<[DelegateConstraint(typeof(System.Func<int>))] T>
{
}