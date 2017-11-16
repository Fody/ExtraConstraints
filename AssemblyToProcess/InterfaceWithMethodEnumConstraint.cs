using System;
using ExtraConstraints;

public interface InterfaceWithMethodEnumConstraint
{
    void Method<[EnumConstraint] T>();
}

public interface InterfaceWithMethodEnumConstraint2
{
    void Method<[EnumConstraint(typeof(ConsoleColor))] T>();
}