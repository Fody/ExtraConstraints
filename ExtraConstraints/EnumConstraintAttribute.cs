using System;

namespace ExtraConstraints
{
    /// <summary>
    /// Adds an Enum constraint to a <see cref="AttributeTargets.GenericParameter"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.GenericParameter)]
    public class EnumConstraintAttribute : Attribute
    {
        private readonly Type _type;
        
        /// <summary>
        /// Constructor
        /// </summary>
        public EnumConstraintAttribute()
        {
            _type = typeof(System.Enum);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="type">A type object representing the type of enum to constrain the generic parameter to</param>
        public EnumConstraintAttribute(Type type)
        {
            if (!type.IsEnum)
                throw new ArgumentException(string.Format("The given type {0} is not an enum", type.FullName));
            _type = type;
        }

        /// <summary>
        /// Gets the type representing the enum 
        /// </summary>
        public Type Type
        {
            get { return _type; }
        }
    }
}