using ExtraConstraints;

public interface InterfaceWithMethodEnumConstraint
{
	void Method<[EnumConstraint] T>();
}