using System;
using ExtraConstraints;

public class ClassWithTypeEnumConstraint<[EnumConstraint] T>
{
}

public class ClassWithTypeEnumConstraint2<[EnumConstraint(typeof(ConsoleColor))] T>
{
}