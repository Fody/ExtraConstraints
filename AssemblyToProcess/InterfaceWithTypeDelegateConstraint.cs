using System;
using ExtraConstraints;

public interface InterfaceWithTypeDelegateConstraint<[DelegateConstraint] T>;

public interface InterfaceWithTypeDelegateConstraint2<[DelegateConstraint(typeof(Func<int>))] T>;