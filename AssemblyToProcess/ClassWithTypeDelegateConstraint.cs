using System;
using ExtraConstraints;

public class ClassWithTypeDelegateConstraint<[DelegateConstraint] T>;

public class ClassWithTypeDelegateConstraint2<[DelegateConstraint(typeof(Func<int>))] T>;