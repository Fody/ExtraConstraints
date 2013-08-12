using System;

namespace ExtraConstraints
{

    /// <summary>
    /// Adds an Delegate constraint to a <see cref="AttributeTargets.GenericParameter"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.GenericParameter)]
    public class DelegateConstraintAttribute : Attribute
    {
        private readonly Type _type;

        /// <summary>
        /// Constructor
        /// </summary>
        public DelegateConstraintAttribute()
        {
            _type = typeof(Delegate);
        }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">A type object representing the type of delegate to constrain the generic parameter to</param>
        public DelegateConstraintAttribute(Type type)
        {
            if (!typeof(Delegate).IsAssignableFrom(type))
                throw new ArgumentException(string.Format("The given type {0} is not a delegate", type.FullName));
            _type = type;
        }
        
        /// <summary>
        /// Gets the type representing the delegate
        /// </summary>
        public Type Type
        {
            get { return _type; }
        }
    }
}