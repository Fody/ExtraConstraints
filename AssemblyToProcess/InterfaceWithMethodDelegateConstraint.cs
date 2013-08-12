using ExtraConstraints;

public interface InterfaceWithMethodDelegateConstraint
{
	void Method<[DelegateConstraint] T>();
}

public interface InterfaceWithMethodDelegateConstraint2
{
    void Method<[DelegateConstraint(typeof(System.Func<int>))] T>();
}