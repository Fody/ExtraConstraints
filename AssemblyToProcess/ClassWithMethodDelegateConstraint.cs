using System;
using ExtraConstraints;

public class ClassWithMethodDelegateConstraint
{
    public void Method<[DelegateConstraint] T> ()
    {

    }
}

public class ClassWithMethodDelegateConstraint2
{
    public void Method<[DelegateConstraint(typeof(Func<int>))] T>()
    {

    }
}