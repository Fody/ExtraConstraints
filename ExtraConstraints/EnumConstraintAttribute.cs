using System;

namespace ExtraConstraints
{
    /// <summary>
    /// Adds an Enum constraint to a <see cref="AttributeTargets.GenericParameter"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.GenericParameter)]
    public class EnumConstraintAttribute : Attribute
    {
    }
}