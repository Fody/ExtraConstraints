using ExtraConstraints;

public interface InterfaceWithTypeEnumConstraint<[EnumConstraint] T>
{
}

public interface InterfaceWithTypeEnumConstraint2<[EnumConstraint(typeof(System.ConsoleColor))] T>
{
}
