using System;

namespace ExtraConstraints
{

    /// <summary>
    /// Adds an Delegate constraint to a <see cref="AttributeTargets.GenericParameter"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.GenericParameter)]
    public class DelegateConstraintAttribute : Attribute
    {
    }
}