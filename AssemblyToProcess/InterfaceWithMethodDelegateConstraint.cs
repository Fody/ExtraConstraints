using ExtraConstraints;

public interface InterfaceWithMethodDelegateConstraint
{
	void Method<[DelegateConstraint] T>();
}