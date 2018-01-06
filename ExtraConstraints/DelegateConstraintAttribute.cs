using System;

namespace ExtraConstraints
{
    /// <summary>
    /// Adds an <see cref="Delegate"/> constraint to a <see cref="AttributeTargets.GenericParameter"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.GenericParameter)]
    public class DelegateConstraintAttribute : Attribute
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public DelegateConstraintAttribute()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="type">A <see cref="Type"/> representing the type of delegate to constrain the generic parameter to.</param>
        // ReSharper disable once UnusedParameter.Local
        public DelegateConstraintAttribute(Type type)
        {
        }
    }
}