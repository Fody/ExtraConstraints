using System;
using ExtraConstraints;

public interface InterfaceWithTypeEnumConstraint<[EnumConstraint] T>;

public interface InterfaceWithTypeEnumConstraint2<[EnumConstraint(typeof(ConsoleColor))] T>;